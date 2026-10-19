using QuanLyNhaHang.Helpers;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Forms
{
    public class FormUserEdit : Form
    {
        public User? ResultUser { get; private set; }
        public string? NewPassword { get; private set; }

        private readonly User? _existing;
        private readonly List<(int RoleID, string RoleName)> _roles;

        private TextBox txtUsername = null!, txtFullName = null!, txtPhone = null!,
                        txtPassword = null!, txtConfirm = null!;
        private ComboBox cboRole = null!;
        private CheckBox chkActive = null!;

        public FormUserEdit(User? existing, List<(int, string)> roles)
        {
            _existing = existing;
            _roles = roles;
            BuildUI();
            if (existing != null) FillData(existing);
        }

        private void BuildUI()
        {
            this.Text = _existing == null ? "Thêm nhân viên" : "Sửa nhân viên";
            this.ClientSize = new Size(460, 480);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false; this.MinimizeBox = false;
            this.BackColor = Color.White;

            var pnlH = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = UITheme.PrimaryDark };
            new Label { Text = _existing == null ? "➕ Thêm nhân viên mới" : "✏️ Cập nhật thông tin", Font = UITheme.FontHeader, ForeColor = Color.White, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }.Also(l => pnlH.Controls.Add(l));
            this.Controls.Add(pnlH);

            int y = 65;
            void Row(string lbl, Control ctrl)
            {
                new Label { Text = lbl, Font = UITheme.FontNormal, ForeColor = UITheme.TextPrimary, AutoSize = true, Location = new Point(30, y + 4) }.Also(l => this.Controls.Add(l));
                ctrl.Location = new Point(180, y); ctrl.Size = new Size(250, 28);
                this.Controls.Add(ctrl);
                y += 44;
            }

            txtUsername = new TextBox { BorderStyle = BorderStyle.FixedSingle, Font = UITheme.FontNormal };
            if (_existing != null) txtUsername.ReadOnly = true;
            Row("Username *", txtUsername);

            txtFullName = new TextBox { BorderStyle = BorderStyle.FixedSingle, Font = UITheme.FontNormal };
            Row("Họ và tên *", txtFullName);

            txtPhone = new TextBox { BorderStyle = BorderStyle.FixedSingle, Font = UITheme.FontNormal };
            Row("Số điện thoại", txtPhone);

            cboRole = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = UITheme.FontNormal };
            foreach (var (id, name) in _roles)
                cboRole.Items.Add(new RoleItem(id, name));
            if (cboRole.Items.Count > 0) cboRole.SelectedIndex = 0;
            Row("Vai trò *", cboRole);

            if (_existing != null)
            {
                chkActive = new CheckBox { Text = "Tài khoản hoạt động", AutoSize = true, Font = UITheme.FontNormal, Checked = true };
                chkActive.Location = new Point(180, y); this.Controls.Add(chkActive); y += 44;
            }

            // Password section
            var sepLine = new Label { Text = _existing == null ? "Mật khẩu *" : "Đổi mật khẩu (để trống = giữ cũ)", BorderStyle = BorderStyle.Fixed3D, Font = UITheme.FontSmall, ForeColor = UITheme.TextMuted, AutoSize = false, Size = new Size(400, 22), Location = new Point(30, y) };
            this.Controls.Add(sepLine); y += 28;

            txtPassword = new TextBox { PasswordChar = '●', BorderStyle = BorderStyle.FixedSingle, Font = UITheme.FontNormal };
            Row("Mật khẩu", txtPassword);

            txtConfirm = new TextBox { PasswordChar = '●', BorderStyle = BorderStyle.FixedSingle, Font = UITheme.FontNormal };
            Row("Xác nhận MK", txtConfirm);

            var btnOK = UITheme.CreateButton("✔ Lưu", UITheme.Success); btnOK.Size = new Size(160, 42); btnOK.Location = new Point(30, y + 10); btnOK.Click += BtnOK_Click; this.Controls.Add(btnOK);
            var btnCancel = UITheme.CreateButton("✖ Hủy", UITheme.Danger); btnCancel.Size = new Size(160, 42); btnCancel.Location = new Point(270, y + 10); btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel; this.Controls.Add(btnCancel);
            this.ClientSize = new Size(460, y + 70);
        }

        private void FillData(User u)
        {
            txtUsername.Text = u.Username;
            txtFullName.Text = u.FullName;
            txtPhone.Text = u.Phone ?? "";
            if (chkActive != null) chkActive.Checked = u.IsActive;
            for (int i = 0; i < cboRole.Items.Count; i++)
                if (cboRole.Items[i] is RoleItem r && r.Id == u.RoleID)
                { cboRole.SelectedIndex = i; break; }
        }

        private void BtnOK_Click(object? s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text)) { ValidationHelper.ShowError("Username không được để trống!"); return; }
            if (string.IsNullOrWhiteSpace(txtFullName.Text)) { ValidationHelper.ShowError("Họ tên không được để trống!"); return; }
            if (_existing == null && string.IsNullOrEmpty(txtPassword.Text)) { ValidationHelper.ShowError("Mật khẩu không được để trống!"); return; }
            if (!string.IsNullOrEmpty(txtPassword.Text) && txtPassword.Text != txtConfirm.Text) { ValidationHelper.ShowError("Xác nhận mật khẩu không khớp!"); return; }

            if (!string.IsNullOrEmpty(txtPassword.Text)) NewPassword = txtPassword.Text;

            var role = cboRole.SelectedItem as RoleItem;
            ResultUser = new User
            {
                EmployeeID = _existing?.EmployeeID ?? 0,
                Username = txtUsername.Text.Trim(),
                FullName = txtFullName.Text.Trim(),
                Phone = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text.Trim(),
                RoleID = role?.Id ?? 1,
                IsActive = _existing == null ? true : (chkActive?.Checked ?? true)
            };
            this.DialogResult = DialogResult.OK;
        }

        private record RoleItem(int Id, string Name) { public override string ToString() => Name; }
    }
}
