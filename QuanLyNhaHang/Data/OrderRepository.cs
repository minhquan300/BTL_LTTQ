using Microsoft.Data.SqlClient;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Service;
using System.Data;

namespace QuanLyNhaHang.Data
{
    public class OrderRepository
    {
        // ===== Orders =====
        public async Task<int> CreateOrderAsync(int tableId, int employeeId)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                INSERT INTO Orders (OrderCode, TableID, EmployeeID, OrderTime, OrderStatus)
                OUTPUT INSERTED.OrderID
                VALUES (@OrderCode, @TID, @EID, SYSDATETIME(), N'Moi')", conn);
            cmd.Parameters.Add("@OrderCode", SqlDbType.VarChar, 30).Value =
                "DH" + Guid.NewGuid().ToString("N")[..28].ToUpperInvariant();
            cmd.Parameters.AddWithValue("@TID", tableId);
            cmd.Parameters.AddWithValue("@EID", employeeId);
            var id = (int)(await cmd.ExecuteScalarAsync())!;

            // Update table status
            var upd = new SqlCommand("UPDATE DiningTables SET TableStatus=N'Dang phuc vu' WHERE TableID=@TID", conn);
            upd.Parameters.AddWithValue("@TID", tableId);
            await upd.ExecuteNonQueryAsync();

            return id;
        }

        public async Task<List<Order>> GetActiveOrdersAsync()
        {
            var list = new List<Order>();
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                SELECT o.OrderID, o.TableID, t.TableName, o.EmployeeID, e.FullName,
                       o.OrderTime, o.OrderStatus
                FROM Orders o
                INNER JOIN DiningTables t ON o.TableID = t.TableID
                INNER JOIN Employees e ON o.EmployeeID = e.EmployeeID
                WHERE o.OrderStatus NOT IN (N'Hoan thanh', N'Da thanh toan', N'Da huy')
                ORDER BY o.OrderTime DESC", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(MapOrder(reader));
            return list;
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                SELECT o.OrderID, o.TableID, t.TableName, o.EmployeeID, e.FullName,
                       o.OrderTime, o.OrderStatus
                FROM Orders o
                INNER JOIN DiningTables t ON o.TableID = t.TableID
                INNER JOIN Employees e ON o.EmployeeID = e.EmployeeID
                WHERE o.OrderID=@ID", conn);
            cmd.Parameters.AddWithValue("@ID", orderId);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var order = MapOrder(reader);
                await reader.CloseAsync();
                order.Details = await GetOrderDetailsAsync(orderId, conn);
                return order;
            }
            return null;
        }

        public async Task<Order?> GetActiveOrderByTableAsync(int tableId)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                SELECT TOP 1 o.OrderID, o.TableID, t.TableName, o.EmployeeID, e.FullName,
                       o.OrderTime, o.OrderStatus
                FROM Orders o
                INNER JOIN DiningTables t ON o.TableID = t.TableID
                INNER JOIN Employees e ON o.EmployeeID = e.EmployeeID
                WHERE o.TableID=@TID AND o.OrderStatus NOT IN (N'Hoan thanh', N'Da thanh toan', N'Da huy')
                ORDER BY o.OrderTime DESC", conn);
            cmd.Parameters.AddWithValue("@TID", tableId);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var order = MapOrder(reader);
                await reader.CloseAsync();
                order.Details = await GetOrderDetailsAsync(order.OrderID, conn);
                return order;
            }
            return null;
        }

        public async Task UpdateOrderStatusAsync(int orderId, string status, string? note = null)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            string? oldStatus;
            using (var getOld = new SqlCommand("SELECT OrderStatus FROM Orders WHERE OrderID=@ID", conn))
            {
                getOld.Parameters.AddWithValue("@ID", orderId);
                oldStatus = (string?)await getOld.ExecuteScalarAsync();
            }
            var cmd = new SqlCommand("UPDATE Orders SET OrderStatus=@S WHERE OrderID=@ID", conn);
            cmd.Parameters.AddWithValue("@S", status);
            cmd.Parameters.AddWithValue("@ID", orderId);
            await cmd.ExecuteNonQueryAsync();
            try
            {
                using var history = new SqlCommand(@"
                    INSERT INTO OrderStatusHistory (OrderID, OldStatus, NewStatus, ChangedAt, EmployeeID, Note)
                    VALUES (@ID, @OldStatus, @NewStatus, SYSDATETIME(), @EmployeeID, @Note)", conn);
                history.Parameters.AddWithValue("@ID", orderId);
                history.Parameters.AddWithValue("@OldStatus", (object?)oldStatus ?? DBNull.Value);
                history.Parameters.AddWithValue("@NewStatus", status);
                history.Parameters.AddWithValue("@EmployeeID", (object?)AuthService.Instance.CurrentUser?.EmployeeID ?? DBNull.Value);
                history.Parameters.AddWithValue("@Note", (object?)note ?? DBNull.Value);
                await history.ExecuteNonQueryAsync();
            }
            catch
            {
                // Keep order status updates working if history logging is unavailable.
            }
        }

        public async Task<List<Order>> GetOrderHistoryAsync(DateTime from, DateTime to)
        {
            var list = new List<Order>();
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                SELECT o.OrderID, o.TableID, t.TableName, o.EmployeeID, e.FullName,
                       o.OrderTime, o.OrderStatus
                FROM Orders o
                INNER JOIN DiningTables t ON o.TableID = t.TableID
                INNER JOIN Employees e ON o.EmployeeID = e.EmployeeID
                WHERE o.OrderTime BETWEEN @From AND @To
                ORDER BY o.OrderTime DESC", conn);
            cmd.Parameters.AddWithValue("@From", from);
            cmd.Parameters.AddWithValue("@To", to);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(MapOrder(reader));
            return list;
        }

        public async Task<List<PaidInvoiceSummary>> GetPaidInvoicesByTableAsync(int tableId)
        {
            var invoices = new List<PaidInvoiceSummary>();
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand(@"
                SELECT o.OrderID, o.OrderCode, p.PaymentTime, t.TableName,
                       COALESCE(e.FullName, N'—'), p.PaymentMethod, p.Amount
                FROM Orders o
                INNER JOIN DiningTables t ON t.TableID=o.TableID
                INNER JOIN Payments p ON p.OrderID=o.OrderID AND p.PaymentStatus=N'Thanh cong'
                LEFT JOIN Employees e ON e.EmployeeID=p.EmployeeID
                WHERE o.TableID=@TableID AND o.OrderStatus=N'Da thanh toan'
                ORDER BY p.PaymentTime DESC, p.PaymentID DESC", conn);
            cmd.Parameters.Add("@TableID", SqlDbType.Int).Value = tableId;
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                invoices.Add(new PaidInvoiceSummary
                {
                    OrderID = reader.GetInt32(0),
                    OrderCode = reader.GetString(1),
                    PaymentTime = reader.GetDateTime(2),
                    TableName = reader.GetString(3),
                    CashierName = reader.GetString(4),
                    PaymentMethod = reader.GetString(5),
                    Amount = reader.GetDecimal(6)
                });
            }
            return invoices;
        }

        // ===== OrderDetails =====
        public async Task<List<OrderDetail>> GetOrderDetailsAsync(int orderId)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            return await GetOrderDetailsAsync(orderId, conn);
        }

        private static async Task<List<OrderDetail>> GetOrderDetailsAsync(int orderId, SqlConnection conn)
        {
            var list = new List<OrderDetail>();
            var cmd = new SqlCommand(@"
                SELECT od.OrderDetailID, od.OrderID, od.MenuItemID, m.ItemName,
                       od.Quantity, od.UnitPrice, od.ItemStatus, od.OrderedAt,
                       od.CookingStartedAt, od.CookingCompletedAt, od.ServedAt, od.Note,
                       t.TableName
                FROM OrderDetails od
                INNER JOIN MenuItems m ON od.MenuItemID = m.MenuItemID
                INNER JOIN Orders o ON od.OrderID = o.OrderID
                INNER JOIN DiningTables t ON o.TableID = t.TableID
                WHERE od.OrderID=@ID
                ORDER BY od.OrderedAt", conn);
            cmd.Parameters.AddWithValue("@ID", orderId);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(MapOrderDetail(reader));
            return list;
        }

        public async Task<int> AddOrderDetailAsync(OrderDetail detail)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            using var tran = conn.BeginTransaction();
            try
            {
                int? matchingDetailId = null;
                using (var find = new SqlCommand(@"
                    SELECT TOP (1) OrderDetailID
                    FROM OrderDetails WITH (UPDLOCK, HOLDLOCK)
                    WHERE OrderID=@OID AND MenuItemID=@MID
                      AND ItemStatus IN (N'Moi', N'Cho bep')
                      AND ISNULL(LTRIM(RTRIM(Note)), N'') = @Note
                    ORDER BY CASE WHEN ItemStatus=N'Moi' THEN 0 ELSE 1 END, OrderedAt", conn, tran))
                {
                    find.Parameters.Add("@OID", SqlDbType.Int).Value = detail.OrderID;
                    find.Parameters.Add("@MID", SqlDbType.Int).Value = detail.MenuItemID;
                    find.Parameters.Add("@Note", SqlDbType.NVarChar, 255).Value = (detail.Note ?? string.Empty).Trim();
                    var result = await find.ExecuteScalarAsync();
                    if (result != null && result != DBNull.Value) matchingDetailId = Convert.ToInt32(result);
                }

                int detailId;
                if (matchingDetailId.HasValue)
                {
                    using var update = new SqlCommand(
                        "UPDATE OrderDetails SET Quantity=Quantity+@Q WHERE OrderDetailID=@ID", conn, tran);
                    update.Parameters.Add("@Q", SqlDbType.Int).Value = detail.Quantity;
                    update.Parameters.Add("@ID", SqlDbType.Int).Value = matchingDetailId.Value;
                    await update.ExecuteNonQueryAsync();
                    detailId = matchingDetailId.Value;
                }
                else
                {
                    using var insert = new SqlCommand(@"
                        INSERT INTO OrderDetails (OrderID,MenuItemID,Quantity,UnitPrice,ItemStatus,Note)
                        OUTPUT INSERTED.OrderDetailID
                        VALUES (@OID,@MID,@Q,@P,N'Moi',@Note)", conn, tran);
                    insert.Parameters.Add("@OID", SqlDbType.Int).Value = detail.OrderID;
                    insert.Parameters.Add("@MID", SqlDbType.Int).Value = detail.MenuItemID;
                    insert.Parameters.Add("@Q", SqlDbType.Int).Value = detail.Quantity;
                    var priceParameter = insert.Parameters.Add("@P", SqlDbType.Decimal);
                    priceParameter.Precision = 18;
                    priceParameter.Scale = 2;
                    priceParameter.Value = detail.UnitPrice;
                    insert.Parameters.Add("@Note", SqlDbType.NVarChar, 255).Value = (object?)detail.Note ?? DBNull.Value;
                    detailId = Convert.ToInt32(await insert.ExecuteScalarAsync());
                }

                using var updateOrder = new SqlCommand(
                    "UPDATE Orders SET OrderStatus=N'Dang xu ly' WHERE OrderID=@OID AND OrderStatus=N'Moi'", conn, tran);
                updateOrder.Parameters.Add("@OID", SqlDbType.Int).Value = detail.OrderID;
                await updateOrder.ExecuteNonQueryAsync();
                tran.Commit();
                return detailId;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public async Task UpdateDetailStatusAsync(int detailId, string status, int? employeeId = null, string? note = null)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();

            // Fetch old status for history tracking
            string? oldStatus = null;
            using (var getOldCmd = new SqlCommand("SELECT ItemStatus FROM OrderDetails WHERE OrderDetailID=@ID", conn))
            {
                getOldCmd.Parameters.AddWithValue("@ID", detailId);
                oldStatus = (string?)await getOldCmd.ExecuteScalarAsync();
            }

            var cmd = new SqlCommand(@"
                UPDATE OrderDetails
                SET ItemStatus=@S,
                    CookingStartedAt=CASE WHEN @S=N'Dang che bien' THEN SYSDATETIME() ELSE CookingStartedAt END,
                    CookingCompletedAt=CASE WHEN @S=N'San sang' THEN SYSDATETIME() ELSE CookingCompletedAt END,
                    ServedAt=CASE WHEN @S=N'Da phuc vu' THEN SYSDATETIME() ELSE ServedAt END
                WHERE OrderDetailID=@ID", conn);
            cmd.Parameters.AddWithValue("@S", status);
            cmd.Parameters.AddWithValue("@ID", detailId);
            if (await cmd.ExecuteNonQueryAsync() == 0)
                throw new InvalidOperationException("Trạng thái món đã thay đổi; hãy tải lại đơn rồi thử lại.");

            // Insert into history table
            try
            {
                var histCmd = new SqlCommand(@"
                    INSERT INTO OrderDetailStatusHistory (OrderDetailID, OldStatus, NewStatus, ChangedAt, EmployeeID, Note)
                    VALUES (@DetailID, @OldStatus, @NewStatus, SYSDATETIME(), @EmpID, @Note)", conn);
                histCmd.Parameters.AddWithValue("@DetailID", detailId);
                histCmd.Parameters.AddWithValue("@OldStatus", (object?)oldStatus ?? DBNull.Value);
                histCmd.Parameters.AddWithValue("@NewStatus", status);
                histCmd.Parameters.AddWithValue("@EmpID", (object?)employeeId ?? (AuthService.Instance.CurrentUser?.EmployeeID ?? (object)DBNull.Value));
                histCmd.Parameters.AddWithValue("@Note", (object?)note ?? DBNull.Value);
                await histCmd.ExecuteNonQueryAsync();
            }
            catch
            {
                // Non-critical if history logging fails
            }
        }

        public async Task RemoveDetailAsync(int detailId, bool managerOverride = false)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                UPDATE OrderDetails SET ItemStatus=N'Da huy'
                WHERE OrderDetailID=@ID AND ItemStatus <> N'Da huy'
                  AND (@ManagerOverride=1 OR ItemStatus IN (N'Moi', N'Cho bep'))", conn);
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = detailId;
            cmd.Parameters.Add("@ManagerOverride", SqlDbType.Bit).Value = managerOverride;
            if (await cmd.ExecuteNonQueryAsync() == 0)
                throw new InvalidOperationException("Trạng thái món đã thay đổi; hãy tải lại đơn rồi thử lại.");
        }

        public async Task UpdateDetailQuantityAsync(int detailId, int quantity, bool managerOverride = false)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                UPDATE OrderDetails SET Quantity=@Q
                WHERE OrderDetailID=@ID AND ItemStatus <> N'Da huy'
                  AND (@ManagerOverride=1 OR ItemStatus IN (N'Moi', N'Cho bep'))", conn);
            cmd.Parameters.Add("@Q", SqlDbType.Int).Value = quantity;
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = detailId;
            cmd.Parameters.Add("@ManagerOverride", SqlDbType.Bit).Value = managerOverride;
            if (await cmd.ExecuteNonQueryAsync() == 0)
                throw new InvalidOperationException("Trạng thái món đã thay đổi; hãy tải lại đơn rồi thử lại.");
        }

        public async Task SendNewItemsToKitchenAsync(int orderId)
        {
            List<int> detailIds = new();
            using (var conn = DatabaseConnection.GetConnection())
            {
                await conn.OpenAsync();
                using var cmd = new SqlCommand("SELECT OrderDetailID FROM OrderDetails WHERE OrderID=@OrderID AND ItemStatus=N'Moi'", conn);
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync()) detailIds.Add(reader.GetInt32(0));
            }

            foreach (var detailId in detailIds)
                await UpdateDetailStatusAsync(detailId, "Cho bep");
        }

        public async Task UpdateDetailNoteAsync(int detailId, string? note)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand("UPDATE OrderDetails SET Note=@Note WHERE OrderDetailID=@ID", conn);
            cmd.Parameters.AddWithValue("@Note", string.IsNullOrWhiteSpace(note) ? (object)DBNull.Value : note.Trim());
            cmd.Parameters.AddWithValue("@ID", detailId);
            await cmd.ExecuteNonQueryAsync();
        }

        // ===== Kitchen view =====
        public async Task<List<OrderDetail>> GetPendingKitchenItemsAsync()
        {
            var list = new List<OrderDetail>();
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                SELECT od.OrderDetailID, od.OrderID, od.MenuItemID, m.ItemName,
                       od.Quantity, od.UnitPrice, od.ItemStatus, od.OrderedAt,
                       od.CookingStartedAt, od.CookingCompletedAt, od.ServedAt, od.Note,
                       t.TableName
                FROM OrderDetails od
                INNER JOIN MenuItems m ON od.MenuItemID = m.MenuItemID
                INNER JOIN Orders o ON od.OrderID = o.OrderID
                INNER JOIN DiningTables t ON o.TableID = t.TableID
                WHERE od.ItemStatus IN (N'Cho bep', N'Dang che bien', N'San sang')
                  AND o.OrderStatus IN (N'Moi', N'Dang xu ly')
                ORDER BY
                    CASE od.ItemStatus
                        WHEN N'Cho bep' THEN 1
                        WHEN N'Dang che bien' THEN 2
                        WHEN N'San sang' THEN 3
                        ELSE 4
                    END,
                    od.OrderedAt ASC", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(MapOrderDetail(reader));
            return list;
        }

        // ===== Service Time Tracking Logs =====
        public async Task<List<OrderDetail>> GetServiceTimeLogsAsync(DateTime from, DateTime to, string? filterStatus = null)
        {
            var list = new List<OrderDetail>();
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();

            string statusClause = string.IsNullOrEmpty(filterStatus) || filterStatus == "Tất cả"
                ? ""
                : "AND od.ItemStatus = @Status";

            var cmd = new SqlCommand($@"
                SELECT od.OrderDetailID, od.OrderID, od.MenuItemID, m.ItemName,
                       od.Quantity, od.UnitPrice, od.ItemStatus, od.OrderedAt,
                       od.CookingStartedAt, od.CookingCompletedAt, od.ServedAt, od.Note,
                       t.TableName
                FROM OrderDetails od
                INNER JOIN MenuItems m ON od.MenuItemID = m.MenuItemID
                INNER JOIN Orders o ON od.OrderID = o.OrderID
                INNER JOIN DiningTables t ON o.TableID = t.TableID
                WHERE od.OrderedAt BETWEEN @From AND @To
                {statusClause}
                ORDER BY od.OrderedAt DESC", conn);
            cmd.Parameters.AddWithValue("@From", from);
            cmd.Parameters.AddWithValue("@To", to.AddDays(1).AddSeconds(-1));
            if (!string.IsNullOrEmpty(statusClause))
                cmd.Parameters.AddWithValue("@Status", filterStatus);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(MapOrderDetail(reader));
            return list;
        }

        // ===== Service Time KPI Metrics =====
        public async Task<(double AvgWaitMinutes, double AvgCookingMinutes, double AvgDeliveryMinutes, int TotalItems, int OverdueCount)> GetCookingKPIMetricsAsync(DateTime from, DateTime to)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                SELECT
                    ISNULL(AVG(DATEDIFF(second, OrderedAt, ISNULL(CookingStartedAt, SYSDATETIME())) / 60.0), 0) AS AvgWait,
                    ISNULL(AVG(CASE WHEN CookingStartedAt IS NOT NULL THEN DATEDIFF(second, CookingStartedAt, ISNULL(CookingCompletedAt, SYSDATETIME())) / 60.0 ELSE NULL END), 0) AS AvgCooking,
                    ISNULL(AVG(CASE WHEN CookingCompletedAt IS NOT NULL THEN DATEDIFF(second, CookingCompletedAt, ISNULL(ServedAt, SYSDATETIME())) / 60.0 ELSE NULL END), 0) AS AvgDelivery,
                    COUNT(*) AS TotalCount,
                    ISNULL(SUM(CASE WHEN DATEDIFF(minute, OrderedAt, ISNULL(ServedAt, SYSDATETIME())) > 20 THEN 1 ELSE 0 END), 0) AS OverdueCount
                FROM OrderDetails
                WHERE OrderedAt BETWEEN @From AND @To
                  AND ItemStatus != N'Da huy'", conn);
            cmd.Parameters.AddWithValue("@From", from);
            cmd.Parameters.AddWithValue("@To", to.AddDays(1).AddSeconds(-1));

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return (
                    AvgWaitMinutes: Convert.ToDouble(reader[0]),
                    AvgCookingMinutes: Convert.ToDouble(reader[1]),
                    AvgDeliveryMinutes: Convert.ToDouble(reader[2]),
                    TotalItems: Convert.ToInt32(reader[3]),
                    OverdueCount: Convert.ToInt32(reader[4])
                );
            }
            return (0, 0, 0, 0, 0);
        }

        public async Task<DataTable> GetAverageCookingTimeByMenuItemAsync(DateTime from, DateTime to)
        {
            var dt = new DataTable();
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                SELECT
                    m.ItemName AS N'Tên món',
                    COUNT(od.OrderDetailID) AS N'Số lần chế biến',
                    CAST(AVG(DATEDIFF_BIG(millisecond, od.CookingStartedAt, od.CookingCompletedAt) / 60000.0) AS decimal(10,1)) AS N'Thời gian TB (phút)',
                    CAST(MIN(DATEDIFF_BIG(millisecond, od.CookingStartedAt, od.CookingCompletedAt) / 60000.0) AS decimal(10,1)) AS N'Nhanh nhất (phút)',
                    CAST(MAX(DATEDIFF_BIG(millisecond, od.CookingStartedAt, od.CookingCompletedAt) / 60000.0) AS decimal(10,1)) AS N'Chậm nhất (phút)'
                FROM OrderDetails od
                INNER JOIN MenuItems m ON od.MenuItemID = m.MenuItemID
                WHERE od.OrderedAt >= @From AND od.OrderedAt < @ToExclusive
                  AND od.CookingStartedAt IS NOT NULL
                  AND od.CookingCompletedAt IS NOT NULL
                  AND od.CookingCompletedAt >= od.CookingStartedAt
                  AND od.ItemStatus != N'Da huy'
                GROUP BY m.ItemName
                ORDER BY AVG(DATEDIFF_BIG(millisecond, od.CookingStartedAt, od.CookingCompletedAt) / 60000.0) DESC", conn);
            cmd.Parameters.AddWithValue("@From", from.Date);
            cmd.Parameters.AddWithValue("@ToExclusive", to.Date.AddDays(1));
            using var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            return dt;
        }

        // ===== Payment =====
        public async Task<int> CreatePaymentAsync(int orderId, int employeeId, decimal amount, string method,
            decimal cashReceived, decimal changeAmount, decimal discountAmount,
            int? managerApprovedBy = null, string? transactionCode = null)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            using var tran = conn.BeginTransaction();
            try
            {
                var orderCmd = new SqlCommand(@"
                    SELECT OrderStatus, TableID
                    FROM Orders WITH (UPDLOCK, HOLDLOCK)
                    WHERE OrderID=@OID", conn, tran);
                orderCmd.Parameters.AddWithValue("@OID", orderId);
                int tableId;
                string? orderStatus;
                using (var reader = await orderCmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        throw new InvalidOperationException("Không tìm thấy hóa đơn cần thanh toán.");
                    orderStatus = reader.GetString(0);
                    tableId = reader.GetInt32(1);
                }
                if (orderStatus == "Da thanh toan")
                    throw new InvalidOperationException("Hóa đơn này đã được thanh toán trước đó.");
                if (orderStatus == "Da huy")
                    throw new InvalidOperationException("Không thể thanh toán hóa đơn đã hủy.");

                var actorCmd = new SqlCommand(@"
                    SELECT e.RoleID
                    FROM Employees e
                    WHERE e.EmployeeID=@EID AND e.IsActive=1", conn, tran);
                actorCmd.Parameters.AddWithValue("@EID", employeeId);
                object? actorRole = await actorCmd.ExecuteScalarAsync();
                if (actorRole == null || actorRole == DBNull.Value ||
                    Convert.ToInt32(actorRole, System.Globalization.CultureInfo.InvariantCulture) is not (1 or 2))
                    throw new UnauthorizedAccessException("Chỉ Thu ngân hoặc Quản lý được thanh toán.");

                var totalCmd = new SqlCommand(@"
                    SELECT COALESCE(SUM(Quantity * UnitPrice), 0)
                    FROM OrderDetails WITH (UPDLOCK, HOLDLOCK)
                    WHERE OrderID=@OID AND ItemStatus<>N'Da huy'", conn, tran);
                totalCmd.Parameters.AddWithValue("@OID", orderId);
                decimal orderTotal = Convert.ToDecimal(await totalCmd.ExecuteScalarAsync(),
                    System.Globalization.CultureInfo.InvariantCulture);
                if (orderTotal <= 0 || amount <= 0 || discountAmount < 0 || discountAmount > orderTotal ||
                    amount != orderTotal - discountAmount)
                    throw new InvalidOperationException("Số tiền hoặc giảm giá không khớp với tổng hóa đơn hiện tại.");

                if (method is not ("Tien mat" or "Chuyen khoan" or "The"))
                    throw new InvalidOperationException("Phương thức thanh toán không hợp lệ.");
                if (method == "Tien mat")
                {
                    if (cashReceived < amount || changeAmount != cashReceived - amount)
                        throw new InvalidOperationException("Tiền khách đưa hoặc tiền thừa không hợp lệ.");
                }
                else if (cashReceived != amount || changeAmount != 0)
                    throw new InvalidOperationException("Thông tin tiền nhận không hợp lệ với phương thức đã chọn.");

                int? approvedBy = managerApprovedBy;
                if (orderTotal > 0 && discountAmount / orderTotal > 0.20m)
                {
                    if (approvedBy == null && Convert.ToInt32(actorRole,
                        System.Globalization.CultureInfo.InvariantCulture) == 1)
                        approvedBy = employeeId;
                    if (approvedBy == null)
                        throw new UnauthorizedAccessException("Giảm giá trên 20% cần Quản lý duyệt.");

                    var managerCmd = new SqlCommand(@"
                        SELECT COUNT(1) FROM Employees e
                        WHERE e.EmployeeID=@ManagerID AND e.IsActive=1 AND e.RoleID=1", conn, tran);
                    managerCmd.Parameters.AddWithValue("@ManagerID", approvedBy.Value);
                    if (Convert.ToInt32(await managerCmd.ExecuteScalarAsync(),
                        System.Globalization.CultureInfo.InvariantCulture) != 1)
                        throw new UnauthorizedAccessException("Tài khoản duyệt giảm giá không có quyền Quản lý.");
                }

                var payCmd = new SqlCommand(@"
                    INSERT INTO Payments (OrderID, EmployeeID, Amount, PaymentMethod, PaymentStatus, PaymentTime, TransactionCode)
                    OUTPUT INSERTED.PaymentID
                    VALUES (@OID, @EID, @Amt, @Method, N'Thanh cong', SYSDATETIME(), @TCode)", conn, tran);
                payCmd.Parameters.AddWithValue("@OID", orderId);
                payCmd.Parameters.AddWithValue("@EID", employeeId);
                payCmd.Parameters.AddWithValue("@Amt", amount);
                payCmd.Parameters.AddWithValue("@Method", method);
                payCmd.Parameters.Add("@TCode", SqlDbType.VarChar, 100).Value = (object?)transactionCode ?? DBNull.Value;
                var payId = (int)(await payCmd.ExecuteScalarAsync())!;

                var ordCmd = new SqlCommand(@"
                    UPDATE Orders
                    SET OrderStatus=N'Da thanh toan', PaymentMethod=@Method,
                        CashReceived=@Cash, ChangeAmount=@Change, DiscountAmount=@Discount,
                        DiscountApprovedBy=@ManagerID
                    WHERE OrderID=@OID AND OrderStatus<>N'Da thanh toan'", conn, tran);
                ordCmd.Parameters.AddWithValue("@OID", orderId);
                ordCmd.Parameters.AddWithValue("@Method", method);
                ordCmd.Parameters.AddWithValue("@Cash", cashReceived);
                ordCmd.Parameters.AddWithValue("@Change", changeAmount);
                ordCmd.Parameters.AddWithValue("@Discount", discountAmount);
                ordCmd.Parameters.Add("@ManagerID", SqlDbType.Int).Value = (object?)approvedBy ?? DBNull.Value;
                if (await ordCmd.ExecuteNonQueryAsync() != 1)
                    throw new InvalidOperationException("Hóa đơn đã được thanh toán hoặc không còn hợp lệ.");

                var tblUpd = new SqlCommand("UPDATE DiningTables SET TableStatus=N'Trong' WHERE TableID=@TID", conn, tran);
                tblUpd.Parameters.AddWithValue("@TID", tableId);
                if (await tblUpd.ExecuteNonQueryAsync() != 1)
                    throw new InvalidOperationException("Không thể cập nhật trạng thái bàn sau thanh toán.");

                tran.Commit();
                return payId;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public async Task<Payment?> GetPaymentByOrderAsync(int orderId)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                SELECT p.PaymentID, p.OrderID, p.EmployeeID, e.FullName,
                       p.Amount, p.PaymentMethod, p.PaymentStatus, p.PaymentTime
                FROM Payments p
                INNER JOIN Employees e ON p.EmployeeID = e.EmployeeID
                WHERE p.OrderID=@ID", conn);
            cmd.Parameters.AddWithValue("@ID", orderId);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return new Payment
                {
                    PaymentID = reader.GetInt32(0),
                    OrderID = reader.GetInt32(1),
                    EmployeeID = reader.GetInt32(2),
                    EmployeeName = reader.GetString(3),
                    Amount = reader.GetDecimal(4),
                    PaymentMethod = reader.GetString(5),
                    PaymentStatus = reader.GetString(6),
                    PaymentTime = reader.GetDateTime(7)
                };
            return null;
        }

        // ===== Statistics =====
        public async Task<decimal> GetRevenueTodayAsync()
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                SELECT ISNULL(SUM(p.Amount), 0) FROM Payments p
                WHERE CAST(p.PaymentTime AS DATE) = CAST(GETDATE() AS DATE)
                  AND p.PaymentStatus = N'Thanh cong'", conn);
            return (decimal)(await cmd.ExecuteScalarAsync())!;
        }

        public async Task<DataTable> GetRevenueByDateAsync(DateTime from, DateTime to)
        {
            var dt = new DataTable();
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                SELECT CAST(p.PaymentTime AS DATE) AS N'Ngày',
                       COUNT(DISTINCT p.OrderID) AS N'Số đơn',
                       SUM(p.Amount) AS N'Doanh thu (đ)'
                FROM Payments p
                WHERE p.PaymentTime >= @From AND p.PaymentTime < @ToExclusive
                  AND p.PaymentStatus=N'Thanh cong'
                GROUP BY CAST(p.PaymentTime AS DATE)
                ORDER BY CAST(p.PaymentTime AS DATE)", conn);
            cmd.Parameters.AddWithValue("@From", from.Date);
            cmd.Parameters.AddWithValue("@ToExclusive", to.Date.AddDays(1));
            using var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            return dt;
        }

        public async Task<DataTable> GetTopSellingItemsAsync(int top = 10, DateTime? from = null, DateTime? to = null)
        {
            var dt = new DataTable();
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();

            string dateClause = from.HasValue && to.HasValue
                ? "AND od.OrderedAt BETWEEN @From AND @To"
                : "";

            var cmd = new SqlCommand($@"
                SELECT TOP ({top})
                    m.ItemName AS N'Tên món ăn',
                    c.CategoryName AS N'Danh mục',
                    SUM(od.Quantity) AS N'Số lượng bán',
                    m.Price AS N'Đơn giá (đ)',
                    SUM(od.Quantity * od.UnitPrice) AS N'Doanh thu (đ)'
                FROM OrderDetails od
                INNER JOIN MenuItems m ON od.MenuItemID = m.MenuItemID
                INNER JOIN Categories c ON m.CategoryID = c.CategoryID
                INNER JOIN Orders o ON od.OrderID = o.OrderID
                WHERE od.ItemStatus != N'Da huy'
                  {dateClause}
                GROUP BY m.ItemName, c.CategoryName, m.Price
                ORDER BY SUM(od.Quantity) DESC", conn);

            if (from.HasValue && to.HasValue)
            {
                cmd.Parameters.AddWithValue("@From", from.Value);
                cmd.Parameters.AddWithValue("@To", to.Value.AddDays(1).AddSeconds(-1));
            }

            using var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            return dt;
        }

        public async Task<DataTable> GetPaymentMethodStatsAsync(DateTime from, DateTime to)
        {
            var dt = new DataTable();
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                SELECT
                    CASE p.PaymentMethod
                        WHEN N'Tien mat' THEN N'Tiền mặt'
                        WHEN N'Chuyen khoan' THEN N'Chuyển khoản'
                        WHEN N'The' THEN N'Thẻ'
                        WHEN N'Card' THEN N'Thẻ'
                        ELSE p.PaymentMethod
                    END AS N'Phương thức',
                    COUNT(DISTINCT p.OrderID) AS N'Số đơn',
                    SUM(p.Amount) AS N'Doanh thu (đ)'
                FROM Payments p
                WHERE p.PaymentTime >= @From AND p.PaymentTime < @ToExclusive
                  AND p.PaymentStatus = N'Thanh cong'
                GROUP BY CASE p.PaymentMethod
                    WHEN N'Tien mat' THEN N'Tiền mặt'
                    WHEN N'Chuyen khoan' THEN N'Chuyển khoản'
                    WHEN N'The' THEN N'Thẻ'
                    WHEN N'Card' THEN N'Thẻ'
                    ELSE p.PaymentMethod
                END
                ORDER BY SUM(p.Amount) DESC", conn);
            cmd.Parameters.AddWithValue("@From", from.Date);
            cmd.Parameters.AddWithValue("@ToExclusive", to.Date.AddDays(1));
            using var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            return dt;
        }

        private static Order MapOrder(SqlDataReader r) => new()
        {
            OrderID = r.GetInt32(0),
            TableID = r.GetInt32(1),
            TableName = r.GetString(2),
            EmployeeID = r.GetInt32(3),
            EmployeeName = r.GetString(4),
            OrderTime = r.GetDateTime(5),
            OrderStatus = r.GetString(6)
        };

        private static OrderDetail MapOrderDetail(SqlDataReader r) => new()
        {
            OrderDetailID = r.GetInt32(0),
            OrderID = r.GetInt32(1),
            MenuItemID = r.GetInt32(2),
            ItemName = r.GetString(3),
            Quantity = r.GetInt32(4),
            UnitPrice = r.GetDecimal(5),
            ItemStatus = r.GetString(6),
            OrderedAt = r.GetDateTime(7),
            CookingStartedAt = r.IsDBNull(8) ? null : r.GetDateTime(8),
            CookingCompletedAt = r.IsDBNull(9) ? null : readerNullableDateTime(r, 9),
            ServedAt = r.IsDBNull(10) ? null : readerNullableDateTime(r, 10),
            Note = r.IsDBNull(11) ? null : r.GetString(11),
            TableName = r.FieldCount > 12 && !r.IsDBNull(12) ? r.GetString(12) : string.Empty
        };

        private static DateTime? readerNullableDateTime(SqlDataReader r, int i) =>
            r.IsDBNull(i) ? null : r.GetDateTime(i);
    }
}
