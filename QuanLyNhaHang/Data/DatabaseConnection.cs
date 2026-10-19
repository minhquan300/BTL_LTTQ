using Microsoft.Data.SqlClient;

namespace QuanLyNhaHang.Data
{
    public class DatabaseConnection
    {
        private static readonly string _connectionString =
            "Server=.\\SQLEXPRESS;Database=QuanLyNhaHang;Trusted_Connection=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public static async Task<bool> TestConnectionAsync()
        {
            try
            {
                using var conn = GetConnection();
                await conn.OpenAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
