using QuanLyNhaHang.Data;
using QuanLyNhaHang.Models;
using System.Data;

namespace QuanLyNhaHang.Service
{
    public class ServiceTimeService
    {
        private readonly OrderRepository _repo = new();

        /// <summary>
        /// Lấy các món đang được xử lý trong bếp (Mới, Đang nấu, Sẵn sàng)
        /// </summary>
        public async Task<List<OrderDetail>> GetActiveCookingItemsAsync() =>
            await _repo.GetPendingKitchenItemsAsync();

        /// <summary>
        /// Lấy nhật ký chi tiết thời gian phục vụ món ăn theo khoảng thời gian
        /// </summary>
        public async Task<List<OrderDetail>> GetServiceTimeLogsAsync(DateTime from, DateTime to, string? status = null) =>
            await _repo.GetServiceTimeLogsAsync(from, to, status);

        /// <summary>
        /// Lấy các chỉ số KPI hiệu suất chế biến và phục vụ
        /// </summary>
        public async Task<(double AvgWaitMinutes, double AvgCookingMinutes, double AvgDeliveryMinutes, int TotalItems, int OverdueCount)> GetKPIsAsync(DateTime from, DateTime to) =>
            await _repo.GetCookingKPIMetricsAsync(from, to);

        /// <summary>
        /// Báo cáo thời gian chế biến trung bình theo từng món ăn
        /// </summary>
        public async Task<DataTable> GetAverageCookingTimeByMenuItemAsync(DateTime from, DateTime to) =>
            await _repo.GetAverageCookingTimeByMenuItemAsync(from, to);
    }
}
