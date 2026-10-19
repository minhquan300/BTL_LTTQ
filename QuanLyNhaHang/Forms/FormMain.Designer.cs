namespace QuanLyNhaHang.Forms
{
    partial class FormMain
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pnlSidebar = new Panel();
            btnUsers = new Button();
            btnStats = new Button();
            btnMenu = new Button();
            btnKitchen = new Button();
            btnOrder = new Button();
            btnTables = new Button();
            btnLogout = new Button();
            pnlUser = new Panel();
            lblUser = new Label();
            lblRole = new Label();
            pnlLogo = new Panel();
            lblLogo = new Label();
            pnlTopBar = new Panel();
            lblTime = new Label();
            lblPageTitle = new Label();
            pnlContent = new Panel();
            clockTimer = new System.Windows.Forms.Timer(components);
            pnlSidebar.SuspendLayout();
            pnlUser.SuspendLayout();
            pnlLogo.SuspendLayout();
            pnlTopBar.SuspendLayout();
            SuspendLayout();
            // 
            // pnlSidebar
            // 
            pnlSidebar.BackColor = Color.FromArgb(30, 42, 56);
            pnlSidebar.Controls.Add(btnUsers);
            pnlSidebar.Controls.Add(btnStats);
            pnlSidebar.Controls.Add(btnMenu);
            pnlSidebar.Controls.Add(btnKitchen);
            pnlSidebar.Controls.Add(btnOrder);
            pnlSidebar.Controls.Add(btnTables);
            pnlSidebar.Controls.Add(btnLogout);
            pnlSidebar.Controls.Add(pnlUser);
            pnlSidebar.Controls.Add(pnlLogo);
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Location = new Point(0, 0);
            pnlSidebar.Name = "pnlSidebar";
            pnlSidebar.Size = new Size(230, 760);
            pnlSidebar.TabIndex = 0;
            // 
            // btnUsers
            // 
            btnUsers.BackColor = Color.FromArgb(30, 42, 56);
            btnUsers.Cursor = Cursors.Hand;
            btnUsers.FlatAppearance.BorderSize = 0;
            btnUsers.FlatStyle = FlatStyle.Flat;
            btnUsers.Font = new Font("Segoe UI", 10.5F);
            btnUsers.ForeColor = Color.FromArgb(189, 195, 199);
            btnUsers.Location = new Point(0, 360);
            btnUsers.Name = "btnUsers";
            btnUsers.Size = new Size(230, 48);
            btnUsers.TabIndex = 6;
            btnUsers.Text = "  👥  Nhân viên";
            btnUsers.TextAlign = ContentAlignment.MiddleLeft;
            btnUsers.UseVisualStyleBackColor = false;
            btnUsers.Click += btnUsers_Click;
            // 
            // btnStats
            // 
            btnStats.BackColor = Color.FromArgb(30, 42, 56);
            btnStats.Cursor = Cursors.Hand;
            btnStats.FlatAppearance.BorderSize = 0;
            btnStats.FlatStyle = FlatStyle.Flat;
            btnStats.Font = new Font("Segoe UI", 10.5F);
            btnStats.ForeColor = Color.FromArgb(189, 195, 199);
            btnStats.Location = new Point(0, 308);
            btnStats.Name = "btnStats";
            btnStats.Size = new Size(230, 48);
            btnStats.TabIndex = 5;
            btnStats.Text = "  📊  Thống kê";
            btnStats.TextAlign = ContentAlignment.MiddleLeft;
            btnStats.UseVisualStyleBackColor = false;
            btnStats.Click += btnStats_Click;
            // 
            // btnMenu
            // 
            btnMenu.BackColor = Color.FromArgb(30, 42, 56);
            btnMenu.Cursor = Cursors.Hand;
            btnMenu.FlatAppearance.BorderSize = 0;
            btnMenu.FlatStyle = FlatStyle.Flat;
            btnMenu.Font = new Font("Segoe UI", 10.5F);
            btnMenu.ForeColor = Color.FromArgb(189, 195, 199);
            btnMenu.Location = new Point(0, 256);
            btnMenu.Name = "btnMenu";
            btnMenu.Size = new Size(230, 48);
            btnMenu.TabIndex = 4;
            btnMenu.Text = "  🍜  Thực đơn";
            btnMenu.TextAlign = ContentAlignment.MiddleLeft;
            btnMenu.UseVisualStyleBackColor = false;
            btnMenu.Click += btnMenu_Click;
            // 
            // btnKitchen
            // 
            btnKitchen.BackColor = Color.FromArgb(30, 42, 56);
            btnKitchen.Cursor = Cursors.Hand;
            btnKitchen.FlatAppearance.BorderSize = 0;
            btnKitchen.FlatStyle = FlatStyle.Flat;
            btnKitchen.Font = new Font("Segoe UI", 10.5F);
            btnKitchen.ForeColor = Color.FromArgb(189, 195, 199);
            btnKitchen.Location = new Point(0, 204);
            btnKitchen.Name = "btnKitchen";
            btnKitchen.Size = new Size(230, 48);
            btnKitchen.TabIndex = 3;
            btnKitchen.Text = "  👨‍🍳  Bếp / Theo dõi";
            btnKitchen.TextAlign = ContentAlignment.MiddleLeft;
            btnKitchen.UseVisualStyleBackColor = false;
            btnKitchen.Click += btnKitchen_Click;
            // 
            // btnOrder
            // 
            btnOrder.BackColor = Color.FromArgb(30, 42, 56);
            btnOrder.Cursor = Cursors.Hand;
            btnOrder.FlatAppearance.BorderSize = 0;
            btnOrder.FlatStyle = FlatStyle.Flat;
            btnOrder.Font = new Font("Segoe UI", 10.5F);
            btnOrder.ForeColor = Color.FromArgb(189, 195, 199);
            btnOrder.Location = new Point(0, 152);
            btnOrder.Name = "btnOrder";
            btnOrder.Size = new Size(230, 48);
            btnOrder.TabIndex = 2;
            btnOrder.Text = "  📋  Gọi món";
            btnOrder.TextAlign = ContentAlignment.MiddleLeft;
            btnOrder.UseVisualStyleBackColor = false;
            btnOrder.Click += btnOrder_Click;
            // 
            // btnTables
            // 
            btnTables.BackColor = Color.FromArgb(30, 42, 56);
            btnTables.Cursor = Cursors.Hand;
            btnTables.FlatAppearance.BorderSize = 0;
            btnTables.FlatStyle = FlatStyle.Flat;
            btnTables.Font = new Font("Segoe UI", 10.5F);
            btnTables.ForeColor = Color.FromArgb(189, 195, 199);
            btnTables.Location = new Point(0, 100);
            btnTables.Name = "btnTables";
            btnTables.Size = new Size(230, 48);
            btnTables.TabIndex = 1;
            btnTables.Text = "  \U0001fa91  Quản lý bàn";
            btnTables.TextAlign = ContentAlignment.MiddleLeft;
            btnTables.UseVisualStyleBackColor = false;
            btnTables.Click += btnTables_Click;
            // 
            // btnLogout
            // 
            btnLogout.BackColor = Color.FromArgb(30, 42, 56);
            btnLogout.Cursor = Cursors.Hand;
            btnLogout.Dock = DockStyle.Bottom;
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.Font = new Font("Segoe UI", 10.5F);
            btnLogout.ForeColor = Color.FromArgb(231, 76, 60);
            btnLogout.Location = new Point(0, 647);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(230, 48);
            btnLogout.TabIndex = 7;
            btnLogout.Text = "  🚪  Đăng xuất";
            btnLogout.TextAlign = ContentAlignment.MiddleLeft;
            btnLogout.UseVisualStyleBackColor = false;
            btnLogout.Click += btnLogout_Click;
            // 
            // pnlUser
            // 
            pnlUser.BackColor = Color.FromArgb(26, 82, 118);
            pnlUser.Controls.Add(lblUser);
            pnlUser.Controls.Add(lblRole);
            pnlUser.Dock = DockStyle.Bottom;
            pnlUser.Location = new Point(0, 695);
            pnlUser.Name = "pnlUser";
            pnlUser.Size = new Size(230, 65);
            pnlUser.TabIndex = 8;
            // 
            // lblUser
            // 
            lblUser.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblUser.ForeColor = Color.White;
            lblUser.Location = new Point(0, 8);
            lblUser.Name = "lblUser";
            lblUser.Size = new Size(230, 30);
            lblUser.TabIndex = 0;
            lblUser.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblRole
            // 
            lblRole.Font = new Font("Segoe UI", 9F);
            lblRole.ForeColor = Color.FromArgb(174, 214, 241);
            lblRole.Location = new Point(0, 38);
            lblRole.Name = "lblRole";
            lblRole.Size = new Size(230, 22);
            lblRole.TabIndex = 1;
            lblRole.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlLogo
            // 
            pnlLogo.BackColor = Color.FromArgb(26, 82, 118);
            pnlLogo.Controls.Add(lblLogo);
            pnlLogo.Dock = DockStyle.Top;
            pnlLogo.Location = new Point(0, 0);
            pnlLogo.Name = "pnlLogo";
            pnlLogo.Size = new Size(230, 90);
            pnlLogo.TabIndex = 0;
            // 
            // lblLogo
            // 
            lblLogo.Dock = DockStyle.Fill;
            lblLogo.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblLogo.ForeColor = Color.White;
            lblLogo.Location = new Point(0, 0);
            lblLogo.Name = "lblLogo";
            lblLogo.Size = new Size(230, 90);
            lblLogo.TabIndex = 0;
            lblLogo.Text = "🍽️ NHÀ HÀNG";
            lblLogo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlTopBar
            // 
            pnlTopBar.BackColor = Color.White;
            pnlTopBar.BorderStyle = BorderStyle.FixedSingle;
            pnlTopBar.Controls.Add(lblTime);
            pnlTopBar.Controls.Add(lblPageTitle);
            pnlTopBar.Dock = DockStyle.Top;
            pnlTopBar.Location = new Point(230, 0);
            pnlTopBar.Name = "pnlTopBar";
            pnlTopBar.Size = new Size(1050, 54);
            pnlTopBar.TabIndex = 1;
            // 
            // lblTime
            // 
            lblTime.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblTime.AutoSize = true;
            lblTime.Font = new Font("Segoe UI", 10F);
            lblTime.ForeColor = Color.FromArgb(127, 140, 141);
            lblTime.Location = new Point(820, 17);
            lblTime.Name = "lblTime";
            lblTime.Size = new Size(44, 19);
            lblTime.TabIndex = 1;
            lblTime.Text = "00:00";
            // 
            // lblPageTitle
            // 
            lblPageTitle.AutoSize = true;
            lblPageTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblPageTitle.ForeColor = Color.FromArgb(26, 82, 118);
            lblPageTitle.Location = new Point(20, 14);
            lblPageTitle.Name = "lblPageTitle";
            lblPageTitle.Size = new Size(179, 25);
            lblPageTitle.TabIndex = 0;
            lblPageTitle.Text = "Quản Lý Nhà Hàng";
            // 
            // pnlContent
            // 
            pnlContent.BackColor = Color.FromArgb(245, 247, 250);
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Location = new Point(230, 54);
            pnlContent.Name = "pnlContent";
            pnlContent.Size = new Size(1050, 706);
            pnlContent.TabIndex = 2;
            // 
            // clockTimer
            // 
            clockTimer.Interval = 1000;
            clockTimer.Tick += clockTimer_Tick;
            // 
            // FormMain
            // 
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(1280, 760);
            Controls.Add(pnlContent);
            Controls.Add(pnlTopBar);
            Controls.Add(pnlSidebar);
            MinimumSize = new Size(1100, 650);
            Name = "FormMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Quản Lý Nhà Hàng";
            WindowState = FormWindowState.Maximized;
            Load += FormMain_Load;
            pnlSidebar.ResumeLayout(false);
            pnlUser.ResumeLayout(false);
            pnlLogo.ResumeLayout(false);
            pnlTopBar.ResumeLayout(false);
            pnlTopBar.PerformLayout();
            ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlSidebar;
        private System.Windows.Forms.Panel pnlLogo;
        private System.Windows.Forms.Label lblLogo;
        private System.Windows.Forms.Button btnTables;
        private System.Windows.Forms.Button btnOrder;
        private System.Windows.Forms.Button btnKitchen;
        private System.Windows.Forms.Button btnMenu;
        private System.Windows.Forms.Button btnStats;
        private System.Windows.Forms.Button btnUsers;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Panel pnlUser;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.Panel pnlTopBar;
        private System.Windows.Forms.Label lblPageTitle;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Timer clockTimer;
    }
}
