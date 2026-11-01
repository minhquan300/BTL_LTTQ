using QuanLyNhaHang.Helpers;
using QuanLyNhaHang.Service;

namespace QuanLyNhaHang.Forms
{
    public partial class FormMain : Form
    {
        private Form? _currentChild;
        private bool _sidebarCollapsed;
        private string _userFullName = string.Empty;
        private string _userRoleName = string.Empty;
        private Button _sidebarToggleButton = null!;
        private ToolTip _sidebarToolTip = null!;

        public FormMain()
        {
            UITheme.ConfigureDpi(this);
            InitializeComponent();

            _sidebarToolTip = new ToolTip(components);
            _sidebarToggleButton = new Button
            {
                Name = "btnSidebarToggle",
                Text = "☰",
                Location = new Point(10, 8),
                Size = new Size(40, 38),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = UITheme.Navy,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                TabStop = true,
                AccessibleName = "Thu gọn thanh điều hướng"
            };
            _sidebarToggleButton.FlatAppearance.BorderSize = 0;
            _sidebarToggleButton.Click += (_, _) => SetSidebarCollapsed(!_sidebarCollapsed);
            _sidebarToolTip.SetToolTip(_sidebarToggleButton, "Thu gọn thanh điều hướng");
            pnlTopBar.Controls.Add(_sidebarToggleButton);
            _sidebarToggleButton.BringToFront();
            lblPageTitle.Location = new Point(60, 14);

            pnlContent.BackColor = UITheme.Light;
            pnlSidebar.BackColor = UITheme.SidebarBg;
            pnlLogo.BackColor = UITheme.Navy;
            pnlUser.BackColor = UITheme.Navy;
            foreach (var button in new[] { btnTables, btnOrder, btnKitchen, btnMenu, btnStats, btnUsers, btnLogout })
            {
                button.BackColor = UITheme.SidebarBg;
                button.FlatAppearance.MouseOverBackColor = UITheme.SidebarHover;
            }
            SetSidebarCollapsed(false);
            UITheme.EnableDoubleBuffering(this);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            var user = AuthService.Instance.CurrentUser;
            if (user != null)
            {
                _userFullName = user.FullName;
                _userRoleName = user.RoleName;
                UpdateSidebarUserInfo();
            }

            clockTimer.Start();
            lblTime.Text = "🕐 " + DateTime.Now.ToString("HH:mm:ss  dd/MM/yyyy");

            // Show table management by default
            ShowForm(new FormTable());

            // Hide admin menus for non-admin
            if (!AuthService.Instance.IsAdmin)
            {
                btnUsers.Visible = false;
                btnMenu.Visible = false;
                btnStats.Visible = false;
            }
        }

        private void clockTimer_Tick(object sender, EventArgs e)
        {
            lblTime.Text = "🕐 " + DateTime.Now.ToString("HH:mm:ss  dd/MM/yyyy");
        }

        private void ShowForm(Form form)
        {
            _currentChild?.Close();
            _currentChild = form;
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            pnlContent.Controls.Clear();
            pnlContent.Controls.Add(form);
            form.Show();
            UpdateNavHighlight(form.GetType());
            lblPageTitle.Text = form.GetType() switch
            {
                var type when type == typeof(FormTable) => "Quản lý bàn ăn",
                var type when type == typeof(FormOrder) => "Gọi món",
                var type when type == typeof(FormKitchen) => "Bếp / Theo dõi",
                var type when type == typeof(FormOrderDetail) => "Quản lý thực đơn",
                var type when type == typeof(FormStatistics) => "Thống kê",
                var type when type == typeof(FormUserManagement) => "Quản lý nhân viên",
                _ => "Quản Lý Nhà Hàng"
            };
        }

        public void NavigateTo(Form form) => ShowForm(form);

        private void SetSidebarCollapsed(bool collapsed)
        {
            _sidebarCollapsed = collapsed;
            int width = collapsed ? 68 : 230;
            pnlSidebar.Width = width;

            SetNavigationButton(btnTables, "🪑", "  🪑  Quản lý bàn", "Quản lý bàn", width, collapsed);
            SetNavigationButton(btnOrder, "📋", "  📋  Gọi món", "Gọi món", width, collapsed);
            SetNavigationButton(btnKitchen, "👨‍🍳", "  👨‍🍳  Bếp / Theo dõi", "Bếp / Theo dõi", width, collapsed);
            SetNavigationButton(btnMenu, "🍜", "  🍜  Thực đơn", "Thực đơn", width, collapsed);
            SetNavigationButton(btnStats, "📊", "  📊  Thống kê", "Thống kê", width, collapsed);
            SetNavigationButton(btnUsers, "👥", "  👥  Nhân viên", "Nhân viên", width, collapsed);

            btnLogout.Text = collapsed ? "🚪" : "  🚪  Đăng xuất";
            btnLogout.TextAlign = collapsed ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft;
            _sidebarToolTip.SetToolTip(btnLogout, collapsed ? "Đăng xuất" : string.Empty);
            lblLogo.Text = collapsed ? "🍽️" : "🍽️ NHÀ HÀNG";
            lblUser.Width = width;
            lblRole.Width = width;
            UpdateSidebarUserInfo();

            _sidebarToggleButton.AccessibleName = collapsed
                ? "Mở rộng thanh điều hướng"
                : "Thu gọn thanh điều hướng";
            _sidebarToolTip.SetToolTip(_sidebarToggleButton, _sidebarToggleButton.AccessibleName);
        }

        private void SetNavigationButton(Button button, string icon, string fullText, string description,
            int sidebarWidth, bool collapsed)
        {
            button.Width = sidebarWidth;
            button.Text = collapsed ? icon : fullText;
            button.TextAlign = collapsed ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft;
            _sidebarToolTip.SetToolTip(button, collapsed ? description : string.Empty);
        }

        private void UpdateSidebarUserInfo()
        {
            if (lblUser == null || lblRole == null) return;
            lblUser.Text = _sidebarCollapsed ? "👤" : $"👤 {_userFullName}";
            lblRole.Text = _sidebarCollapsed ? string.Empty : _userRoleName;
            _sidebarToolTip?.SetToolTip(lblUser, _sidebarCollapsed ? _userFullName : string.Empty);
            _sidebarToolTip?.SetToolTip(lblRole, _sidebarCollapsed ? _userRoleName : string.Empty);
        }

        private void UpdateNavHighlight(Type formType)
        {
            Button[] navBtns = { btnTables, btnOrder, btnKitchen, btnMenu, btnStats, btnUsers };
            Type[] formTypes = { typeof(FormTable), typeof(FormOrder), typeof(FormKitchen),
                                 typeof(FormOrderDetail), typeof(FormStatistics), typeof(FormUserManagement) };

            for (int i = 0; i < navBtns.Length; i++)
            {
                navBtns[i].BackColor = formType == formTypes[i]
                    ? UITheme.Primary
                    : UITheme.SidebarBg;
            }
        }

        private void btnTables_Click(object? s, EventArgs e) => ShowForm(new FormTable());
        private void btnOrder_Click(object? s, EventArgs e) => ShowForm(new FormOrder());
        private void btnKitchen_Click(object? s, EventArgs e) => ShowForm(new FormKitchen());
        private void btnMenu_Click(object? s, EventArgs e) => ShowForm(new FormOrderDetail());
        private void btnStats_Click(object? s, EventArgs e) => ShowForm(new FormStatistics());
        private void btnUsers_Click(object? s, EventArgs e) => ShowForm(new FormUserManagement());

        private void btnLogout_Click(object? s, EventArgs e)
        {
            if (!ValidationHelper.Confirm("Bạn có chắc muốn đăng xuất?")) return;
            clockTimer.Stop();
            AuthService.Instance.Logout();
            _currentChild?.Close();
            var login = new FormLogin();
            login.Show();
            this.Close();
        }
    }
}
