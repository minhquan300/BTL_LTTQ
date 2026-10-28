using Microsoft.Data.SqlClient;
using QuanLyNhaHang.Models;
using MenuItemModel = QuanLyNhaHang.Models.MenuItem;

namespace QuanLyNhaHang.Data
{
    public class MenuRepository
    {
        // ===== Categories =====
        public async Task<List<Category>> GetCategoriesAsync()
        {
            var list = new List<Category>();
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand("SELECT CategoryID, CategoryName, Description, IsActive FROM Categories WHERE IsActive=1 ORDER BY CategoryName", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(new Category
                {
                    CategoryID = reader.GetInt32(0),
                    CategoryName = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    IsActive = reader.GetBoolean(3)
                });
            return list;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            var list = new List<Category>();
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand("SELECT CategoryID, CategoryName, Description, IsActive FROM Categories ORDER BY CategoryName", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(new Category
                {
                    CategoryID = reader.GetInt32(0),
                    CategoryName = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    IsActive = reader.GetBoolean(3)
                });
            return list;
        }

        public async Task<int> CreateCategoryAsync(Category cat)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"INSERT INTO Categories (CategoryName,Description,IsActive) OUTPUT INSERTED.CategoryID VALUES (@N,@D,@A)", conn);
            cmd.Parameters.AddWithValue("@N", cat.CategoryName);
            cmd.Parameters.AddWithValue("@D", (object?)cat.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@A", cat.IsActive);
            return (int)(await cmd.ExecuteScalarAsync())!;
        }

        public async Task UpdateCategoryAsync(Category cat)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand("UPDATE Categories SET CategoryName=@N, Description=@D, IsActive=@A WHERE CategoryID=@ID", conn);
            cmd.Parameters.AddWithValue("@N", cat.CategoryName);
            cmd.Parameters.AddWithValue("@D", (object?)cat.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@A", cat.IsActive);
            cmd.Parameters.AddWithValue("@ID", cat.CategoryID);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                DELETE FROM Categories
                WHERE CategoryID = @ID
                  AND NOT EXISTS (SELECT 1 FROM MenuItems WHERE CategoryID = @ID)", conn);
            cmd.Parameters.AddWithValue("@ID", categoryId);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        // ===== MenuItems =====
        public async Task<List<MenuItemModel>> GetAllMenuItemsAsync()
        {
            var list = new List<MenuItemModel>();
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                SELECT m.MenuItemID, m.ItemName, m.CategoryID, c.CategoryName,
                       m.Price, m.Description, m.ImagePath, m.IsAvailable
                FROM MenuItems m
                INNER JOIN Categories c ON m.CategoryID = c.CategoryID
                ORDER BY c.CategoryName, m.ItemName", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(MapMenuItem(reader));
            return list;
        }

        public async Task<List<MenuItemModel>> GetAvailableMenuItemsAsync()
        {
            var list = new List<MenuItemModel>();
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                SELECT m.MenuItemID, m.ItemName, m.CategoryID, c.CategoryName,
                       m.Price, m.Description, m.ImagePath, m.IsAvailable
                FROM MenuItems m
                INNER JOIN Categories c ON m.CategoryID = c.CategoryID
                WHERE m.IsAvailable=1 AND c.IsActive=1
                ORDER BY c.CategoryName, m.ItemName", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(MapMenuItem(reader));
            return list;
        }

        public async Task<List<MenuItemModel>> GetByCategoryAsync(int categoryId)
        {
            var list = new List<MenuItemModel>();
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                SELECT m.MenuItemID, m.ItemName, m.CategoryID, c.CategoryName,
                       m.Price, m.Description, m.ImagePath, m.IsAvailable
                FROM MenuItems m
                INNER JOIN Categories c ON m.CategoryID = c.CategoryID
                WHERE m.CategoryID=@CatID AND m.IsAvailable=1
                ORDER BY m.ItemName", conn);
            cmd.Parameters.AddWithValue("@CatID", categoryId);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(MapMenuItem(reader));
            return list;
        }

        public async Task<int> CreateMenuItemAsync(MenuItemModel item)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                INSERT INTO MenuItems (ItemName,CategoryID,Price,Description,ImagePath,IsAvailable)
                OUTPUT INSERTED.MenuItemID
                VALUES (@N,@C,@P,@D,@I,@A)", conn);
            cmd.Parameters.AddWithValue("@N", item.ItemName);
            cmd.Parameters.AddWithValue("@C", item.CategoryID);
            cmd.Parameters.AddWithValue("@P", item.Price);
            cmd.Parameters.AddWithValue("@D", (object?)item.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@I", (object?)item.ImagePath ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@A", item.IsAvailable);
            return (int)(await cmd.ExecuteScalarAsync())!;
        }

        public async Task UpdateMenuItemAsync(MenuItemModel item)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                UPDATE MenuItems 
                SET ItemName=@N, CategoryID=@C, Price=@P, Description=@D, ImagePath=@I, IsAvailable=@A
                WHERE MenuItemID=@ID", conn);
            cmd.Parameters.AddWithValue("@N", item.ItemName);
            cmd.Parameters.AddWithValue("@C", item.CategoryID);
            cmd.Parameters.AddWithValue("@P", item.Price);
            cmd.Parameters.AddWithValue("@D", (object?)item.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@I", (object?)item.ImagePath ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@A", item.IsAvailable);
            cmd.Parameters.AddWithValue("@ID", item.MenuItemID);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdateMenuItemImagePathAsync(int menuItemId, string imagePath)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand(
                "UPDATE MenuItems SET ImagePath=@ImagePath WHERE MenuItemID=@ID", conn);
            cmd.Parameters.AddWithValue("@ImagePath", imagePath);
            cmd.Parameters.AddWithValue("@ID", menuItemId);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteMenuItemAsync(int id)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand("UPDATE MenuItems SET IsAvailable=0 WHERE MenuItemID=@ID", conn);
            cmd.Parameters.AddWithValue("@ID", id);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task SetMenuItemAvailabilityAsync(int id, bool isAvailable)
        {
            using var conn = DatabaseConnection.GetConnection();
            await conn.OpenAsync();
            var cmd = new SqlCommand("UPDATE MenuItems SET IsAvailable=@Available WHERE MenuItemID=@ID", conn);
            cmd.Parameters.AddWithValue("@Available", isAvailable);
            cmd.Parameters.AddWithValue("@ID", id);
            await cmd.ExecuteNonQueryAsync();
        }

        private static MenuItemModel MapMenuItem(SqlDataReader r) => new()
        {
            MenuItemID = r.GetInt32(0),
            ItemName = r.GetString(1),
            CategoryID = r.GetInt32(2),
            CategoryName = r.GetString(3),
            Price = r.GetDecimal(4),
            Description = r.IsDBNull(5) ? null : r.GetString(5),
            ImagePath = r.IsDBNull(6) ? null : r.GetString(6),
            IsAvailable = r.GetBoolean(7)
        };
    }
}
