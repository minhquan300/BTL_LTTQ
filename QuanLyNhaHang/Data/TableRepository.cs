using Microsoft.Data.SqlClient;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Data
{
    public class TableRepository
    {
        public async Task<List<Table>> GetAllAsync()
        {
            var list = new List<Table>();
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand("SELECT TableID, TableName, QRCode, Capacity, TableStatus FROM DiningTables ORDER BY TableName", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Table
                {
                    TableID = reader.GetInt32(0),
                    TableName = reader.GetString(1),
                    QRCode = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Capacity = reader.GetInt32(3),
                    TableStatus = reader.GetString(4)
                });
            }
            return list;
        }

        public async Task<Table?> GetByIdAsync(int id)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand("SELECT TableID, TableName, QRCode, Capacity, TableStatus FROM DiningTables WHERE TableID=@ID", conn);
            cmd.Parameters.AddWithValue("@ID", id);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return new Table
                {
                    TableID = reader.GetInt32(0),
                    TableName = reader.GetString(1),
                    QRCode = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Capacity = reader.GetInt32(3),
                    TableStatus = reader.GetString(4)
                };
            return null;
        }

        public async Task<int> CreateAsync(Table table)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                INSERT INTO DiningTables (TableName, QRCode, Capacity, TableStatus)
                OUTPUT INSERTED.TableID
                VALUES (@Name, @QR, @Cap, @Status)", conn);
            cmd.Parameters.AddWithValue("@Name", table.TableName);
            cmd.Parameters.AddWithValue("@QR", (object?)table.QRCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Cap", table.Capacity);
            cmd.Parameters.AddWithValue("@Status", table.TableStatus);
            return (int)(await cmd.ExecuteScalarAsync())!;
        }

        public async Task UpdateAsync(Table table)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                UPDATE DiningTables 
                SET TableName=@Name, QRCode=@QR, Capacity=@Cap, TableStatus=@Status
                WHERE TableID=@ID", conn);
            cmd.Parameters.AddWithValue("@Name", table.TableName);
            cmd.Parameters.AddWithValue("@QR", (object?)table.QRCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Cap", table.Capacity);
            cmd.Parameters.AddWithValue("@Status", table.TableStatus);
            cmd.Parameters.AddWithValue("@ID", table.TableID);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdateStatusAsync(int tableId, string status)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand("UPDATE DiningTables SET TableStatus=@Status WHERE TableID=@ID", conn);
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@ID", tableId);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int tableId)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand("DELETE FROM DiningTables WHERE TableID=@ID", conn);
            cmd.Parameters.AddWithValue("@ID", tableId);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
