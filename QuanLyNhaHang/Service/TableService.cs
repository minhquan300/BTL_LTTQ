using QuanLyNhaHang.Data;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Service
{
    public class TableService
    {
        private readonly TableRepository _repo = new();

        public async Task<List<Table>> GetAllTablesAsync() => await _repo.GetAllAsync();

        public async Task<(bool Ok, string Msg)> AddTableAsync(string name, int capacity)
        {
            if (string.IsNullOrWhiteSpace(name)) return (false, "Tên bàn không được để trống!");
            if (capacity <= 0) return (false, "Sức chứa phải lớn hơn 0!");
            await _repo.CreateAsync(new Table { TableName = name.Trim(), Capacity = capacity, TableStatus = "Trong" });
            return (true, "Thêm bàn thành công!");
        }

        public async Task<(bool Ok, string Msg)> UpdateTableAsync(Table table)
        {
            if (string.IsNullOrWhiteSpace(table.TableName)) return (false, "Tên bàn không được để trống!");
            if (table.Capacity <= 0) return (false, "Sức chứa phải lớn hơn 0!");
            await _repo.UpdateAsync(table);
            return (true, "Cập nhật bàn thành công!");
        }

        public async Task<(bool Ok, string Msg)> DeleteTableAsync(int tableId)
        {
            try
            {
                await _repo.DeleteAsync(tableId);
                return (true, "Xóa bàn thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Không thể xóa bàn: {ex.Message}");
            }
        }

        public async Task UpdateStatusAsync(int tableId, string status) =>
            await _repo.UpdateStatusAsync(tableId, status);
    }
}
