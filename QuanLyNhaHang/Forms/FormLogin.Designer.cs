using QuanLyNhaHang.Helpers;
using QuanLyNhaHang.Controls;

namespace QuanLyNhaHang.Forms
{
    partial class FormLogin
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
            pnlLeft = new BackgroundPanel();
            lblBrand = new Label();
            lblRestaurant = new ShadowLabel();
            lblSub = new ShadowLabel();
            pnlRight = new Panel();
            lblTitle = new Label();
            lblWelcome = new Label();
            lblUser = new Label();
            txtUsername = new TextBox();
            lblPass = new Label();
            txtPassword = new TextBox();
            chkShowPassword = new CheckBox();
            lblError = new Label();
            btnLogin = new Button();
            lblVersion = new Label();
            pnlTitleBar = new Panel();
            lblWindowTitle = new Label();
            btnMinimize = new Button();
            btnClose = new Button();
            pnlLeft.SuspendLayout();
            pnlRight.SuspendLayout();
            pnlTitleBar.SuspendLayout();
            SuspendLayout();
            // 
            // pnlLeft
            // 
            pnlLeft.BackColor = Color.FromArgb(30, 58, 95);
            pnlLeft.Controls.Add(lblBrand);
            pnlLeft.Controls.Add(lblRestaurant);
            pnlLeft.Controls.Add(lblSub);
            pnlLeft.Dock = DockStyle.Left;
            pnlLeft.Location = new Point(0, 0);
            pnlLeft.Margin = new Padding(0);
            pnlLeft.Name = "pnlLeft";
            pnlLeft.Size = new Size(375, 580);
            pnlLeft.TabIndex = 0;
            // 
            // lblBrand
            // 
            lblBrand.Font = new Font("Segoe UI Emoji", 42F);
            lblBrand.ForeColor = Color.White;
            lblBrand.Location = new Point(0, 45);
            lblBrand.Name = "lblBrand";
            lblBrand.Size = new Size(378, 70);
            lblBrand.TabIndex = 0;
            lblBrand.Text = "🍽️";
            lblBrand.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblRestaurant
            // 
            lblRestaurant.BackColor = Color.Transparent;
            lblRestaurant.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblRestaurant.ForeColor = Color.White;
            lblRestaurant.Location = new Point(0, 120);
            lblRestaurant.Name = "lblRestaurant";
            lblRestaurant.Size = new Size(378, 35);
            lblRestaurant.TabIndex = 1;
            lblRestaurant.Text = "QUẢN LÝ NHÀ HÀNG";
            lblRestaurant.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblSub
            // 
            lblSub.BackColor = Color.Transparent;
            lblSub.Font = new Font("Segoe UI", 9.5F);
            lblSub.ForeColor = Color.FromArgb(203, 213, 225);
            lblSub.Location = new Point(0, 160);
            lblSub.Name = "lblSub";
            lblSub.Size = new Size(380, 42);
            lblSub.TabIndex = 2;
            lblSub.Text = "Hệ thống quản lý đặt bàn, gọi món,\r\nbếp và thanh toán chuyên nghiệp";
            lblSub.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlRight
            // 
            pnlRight.BackColor = Color.White;
            pnlRight.Controls.Add(lblTitle);
            pnlRight.Controls.Add(lblWelcome);
            pnlRight.Controls.Add(lblUser);
            pnlRight.Controls.Add(txtUsername);
            pnlRight.Controls.Add(lblPass);
            pnlRight.Controls.Add(txtPassword);
            pnlRight.Controls.Add(chkShowPassword);
            pnlRight.Controls.Add(lblError);
            pnlRight.Controls.Add(btnLogin);
            pnlRight.Controls.Add(lblVersion);
            pnlRight.Dock = DockStyle.Fill;
            pnlRight.Location = new Point(375, 0);
            pnlRight.Name = "pnlRight";
            pnlRight.Size = new Size(525, 580);
            pnlRight.TabIndex = 1;
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(26, 82, 118);
            lblTitle.Location = new Point(50, 45);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(400, 45);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Đăng Nhập";
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblWelcome
            // 
            lblWelcome.Font = new Font("Segoe UI", 10F);
            lblWelcome.ForeColor = Color.FromArgb(127, 140, 141);
            lblWelcome.Location = new Point(50, 95);
            lblWelcome.Name = "lblWelcome";
            lblWelcome.Size = new Size(400, 24);
            lblWelcome.TabIndex = 1;
            lblWelcome.Text = "Chào mừng bạn trở lại! Vui lòng đăng nhập hệ thống.";
            // 
            // lblUser
            // 
            lblUser.AutoSize = true;
            lblUser.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblUser.ForeColor = Color.FromArgb(44, 62, 80);
            lblUser.Location = new Point(50, 140);
            lblUser.Name = "lblUser";
            lblUser.Size = new Size(175, 17);
            lblUser.TabIndex = 2;
            lblUser.Text = "Tên đăng nhập / Username";
            // 
            // txtUsername
            // 
            txtUsername.BorderStyle = BorderStyle.FixedSingle;
            txtUsername.Font = new Font("Segoe UI", 11F);
            txtUsername.Location = new Point(50, 165);
            txtUsername.Name = "txtUsername";
            txtUsername.PlaceholderText = "Nhập tài khoản (vd: admin)";
            txtUsername.Size = new Size(400, 27);
            txtUsername.TabIndex = 3;
            txtUsername.KeyDown += txtUsername_KeyDown;
            // 
            // lblPass
            // 
            lblPass.AutoSize = true;
            lblPass.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblPass.ForeColor = Color.FromArgb(44, 62, 80);
            lblPass.Location = new Point(50, 215);
            lblPass.Name = "lblPass";
            lblPass.Size = new Size(66, 17);
            lblPass.TabIndex = 4;
            lblPass.Text = "Mật khẩu";
            // 
            // txtPassword
            // 
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.Font = new Font("Segoe UI", 11F);
            txtPassword.Location = new Point(50, 240);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '●';
            txtPassword.PlaceholderText = "Nhập mật khẩu";
            txtPassword.Size = new Size(400, 27);
            txtPassword.TabIndex = 5;
            txtPassword.KeyDown += txtPassword_KeyDown;
            // 
            // chkShowPassword
            // 
            chkShowPassword.AutoSize = true;
            chkShowPassword.Cursor = Cursors.Hand;
            chkShowPassword.Font = new Font("Segoe UI", 9F);
            chkShowPassword.ForeColor = Color.FromArgb(127, 140, 141);
            chkShowPassword.Location = new Point(50, 280);
            chkShowPassword.Name = "chkShowPassword";
            chkShowPassword.Size = new Size(104, 19);
            chkShowPassword.TabIndex = 6;
            chkShowPassword.Text = "Hiện mật khẩu";
            chkShowPassword.UseVisualStyleBackColor = true;
            chkShowPassword.CheckedChanged += chkShowPassword_CheckedChanged;
            // 
            // lblError
            // 
            lblError.Font = new Font("Segoe UI", 10F);
            lblError.ForeColor = Color.FromArgb(231, 76, 60);
            lblError.Location = new Point(50, 310);
            lblError.Name = "lblError";
            lblError.Size = new Size(400, 26);
            lblError.TabIndex = 7;
            lblError.Visible = false;
            // 
            // btnLogin
            // 
            btnLogin.BackColor = Color.FromArgb(41, 128, 185);
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Segoe UI", 11.5F, FontStyle.Bold);
            btnLogin.ForeColor = Color.White;
            btnLogin.Location = new Point(50, 345);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(400, 46);
            btnLogin.TabIndex = 8;
            btnLogin.Text = "ĐĂNG NHẬP";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += btnLogin_Click;
            // 
            // lblVersion
            // 
            lblVersion.Font = new Font("Segoe UI", 9F);
            lblVersion.ForeColor = Color.FromArgb(127, 140, 141);
            lblVersion.Location = new Point(50, 475);
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new Size(400, 20);
            lblVersion.TabIndex = 9;
            lblVersion.Text = "Phiên bản 1.0 • Quản Lý Nhà Hàng © 2026";
            lblVersion.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlTitleBar
            // 
            pnlTitleBar.BackColor = Color.FromArgb(30, 58, 95);
            pnlTitleBar.Controls.Add(lblWindowTitle);
            pnlTitleBar.Controls.Add(btnMinimize);
            pnlTitleBar.Controls.Add(btnClose);
            pnlTitleBar.Dock = DockStyle.Top;
            pnlTitleBar.Location = new Point(375, 0);
            pnlTitleBar.Name = "pnlTitleBar";
            pnlTitleBar.Size = new Size(525, 32);
            pnlTitleBar.TabIndex = 2;
            // 
            // lblWindowTitle
            // 
            lblWindowTitle.Dock = DockStyle.Left;
            lblWindowTitle.Font = new Font("Segoe UI", 9F);
            lblWindowTitle.ForeColor = Color.White;
            lblWindowTitle.Location = new Point(0, 0);
            lblWindowTitle.Name = "lblWindowTitle";
            lblWindowTitle.Padding = new Padding(12, 0, 0, 0);
            lblWindowTitle.Size = new Size(240, 32);
            lblWindowTitle.TabIndex = 0;
            lblWindowTitle.Text = "Quản lý nhà hàng";
            lblWindowTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblWindowTitle.MouseDown += WindowDrag_MouseDown;
            // 
            // btnMinimize
            // 
            btnMinimize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMinimize.BackColor = Color.FromArgb(30, 58, 95);
            btnMinimize.FlatAppearance.BorderSize = 0;
            btnMinimize.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 72, 110);
            btnMinimize.FlatStyle = FlatStyle.Flat;
            btnMinimize.Font = new Font("Segoe UI", 12F);
            btnMinimize.ForeColor = Color.White;
            btnMinimize.Location = new Point(453, 0);
            btnMinimize.Name = "btnMinimize";
            btnMinimize.Size = new Size(36, 32);
            btnMinimize.TabIndex = 10;
            btnMinimize.Text = "−";
            btnMinimize.UseVisualStyleBackColor = false;
            btnMinimize.Click += btnMinimize_Click;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.BackColor = Color.FromArgb(30, 58, 95);
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 38, 38);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Segoe UI", 12F);
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(489, 0);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(36, 32);
            btnClose.TabIndex = 11;
            btnClose.Text = "×";
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += btnClose_Click;
            // 
            // FormLogin
            // 
            BackColor = Color.White;
            ClientSize = new Size(900, 580);
            Controls.Add(pnlTitleBar);
            Controls.Add(pnlRight);
            Controls.Add(pnlLeft);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MaximumSize = new Size(900, 580);
            MinimizeBox = false;
            MinimumSize = new Size(900, 580);
            Name = "FormLogin";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Đăng nhập - Quản Lý Nhà Hàng";
            Load += FormLogin_Load;
            pnlLeft.ResumeLayout(false);
            pnlRight.ResumeLayout(false);
            pnlRight.PerformLayout();
            pnlTitleBar.ResumeLayout(false);
            ResumeLayout(false);

        }

        private QuanLyNhaHang.Controls.BackgroundPanel pnlLeft;
        private System.Windows.Forms.Label lblBrand;
        private QuanLyNhaHang.Controls.ShadowLabel lblRestaurant;
        private QuanLyNhaHang.Controls.ShadowLabel lblSub;
        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblPass;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.CheckBox chkShowPassword;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Panel pnlTitleBar;
        private System.Windows.Forms.Label lblWindowTitle;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Button btnClose;
    }
}
