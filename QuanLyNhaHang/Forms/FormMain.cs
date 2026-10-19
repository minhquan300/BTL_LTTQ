using QuanLyNhaHang.Helpers;
using QuanLyNhaHang.Service;

namespace QuanLyNhaHang.Forms
{
    public partial class FormMain : Form
    {
        private Form? _currentChild;

        public FormMain()
        {
            UITheme.ConfigureDpi(this);
            InitializeComponent();
            pnlContent.BackColor = UITheme.Light;
            pnlSidebar.BackColor = UITheme.SidebarBg;
            pnlLogo.BackColor = UITheme.Navy;
            pnlUser.BackColor = UITheme.Navy;
            foreach (var button in new[] { btnTables, btnOrder, btnKitchen, btnMenu, btnStats, btnUsers, btnLogout })
            {
                button.BackColor = UITheme.SidebarBg;
                button.FlatAppearance.MouseOverBackColor = UITheme.SidebarHover;
            }
            UITheme.EnableDoubleBuffering(this);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            var user = AuthService.Instance.CurrentUser;
            if (user != null)
            {
                lblUser.Text = $"👤 {user.FullName}";
                lblRole.Text = user.RoleName;
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
