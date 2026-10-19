using QuanLyNhaHang.Data;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Service
{
    public class KitchenService
    {
        private readonly OrderRepository _repo = new();

        public async Task<List<OrderDetail>> GetPendingItemsAsync() =>
            await _repo.GetPendingKitchenItemsAsync();

        public async Task StartCookingAsync(int detailId) =>
            await _repo.UpdateDetailStatusAsync(detailId, "Dang che bien", AuthService.Instance.CurrentUser?.EmployeeID);

        public async Task FinishCookingAsync(int detailId) =>
            await _repo.UpdateDetailStatusAsync(detailId, "San sang", AuthService.Instance.CurrentUser?.EmployeeID);

        public async Task ServeItemAsync(int detailId) =>
            await _repo.UpdateDetailStatusAsync(detailId, "Da phuc vu", AuthService.Instance.CurrentUser?.EmployeeID);

        public async Task CancelItemAsync(int detailId, string reason = "Hết nguyên liệu / Khách đổi ý") =>
            await _repo.UpdateDetailStatusAsync(detailId, "Da huy", AuthService.Instance.CurrentUser?.EmployeeID, reason);
    }
}
