using QuanLyNhaHang.Data;
using QuanLyNhaHang.Helpers;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Service;

namespace QuanLyNhaHang.Forms
{
    public partial class FormUserManagement : Form
    {
        private readonly UserRepository _repo = new();
        private List<User> _users = new();
        private List<(int RoleID, string RoleName)> _roles = new();

        public FormUserManagement()
        {
            InitializeComponent(); 
        }
        private async void FormUserManagement_Load(object sender, EventArgs e)
        {
            if (!AuthService.Instance.IsAdmin)
            {
                ValidationHelper.ShowError("Bạn không có quyền truy cập chức năng này!");
                return;
            }
            _roles = await _repo.GetRolesAsync();
            await LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            _users = await _repo.GetAllAsync();
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            dgvUsers.Rows.Clear();
            var q = txtSearch.Text.Trim().ToLower();
            var filtered = string.IsNullOrEmpty(q)
                ? _users
                : _users.Where(u => u.FullName.ToLower().Contains(q) || u.Username.ToLower().Contains(q)).ToList();

            foreach (var u in filtered)
            {
                var idx = dgvUsers.Rows.Add(u.EmployeeID, u.Username, u.FullName, u.RoleName, u.Phone ?? "", u.IsActive ? "✓ Hoạt động" : "✗ Vô hiệu", u.CreatedAt.ToString("dd/MM/yyyy"));
                dgvUsers.Rows[idx].Tag = u;
                if (!u.IsActive) dgvUsers.Rows[idx].DefaultCellStyle.ForeColor = Color.Gray;
            }
            lblCount.Text = $"Tổng: {filtered.Count} nhân viên";
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            using var dlg = new FormUserEdit(null, _roles);
            if (dlg.ShowDialog() == DialogResult.OK && dlg.ResultUser != null && dlg.NewPassword != null)
            {
                dlg.ResultUser.PasswordHash = AuthService.HashPassword(dlg.NewPassword);
                await _repo.CreateAsync(dlg.ResultUser);
                await LoadUsersAsync();
                ValidationHelper.ShowSuccess("Thêm nhân viên thành công!");
            }
        }

        private async void btnEdit_Click(object sender, EventArgs e)
        {
            if (GetSelected() is not User u) return;
            using var dlg = new FormUserEdit(u, _roles);
            if (dlg.ShowDialog() == DialogResult.OK && dlg.ResultUser != null)
            {
                await _repo.UpdateAsync(dlg.ResultUser);
                if (!string.IsNullOrEmpty(dlg.NewPassword))
                    await _repo.UpdatePasswordAsync(dlg.ResultUser.EmployeeID, AuthService.HashPassword(dlg.NewPassword));
                await LoadUsersAsync();
                ValidationHelper.ShowSuccess("Cập nhật thành công!");
            }
        }

        private async void btnToggle_Click(object sender, EventArgs e)
        {
            if (GetSelected() is not User u) return;
            if (u.EmployeeID == AuthService.Instance.CurrentUser!.EmployeeID)
            { ValidationHelper.ShowError("Không thể vô hiệu hóa tài khoản đang đăng nhập!"); return; }
            u.IsActive = !u.IsActive;
            await _repo.UpdateAsync(u);
            await LoadUsersAsync();
            ValidationHelper.ShowSuccess(u.IsActive ? "Đã kích hoạt tài khoản!" : "Đã vô hiệu hóa tài khoản!");
        }

        private User? GetSelected() =>
            dgvUsers.SelectedRows.Count > 0 ? dgvUsers.SelectedRows[0].Tag as User : null;

        private void txtSearch_TextChanged(object sender, EventArgs e) => RefreshGrid();
    }
}
