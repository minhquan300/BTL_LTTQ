using QuanLyNhaHang.Data;
using QuanLyNhaHang.Models;
using MenuItemModel = QuanLyNhaHang.Models.MenuItem;
using System.Data;
namespace QuanLyNhaHang.Service
{
    public class OrderService
    {
        private readonly OrderRepository _repo = new();

        public async Task<Order> CreateOrderAsync(int tableId)
        {
            var empId = AuthService.Instance.CurrentUser!.EmployeeID;
            var orderId = await _repo.CreateOrderAsync(tableId, empId);
            return (await _repo.GetOrderByIdAsync(orderId))!;
        }

        public async Task<Order?> GetOrCreateOrderForTableAsync(int tableId)
        {
            var existing = await _repo.GetActiveOrderByTableAsync(tableId);
            return existing ?? await CreateOrderAsync(tableId);
        }

        public async Task<Order?> GetOrderAsync(int orderId) =>
            await _repo.GetOrderByIdAsync(orderId);

        public async Task<List<Order>> GetActiveOrdersAsync() =>
            await _repo.GetActiveOrdersAsync();

        public async Task<List<PaidInvoiceSummary>> GetPaidInvoicesForTableAsync(int tableId)
        {
            var auth = AuthService.Instance;
            if (auth.CurrentUser == null || (!auth.IsAdmin && !auth.IsCashier))
                throw new UnauthorizedAccessException("Chỉ Thu ngân hoặc Quản lý được in lại hóa đơn.");
            return await _repo.GetPaidInvoicesByTableAsync(tableId);
        }

        public async Task<List<OrderDetail>> GetOrderDetailsAsync(int orderId) =>
            await _repo.GetOrderDetailsAsync(orderId);

        public async Task AddItemAsync(int orderId, MenuItemModel item, int qty, string? note = null)
        {
            await _repo.AddOrderDetailAsync(new OrderDetail
            {
                OrderID = orderId,
                MenuItemID = item.MenuItemID,
                Quantity = qty,
                UnitPrice = item.Price,
                Note = note
            });
        }

        public async Task RemoveItemAsync(int detailId, bool managerOverride = false) =>
            await _repo.RemoveDetailAsync(detailId, managerOverride);

        public async Task UpdateQuantityAsync(int detailId, int qty, bool managerOverride = false) =>
            await _repo.UpdateDetailQuantityAsync(detailId, qty, managerOverride);

        public async Task UpdateDetailNoteAsync(int detailId, string? note) =>
            await _repo.UpdateDetailNoteAsync(detailId, note);

        public async Task SendNewItemsToKitchenAsync(int orderId) =>
            await _repo.SendNewItemsToKitchenAsync(orderId);

        public async Task CancelOrderAsync(int orderId, int tableId, string? reason = null)
        {
            await _repo.UpdateOrderStatusAsync(orderId, "Da huy", reason);
            var tblRepo = new TableRepository();
            await tblRepo.UpdateStatusAsync(tableId, "Trong");
        }

        public async Task<int> ProcessPaymentAsync(int orderId, decimal amount, string method,
            decimal cashReceived, decimal changeAmount, decimal discountAmount,
            int? managerApprovedBy = null, string? transactionCode = null)
        {
            var auth = AuthService.Instance;
            if ((!auth.IsCashier && !auth.IsAdmin) || auth.CurrentUser == null)
                throw new UnauthorizedAccessException("Chỉ Thu ngân hoặc Quản lý được thanh toán.");
            var empId = auth.CurrentUser.EmployeeID;
            return await _repo.CreatePaymentAsync(orderId, empId, amount, method,
                cashReceived, changeAmount, discountAmount, managerApprovedBy, transactionCode);
        }

        public async Task<Payment?> GetPaymentAsync(int orderId) =>
            await _repo.GetPaymentByOrderAsync(orderId);

        public async Task<decimal> GetRevenueTodayAsync() =>
            await _repo.GetRevenueTodayAsync();

        public async Task<DataTable> GetRevenueReportAsync(DateTime from, DateTime to) =>
            await _repo.GetRevenueByDateAsync(from, to);
    }
}
