using Microsoft.Data.SqlClient;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Data
{
    public class UserRepository
    {
        public async Task<User?> GetByUsernameAsync(string username)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                SELECT e.EmployeeID, e.Username, e.PasswordHash, e.FullName, 
                       e.Phone, e.IsActive, e.RoleID, r.RoleName, e.CreatedAt
                FROM Employees e
                INNER JOIN Roles r ON e.RoleID = r.RoleID
                WHERE e.Username = @Username AND e.IsActive = 1", conn);
            cmd.Parameters.AddWithValue("@Username", username);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    EmployeeID = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    PasswordHash = reader.GetString(2),
                    FullName = reader.GetString(3),
                    Phone = reader.IsDBNull(4) ? null : reader.GetString(4),
                    IsActive = reader.GetBoolean(5),
                    RoleID = reader.GetInt32(6),
                    RoleName = reader.GetString(7),
                    CreatedAt = reader.GetDateTime(8)
                };
            }
            return null;
        }

        public async Task<List<User>> GetAllAsync()
        {
            var list = new List<User>();
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                SELECT e.EmployeeID, e.Username, e.PasswordHash, e.FullName,
                       e.Phone, e.IsActive, e.RoleID, r.RoleName, e.CreatedAt
                FROM Employees e
                INNER JOIN Roles r ON e.RoleID = r.RoleID
                ORDER BY e.FullName", conn);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new User
                {
                    EmployeeID = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    PasswordHash = reader.GetString(2),
                    FullName = reader.GetString(3),
                    Phone = reader.IsDBNull(4) ? null : reader.GetString(4),
                    IsActive = reader.GetBoolean(5),
                    RoleID = reader.GetInt32(6),
                    RoleName = reader.GetString(7),
                    CreatedAt = reader.GetDateTime(8)
                });
            }
            return list;
        }

        public async Task<int> CreateAsync(User user)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                INSERT INTO Employees (Username, PasswordHash, FullName, Phone, IsActive, RoleID)
                OUTPUT INSERTED.EmployeeID
                VALUES (@Username, @PasswordHash, @FullName, @Phone, @IsActive, @RoleID)", conn);
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            cmd.Parameters.AddWithValue("@FullName", user.FullName);
            cmd.Parameters.AddWithValue("@Phone", (object?)user.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
            cmd.Parameters.AddWithValue("@RoleID", user.RoleID);
            return (int)(await cmd.ExecuteScalarAsync())!;
        }

        public async Task UpdateAsync(User user)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                UPDATE Employees 
                SET FullName=@FullName, Phone=@Phone, IsActive=@IsActive, RoleID=@RoleID
                WHERE EmployeeID=@ID", conn);
            cmd.Parameters.AddWithValue("@FullName", user.FullName);
            cmd.Parameters.AddWithValue("@Phone", (object?)user.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
            cmd.Parameters.AddWithValue("@RoleID", user.RoleID);
            cmd.Parameters.AddWithValue("@ID", user.EmployeeID);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdatePasswordAsync(int employeeId, string newHash)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand("UPDATE Employees SET PasswordHash=@Hash WHERE EmployeeID=@ID", conn);
            cmd.Parameters.AddWithValue("@Hash", newHash);
            cmd.Parameters.AddWithValue("@ID", employeeId);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task LogLoginAttemptAsync(string username, int? employeeId, bool succeeded)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand(@"
                INSERT INTO dbo.LichSuDangNhap (Username, EmployeeID, LoginTime, IsSuccess)
                VALUES (@Username, @EmployeeID, SYSUTCDATETIME(), @IsSuccess)", conn);
            cmd.Parameters.Add("@Username", System.Data.SqlDbType.NVarChar, 100).Value =
                username.Length > 100 ? username[..100] : username;
            cmd.Parameters.Add("@EmployeeID", System.Data.SqlDbType.Int).Value =
                (object?)employeeId ?? DBNull.Value;
            cmd.Parameters.Add("@IsSuccess", System.Data.SqlDbType.Bit).Value = succeeded;
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task LogSecurityWarningAsync(string username, string eventType, string details)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand(@"
                INSERT INTO dbo.LichSuBaoMat (EventType, Username, Details, CreatedAt)
                VALUES (@EventType, @Username, @Details, SYSUTCDATETIME())", conn);
            cmd.Parameters.Add("@EventType", System.Data.SqlDbType.NVarChar, 60).Value = eventType;
            cmd.Parameters.Add("@Username", System.Data.SqlDbType.NVarChar, 100).Value =
                string.IsNullOrWhiteSpace(username) ? DBNull.Value : username.Trim()[..Math.Min(username.Trim().Length, 100)];
            cmd.Parameters.Add("@Details", System.Data.SqlDbType.NVarChar, 250).Value =
                details.Length > 250 ? details[..250] : details;
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task LogApprovalAsync(
            int managerEmployeeId,
            int? requesterEmployeeId,
            string? requesterUsername,
            string? requesterFullName,
            string actionDescription)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand(@"
                INSERT INTO dbo.LichSuDuyet
                    (MaQuanLyDuyet, MaNhanVienYeuCau, TenDangNhapYeuCau, TenNguoiYeuCau, MoTaThaoTac, ThoiGianDuyet)
                VALUES
                    (@MaQuanLyDuyet, @MaNhanVienYeuCau, @TenDangNhapYeuCau, @TenNguoiYeuCau, @MoTaThaoTac, SYSUTCDATETIME())", conn);
            cmd.Parameters.Add("@MaQuanLyDuyet", System.Data.SqlDbType.Int).Value = managerEmployeeId;
            cmd.Parameters.Add("@MaNhanVienYeuCau", System.Data.SqlDbType.Int).Value =
                (object?)requesterEmployeeId ?? DBNull.Value;
            cmd.Parameters.Add("@TenDangNhapYeuCau", System.Data.SqlDbType.NVarChar, 100).Value =
                (object?)LimitText(requesterUsername, 100) ?? DBNull.Value;
            cmd.Parameters.Add("@TenNguoiYeuCau", System.Data.SqlDbType.NVarChar, 150).Value =
                (object?)LimitText(requesterFullName, 150) ?? DBNull.Value;
            cmd.Parameters.Add("@MoTaThaoTac", System.Data.SqlDbType.NVarChar, 500).Value =
                LimitText(actionDescription, 500) ?? string.Empty;
            await cmd.ExecuteNonQueryAsync();
        }

        private static string? LimitText(string? value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            string trimmed = value.Trim();
            return trimmed.Length > maxLength ? trimmed[..maxLength] : trimmed;
        }

        public async Task<List<(int RoleID, string RoleName)>> GetRolesAsync()
        {
            var list = new List<(int, string)>();
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand("SELECT RoleID, RoleName FROM Roles ORDER BY RoleName", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add((reader.GetInt32(0), reader.GetString(1)));
            return list;
        }
    }
}
