using QuanLyNhaHang.Data;
using System.Data;

namespace QuanLyNhaHang.Service
{
    public class StatisticsService
    {
        private readonly OrderRepository _repo = new();

        /// <summary>
        /// Doanh thu hôm nay
        /// </summary>
        public async Task<decimal> GetRevenueTodayAsync() =>
            await _repo.GetRevenueTodayAsync();

        /// <summary>
        /// Báo cáo doanh thu theo dải ngày
        /// </summary>
        public async Task<DataTable> GetRevenueReportAsync(DateTime from, DateTime to) =>
            await _repo.GetRevenueByDateAsync(from, to);

        /// <summary>
        /// Top món ăn bán chạy nhất
        /// </summary>
        public async Task<DataTable> GetTopSellingItemsAsync(int top = 10, DateTime? from = null, DateTime? to = null) =>
            await _repo.GetTopSellingItemsAsync(top, from, to);

        /// <summary>
        /// Phân tích doanh thu theo phương thức thanh toán
        /// </summary>
        public async Task<DataTable> GetPaymentMethodStatsAsync(DateTime from, DateTime to) =>
            await _repo.GetPaymentMethodStatsAsync(from, to);
    }
}
