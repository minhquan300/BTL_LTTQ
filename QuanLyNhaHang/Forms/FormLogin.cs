using QuanLyNhaHang.Helpers;
using QuanLyNhaHang.Service;
using QuanLyNhaHang.Controls;
using System.IO;
using System.Runtime.InteropServices;

namespace QuanLyNhaHang.Forms
{
    public partial class FormLogin : Form
    {
        private const int WmNcLButtonDown = 0x00A1;
        private static readonly IntPtr HtCaption = new(2);
        private Panel pnlUsernameInput = null!, pnlPasswordInput = null!, pnlGeneralError = null!;
        private Label lblUsernameError = null!, lblPasswordError = null!, lblGeneralErrorIcon = null!;
        private CheckBox chkRememberUsername = null!;
        private LinkLabel lnkForgotPassword = null!;
        private Button btnTogglePassword = null!;
        private bool _usernameInvalid, _passwordInvalid;
        private Label lblCapsLock = null!, lblSpinner = null!;
        private PictureBox? _brandLogo;
        private ShadowLabel? _leftFooter;
        private TableLayoutPanel? _leftLayout, _brandStack;
        private readonly System.Windows.Forms.Timer _spinnerTimer = new() { Interval = 90 };
        private readonly System.Windows.Forms.Timer _lockoutTimer = new() { Interval = 1000 };
        private int _spinnerFrame, _failedLoginAttempts;
        private DateTime? _lockoutUntil;
        private bool _suppressInputFeedback;

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        public FormLogin()
        {
            UITheme.ConfigureDpi(this);
            InitializeComponent();
            ConfigureLeftBackground();
            UITheme.EnableDoubleBuffering(this);
            pnlTitleBar.MouseDown += WindowDrag_MouseDown;
            KeyPreview = true;
            AcceptButton = btnLogin;
            KeyDown += FormLogin_KeyDown;
            FormClosed += (_, _) => { _spinnerTimer.Stop(); _lockoutTimer.Stop(); };
            _spinnerTimer.Tick += (_, _) =>
            {
                string[] frames = { "◴", "◷", "◶", "◵" };
                lblSpinner.Text = frames[_spinnerFrame++ % frames.Length];
            };
            _lockoutTimer.Tick += LockoutTimer_Tick;
            ConfigureLoginPanel();
            lblBrand.BackColor = Color.Transparent;
            lblRestaurant.BackColor = Color.Transparent;
            lblSub.BackColor = Color.Transparent;
            ConfigureLeftPanelLayout();
            DpiChanged += (_, _) =>
            {
                if (IsHandleCreated)
                    BeginInvoke(new Action(ApplyLeftPanelDpiScale));
            };
            UITheme.ApplyRoundedCorners(this, UITheme.CornerRadius);
            FormClosed += (_, _) =>
            {
                _brandLogo?.Image?.Dispose();
            };
        }

        private void ConfigureLeftBackground()
        {
            string imagePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Images",
                "restaurant.PNG"
            );

            if (!File.Exists(imagePath))
            {
                MessageBox.Show(
                    "Không tìm thấy ảnh:\n" + imagePath
                );
                return;
            }

            using (Image img = Image.FromFile(imagePath))
            {
                pnlLeft.BackgroundImage = new Bitmap(img);
            }
            pnlLeft.BackgroundImageLayout = ImageLayout.Stretch;

            pnlLeft.Paint -= PnlLeft_Paint;
            pnlLeft.Paint += PnlLeft_Paint;
        }

        private void PnlLeft_Paint(object? sender, PaintEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(
                Color.FromArgb(150, 12, 43, 68)))
            {
                e.Graphics.FillRectangle(
                    brush,
                    pnlLeft.ClientRectangle
                );
            }
        }


        private void ConfigureLeftPanelLayout()
        {
            pnlLeft.SuspendLayout();
            pnlLeft.Controls.Clear();
            pnlLeft.Padding = Padding.Empty;

            var layout = _leftLayout = new TableLayoutPanel
            {
                Name = "tlpLeftLayout",
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(ScaleLeft(32), 0, ScaleLeft(32), 0),
                BackColor = Color.Transparent,
                Margin = Padding.Empty,
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, ScaleLeft(40)));
            pnlLeft.Controls.Add(layout);

            // Keep the existing designer control name for compatibility, while the logo is now a 72px PictureBox.
            lblBrand.Visible = false;
            ConfigureLeftLabel(lblRestaurant, "QUẢN LÝ NHÀ HÀNG", 20f, FontStyle.Bold, Color.White);
            ConfigureLeftLabel(lblSub, "Hệ thống quản lý đặt bàn, gọi món,\nbếp và thanh toán chuyên nghiệp", 10f,
                FontStyle.Regular, Color.FromArgb(204, 255, 255, 255));

            var brandStack = _brandStack = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 7,
                BackColor = Color.Transparent,
                Margin = Padding.Empty,
            };
            brandStack.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            brandStack.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            brandStack.RowStyles.Add(new RowStyle(SizeType.Absolute, ScaleLeft(72)));
            brandStack.RowStyles.Add(new RowStyle(SizeType.Absolute, ScaleLeft(12)));
            brandStack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            brandStack.RowStyles.Add(new RowStyle(SizeType.Absolute, ScaleLeft(6)));
            brandStack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            brandStack.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            brandStack.Controls.Add(CreateBrandLogo(), 0, 1);
            brandStack.Controls.Add(lblRestaurant, 0, 3);
            brandStack.Controls.Add(lblSub, 0, 5);
            layout.Controls.Add(brandStack, 0, 0);

            _leftFooter = new ShadowLabel
            {
                Name = "lblLeftFooter",
                Text = "QUẢN LÝ NHÀ HÀNG  •  2026",
                AutoSize = true,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(204, 255, 255, 255),
                Font = new Font(UITheme.FontFamily, 8.5f, FontStyle.Regular),
                Anchor = AnchorStyles.None,
                Margin = Padding.Empty,
            };
            layout.Controls.Add(_leftFooter, 0, 1);
            pnlLeft.ResumeLayout(true);
        }

        private int ScaleLeft(int logicalPixels) =>
            Math.Max(1, (int)Math.Round(logicalPixels * pnlLeft.DeviceDpi / 96f));

        private void ApplyLeftPanelDpiScale()
        {
            if (_leftLayout == null || _brandStack == null) return;
            _leftLayout.SuspendLayout();
            _leftLayout.Padding = new Padding(ScaleLeft(32), 0, ScaleLeft(32), 0);
            _leftLayout.RowStyles[1].Height = ScaleLeft(40);
            lblRestaurant.MaximumSize = new Size(ScaleLeft(314), 0);
            lblSub.MaximumSize = new Size(ScaleLeft(314), 0);
            _brandStack.RowStyles[1].Height = ScaleLeft(72);
            _brandStack.RowStyles[2].Height = ScaleLeft(12);
            _brandStack.RowStyles[4].Height = ScaleLeft(6);
            if (_brandLogo != null)
            {
                _brandLogo.Size = new Size(ScaleLeft(72), ScaleLeft(72));
                _brandLogo.Margin = Padding.Empty;
            }
            _leftLayout.ResumeLayout(true);
            _brandLogo?.Invalidate();
            pnlLeft.Invalidate();
        }

        private void ConfigureLeftLabel(Label label, string text, float size, FontStyle style, Color color)
        {
            label.Text = text;
            label.AutoSize = true;
            label.MaximumSize = new Size(ScaleLeft(314), 0);
            label.BackColor = Color.Transparent;
            label.ForeColor = color;
            label.Font = new Font(UITheme.FontFamily, size, style);
            label.Anchor = AnchorStyles.None;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Margin = Padding.Empty;
        }

        private PictureBox CreateBrandLogo()
        {
            var logoSize = ScaleLeft(72);
            var picture = new PictureBox
            {
                Name = "picBrandLogo",
                Width = logoSize,
                Height = logoSize,
                SizeMode = PictureBoxSizeMode.CenterImage,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.None,
                Margin = Padding.Empty,
                Image = CreateRestaurantLogoImage(),
            };
            _brandLogo = picture;
            return picture;
        }

        private static Image CreateRestaurantLogoImage()
        {
            var image = new Bitmap(72, 72);
            using var graphics = Graphics.FromImage(image);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.Clear(Color.Transparent);
            using var badgeShadow = new SolidBrush(Color.FromArgb(24, Color.Black));
            using var badgeFill = new SolidBrush(Color.FromArgb(35, Color.White));
            using var badgeBorder = new Pen(Color.FromArgb(120, Color.White), 1.4f);
            graphics.FillEllipse(badgeShadow, 2, 3, 68, 68);
            graphics.FillEllipse(badgeFill, 2, 2, 68, 68);
            graphics.DrawEllipse(badgeBorder, 2.7f, 2.7f, 66.6f, 66.6f);
            using var plate = new Pen(Color.FromArgb(238, 255, 255, 255), 3.2f);
            using var cutlery = new Pen(Color.FromArgb(238, 255, 255, 255), 2.8f)
            {
                StartCap = System.Drawing.Drawing2D.LineCap.Round,
                EndCap = System.Drawing.Drawing2D.LineCap.Round,
            };
            graphics.DrawEllipse(plate, 20, 20, 32, 32);
            graphics.DrawEllipse(plate, 25, 25, 22, 22);
            graphics.DrawLine(cutlery, 11, 20, 11, 52);
            graphics.DrawLine(cutlery, 7, 20, 7, 30);
            graphics.DrawLine(cutlery, 15, 20, 15, 30);
            graphics.DrawLine(cutlery, 7, 30, 15, 30);
            graphics.DrawLine(cutlery, 11, 30, 11, 52);
            graphics.DrawLine(cutlery, 61, 20, 61, 52);
            graphics.DrawArc(cutlery, 55, 17, 12, 18, 180, 180);
            return image;
        }

        private void ConfigureLoginPanel()
        {
            lblTitle.Text = "Đăng nhập";
            lblTitle.Font = new Font(UITheme.FontFamily, 24f, FontStyle.Bold);
            lblTitle.ForeColor = UITheme.PrimaryDark;
            lblTitle.SetBounds(50, 42, 422, 42);

            lblWelcome.Text = "Chào mừng bạn trở lại! Vui lòng đăng nhập hệ thống.";
            lblWelcome.Font = new Font(UITheme.FontFamily, 10f);
            lblWelcome.ForeColor = UITheme.TextMuted;
            lblWelcome.SetBounds(50, 88, 422, 24);

            lblUser.Text = "Tên đăng nhập";
            lblUser.Font = new Font(UITheme.FontFamily, 10f, FontStyle.Bold);
            lblUser.ForeColor = UITheme.TextPrimary;
            lblUser.SetBounds(50, 135, 422, 20);

            pnlUsernameInput = CreateRoundedInput(txtUsername, "👤", new Rectangle(50, 158, 422, 44), false);
            pnlUsernameInput.TabIndex = 0;
            pnlRight.Controls.Add(pnlUsernameInput);
            lblUsernameError = CreateFieldError("lblUsernameError");
            lblUsernameError.SetBounds(50, 204, 422, 18);
            pnlRight.Controls.Add(lblUsernameError);

            lblPass.Text = "Mật khẩu";
            lblPass.Font = new Font(UITheme.FontFamily, 10f, FontStyle.Bold);
            lblPass.ForeColor = UITheme.TextPrimary;
            lblPass.SetBounds(50, 232, 422, 20);

            pnlPasswordInput = CreateRoundedInput(txtPassword, "🔒", new Rectangle(50, 255, 422, 44), true);
            pnlPasswordInput.TabIndex = 1;
            pnlRight.Controls.Add(pnlPasswordInput);
            lblPasswordError = CreateFieldError("lblPasswordError");
            lblPasswordError.SetBounds(50, 301, 422, 18);
            pnlRight.Controls.Add(lblPasswordError);

            chkShowPassword.Visible = false;
            chkRememberUsername = new CheckBox
            {
                Text = "Ghi nhớ tên đăng nhập",
                AutoSize = true,
                Font = UITheme.FontSmall,
                ForeColor = UITheme.TextMuted,
                Location = new Point(50, 344),
                Cursor = Cursors.Hand
            };
            chkRememberUsername.TabIndex = 2;
            chkRememberUsername.CheckedChanged += ChkRememberUsername_CheckedChanged;
            pnlRight.Controls.Add(chkRememberUsername);

            lnkForgotPassword = new LinkLabel
            {
                Text = "Quên mật khẩu?",
                AutoSize = true,
                Font = UITheme.FontSmall,
                LinkColor = UITheme.Primary,
                ActiveLinkColor = UITheme.PrimaryDark,
                Location = new Point(366, 344),
                Cursor = Cursors.Hand
            };
            lnkForgotPassword.LinkClicked += (_, _) => MessageBox.Show(
                this,
                "Vui lòng liên hệ quản lý để đặt lại mật khẩu.",
                "Quên mật khẩu",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            pnlRight.Controls.Add(lnkForgotPassword);

            lblCapsLock = new Label
            {
                Text = "Caps Lock đang bật",
                AutoSize = true,
                Font = UITheme.FontSmall,
                ForeColor = UITheme.Warning,
                Location = new Point(50, 320),
                Visible = false
            };
            pnlRight.Controls.Add(lblCapsLock);

            pnlGeneralError = new Panel
            {
                Location = new Point(50, 370),
                Size = new Size(422, 42),
                BackColor = UITheme.DangerSoft,
                Visible = false
            };
            UITheme.ApplyRoundedCorners(pnlGeneralError, UITheme.CornerRadius);
            lblGeneralErrorIcon = new Label
            {
                Text = "⚠",
                Location = new Point(10, 0),
                Size = new Size(28, 42),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(UITheme.FontFamily, 13f, FontStyle.Bold),
                ForeColor = UITheme.Danger
            };
            lblError.Text = "Sai tên đăng nhập hoặc mật khẩu";
            lblError.Font = UITheme.FontSmall;
            lblError.ForeColor = UITheme.Danger;
            lblError.Location = new Point(40, 0);
            lblError.Size = new Size(372, 42);
            lblError.TextAlign = ContentAlignment.MiddleLeft;
            lblError.Visible = true;
            pnlGeneralError.Controls.Add(lblGeneralErrorIcon);
            pnlGeneralError.Controls.Add(lblError);
            pnlRight.Controls.Add(pnlGeneralError);

            btnLogin.Text = "ĐĂNG NHẬP";
            btnLogin.SetBounds(50, 420, 422, 48);
            btnLogin.Font = new Font(UITheme.FontFamily, 11f, FontStyle.Bold);
            btnLogin.BackColor = UITheme.Primary;
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatAppearance.MouseOverBackColor = ControlPaint.Dark(UITheme.Primary, 0.06f);
            btnLogin.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(UITheme.Primary, 0.14f);
            btnLogin.UseVisualStyleBackColor = false;
            UITheme.ApplyRoundedCorners(btnLogin, UITheme.CornerRadius);
            btnLogin.MouseEnter += (_, _) => { if (btnLogin.Enabled) btnLogin.BackColor = ControlPaint.Dark(UITheme.Primary, 0.06f); };
            btnLogin.MouseLeave += (_, _) => btnLogin.BackColor = btnLogin.Enabled ? UITheme.Primary : UITheme.Border;
            btnLogin.EnabledChanged += (_, _) =>
            {
                btnLogin.BackColor = btnLogin.Enabled ? UITheme.Primary : UITheme.Border;
                btnLogin.ForeColor = btnLogin.Enabled ? Color.White : UITheme.TextMuted;
            };

            lblSpinner = new Label
            {
                Text = "◴",
                Font = new Font(UITheme.FontFamily, 15f, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(btnLogin.Right - 42, btnLogin.Top, 36, btnLogin.Height),
                Visible = false
            };
            pnlRight.Controls.Add(lblSpinner);

            lblVersion.Text = "Phiên bản 1.0 • Quản Lý Nhà Hàng © 2026";
            lblVersion.Font = UITheme.FontSmall;
            lblVersion.ForeColor = UITheme.TextMuted;
            lblVersion.SetBounds(50, 502, 422, 20);
            txtUsername.TabIndex = 0;
            txtPassword.TabIndex = 1;
            btnLogin.TabIndex = 4;

            LoadRememberedUsername();
        }

        private Panel CreateRoundedInput(TextBox textBox, string icon, Rectangle bounds, bool password)
        {
            var panel = new Panel
            {
                Location = bounds.Location,
                Size = bounds.Size,
                BackColor = Color.White,
                Tag = false
            };
            void PaintBorder(object? sender, PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var invalid = password ? _passwordInvalid : _usernameInvalid;
                var focused = textBox.Focused || (password && btnTogglePassword?.Focused == true);
                var color = invalid ? UITheme.Danger : focused ? UITheme.Primary : UITheme.Border;
                using var pen = new Pen(color, focused && !invalid ? 2f : 1f);
                var rect = new Rectangle(1, 1, panel.Width - 3, panel.Height - 3);
                var diameter = UITheme.CornerRadius * 2;
                using var path = new System.Drawing.Drawing2D.GraphicsPath();
                path.AddArc(rect.Left, rect.Top, diameter, diameter, 180, 90);
                path.AddArc(rect.Right - diameter, rect.Top, diameter, diameter, 270, 90);
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(rect.Left, rect.Bottom - diameter, diameter, diameter, 90, 90);
                path.CloseFigure();
                e.Graphics.DrawPath(pen, path);
            }
            panel.Paint += PaintBorder;
            panel.Resize += (_, _) => panel.Invalidate();
            UITheme.ApplyRoundedCorners(panel, UITheme.CornerRadius);

            var iconLabel = new Label
            {
                Text = icon,
                Location = new Point(12, 0),
                Size = new Size(30, bounds.Height),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Emoji", 12f),
                ForeColor = UITheme.TextMuted,
                BackColor = Color.Transparent
            };
            panel.Controls.Add(iconLabel);

            textBox.BorderStyle = BorderStyle.None;
            textBox.Font = new Font(UITheme.FontFamily, 10f);
            textBox.BackColor = Color.White;
            textBox.Location = new Point(48, 11);
            textBox.Size = new Size(password ? bounds.Width - 94 : bounds.Width - 60, 22);
            textBox.Enter += (_, _) => { panel.Invalidate(); if (password) UpdateCapsLockIndicator(); };
            textBox.Leave += (_, _) => { panel.Invalidate(); if (password) UpdateCapsLockIndicator(); };
            if (password)
            {
                textBox.KeyUp += (_, _) => UpdateCapsLockIndicator();
                textBox.KeyDown += (_, _) => UpdateCapsLockIndicator();
            }
            textBox.TextChanged += (_, _) =>
            {
                panel.Invalidate();
                if (!_suppressInputFeedback)
                {
                    HideGeneralError();
                    if (password) SetPasswordError(string.Empty);
                    else
                    {
                        SetUsernameError(string.Empty);
                        if (chkRememberUsername?.Checked == true) SaveRememberedUsername();
                    }
                }
            };
            panel.Controls.Add(textBox);

            if (password)
            {
                btnTogglePassword = new Button
                {
                    Text = "◉",
                    Location = new Point(bounds.Width - 42, 4),
                    Size = new Size(34, 34),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.White,
                    ForeColor = UITheme.TextMuted,
                    Font = new Font(UITheme.FontFamily, 12f),
                    Cursor = Cursors.Hand,
                    TabStop = false,
                    UseVisualStyleBackColor = false
                };
                btnTogglePassword.TabStop = false;
                btnTogglePassword.FlatAppearance.BorderSize = 0;
                btnTogglePassword.FlatAppearance.MouseOverBackColor = UITheme.AlternateRow;
                btnTogglePassword.Click += (_, _) =>
                {
                    chkShowPassword.Checked = !chkShowPassword.Checked;
                    textBox.Focus();
                    panel.Invalidate();
                };
                panel.Controls.Add(btnTogglePassword);
            }

            return panel;
        }

        private static Label CreateFieldError(string name) => new()
        {
            Name = name,
            AutoSize = false,
            Font = UITheme.FontSmall,
            ForeColor = UITheme.Danger,
            Visible = false
        };

        private void SetUsernameError(string message)
        {
            _usernameInvalid = !string.IsNullOrEmpty(message);
            lblUsernameError.Text = message;
            lblUsernameError.Visible = _usernameInvalid;
            pnlUsernameInput.Invalidate();
        }

        private void SetPasswordError(string message)
        {
            _passwordInvalid = !string.IsNullOrEmpty(message);
            lblPasswordError.Text = message;
            lblPasswordError.Visible = _passwordInvalid;
            pnlPasswordInput.Invalidate();
        }

        private void HideGeneralError()
        {
            if (pnlGeneralError == null) return;
            pnlGeneralError.Visible = false;
            lblError.Visible = false;
        }

        private void LoadRememberedUsername()
        {
            try
            {
                txtUsername.Text = QuanLyNhaHang.Properties.Settings.Default.RememberedUsername;
                chkRememberUsername.Checked = !string.IsNullOrWhiteSpace(txtUsername.Text);
            }
            catch { /* Remembered username is optional. */ }
        }

        private void ChkRememberUsername_CheckedChanged(object? sender, EventArgs e)
        {
            if (!chkRememberUsername.Checked)
            {
                QuanLyNhaHang.Properties.Settings.Default.RememberedUsername = string.Empty;
                try { QuanLyNhaHang.Properties.Settings.Default.Save(); }
                catch { /* Do not block sign-in if the preference cannot be removed. */ }
            }
            else SaveRememberedUsername();
        }

        private void SaveRememberedUsername()
        {
            try
            {
                QuanLyNhaHang.Properties.Settings.Default.RememberedUsername = txtUsername.Text.Trim();
                QuanLyNhaHang.Properties.Settings.Default.Save();
            }
            catch { /* Remembered username is optional. */ }
        }

        private void WindowDrag_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            ReleaseCapture();
            SendMessage(Handle, WmNcLButtonDown, HtCaption, IntPtr.Zero);
        }

        private void btnMinimize_Click(object? sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void btnClose_Click(object? sender, EventArgs e)
        {
            Close();
        }

        private void FormLogin_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Escape) return;

            e.Handled = true;
            e.SuppressKeyPress = true;
            if (string.IsNullOrWhiteSpace(txtUsername.Text) && string.IsNullOrEmpty(txtPassword.Text))
            {
                Close();
                return;
            }

            if (ValidationHelper.Confirm("Bạn có chắc muốn thoát khỏi màn hình đăng nhập?"))
                Close();
        }

        private void SetLoginBusy(bool busy)
        {
            txtUsername.Enabled = !busy;
            txtPassword.Enabled = !busy;
            btnTogglePassword.Enabled = !busy;
            chkRememberUsername.Enabled = !busy;
            lnkForgotPassword.Enabled = !busy;

            if (busy)
            {
                btnLogin.Text = "Đang đăng nhập...";
                btnLogin.Enabled = false;
                lblSpinner.Visible = true;
                _spinnerTimer.Start();
                return;
            }

            _spinnerTimer.Stop();
            lblSpinner.Visible = false;
            if (_lockoutUntil.HasValue)
            {
                UpdateLockoutDisplay();
                return;
            }

            btnLogin.Text = "ĐĂNG NHẬP";
            btnLogin.Enabled = true;
        }

        private void UpdateCapsLockIndicator()
        {
            if (lblCapsLock == null || txtPassword == null) return;
            lblCapsLock.Visible = txtPassword.Focused && Control.IsKeyLocked(Keys.CapsLock);
        }

        private async Task ShakeFormAsync()
        {
            var original = Location;
            var offsets = new[] { -8, 8, -7, 7, -5, 5, -3, 3, -1, 1 };
            foreach (var offset in offsets)
            {
                Location = new Point(original.X + offset, original.Y);
                await Task.Delay(30);
            }
            Location = original;
        }

        private void StartLockout()
        {
            _lockoutUntil = DateTime.Now.AddSeconds(30);
            _lockoutTimer.Start();
            UpdateLockoutDisplay();
        }

        private void LockoutTimer_Tick(object? sender, EventArgs e)
        {
            UpdateLockoutDisplay();
        }

        private void UpdateLockoutDisplay()
        {
            if (!_lockoutUntil.HasValue) return;
            var remainingSeconds = (int)Math.Ceiling((_lockoutUntil.Value - DateTime.Now).TotalSeconds);
            if (remainingSeconds <= 0)
            {
                _lockoutTimer.Stop();
                _lockoutUntil = null;
                _failedLoginAttempts = 0;
                btnLogin.Text = "ĐĂNG NHẬP";
                btnLogin.Enabled = true;
                HideGeneralError();
                return;
            }

            btnLogin.Text = $"Thử lại sau {remainingSeconds}s";
            btnLogin.Enabled = false;
            lblError.Text = $"Bạn đã đăng nhập sai 5 lần. Thử lại sau {remainingSeconds}s";
            lblError.Visible = true;
            pnlGeneralError.Visible = true;
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            HideGeneralError();
            SetUsernameError(string.Empty);
            SetPasswordError(string.Empty);

            var hasUsername = !string.IsNullOrWhiteSpace(txtUsername.Text);
            var hasPassword = !string.IsNullOrEmpty(txtPassword.Text);
            if (!hasUsername) SetUsernameError("Vui lòng nhập tên đăng nhập");
            if (!hasPassword) SetPasswordError("Vui lòng nhập mật khẩu");
            if (!hasUsername || !hasPassword)
            {
                (hasUsername ? txtPassword : txtUsername).Focus();
                return;
            }

            SetLoginBusy(true);
            (bool Success, string Message) result;
            string submittedPassword = txtPassword.Text;
            try
            {
                result = await AuthService.Instance.LoginAsync(txtUsername.Text, submittedPassword);
            }
            catch
            {
                SetLoginBusy(false);
                lblError.Text = "Không thể kết nối đến hệ thống. Vui lòng thử lại.";
                lblError.Visible = true;
                pnlGeneralError.Visible = true;
                return;
            }
            finally
            {
                _suppressInputFeedback = true;
                txtPassword.Clear();
                _suppressInputFeedback = false;
                submittedPassword = string.Empty;
            }
            SetLoginBusy(false);

            if (result.Success)
            {
                _failedLoginAttempts = 0;
                var main = new FormMain();
                main.Show();
                this.Hide();
            }
            else
            {
                lblError.Text = "Sai tên đăng nhập hoặc mật khẩu";
                lblError.Visible = true;
                pnlGeneralError.Visible = true;
                SetUsernameError("Kiểm tra lại thông tin đăng nhập");
                SetPasswordError("Kiểm tra lại thông tin đăng nhập");
                _failedLoginAttempts++;

                if (_failedLoginAttempts >= 5) StartLockout();
                await ShakeFormAsync();
                txtPassword.Focus();
                UpdateCapsLockIndicator();
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            UpdateCapsLockIndicator();
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                btnLogin.PerformClick();
            }
        }

        private void txtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                txtPassword.Focus();
            }
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = chkShowPassword.Checked ? '\0' : '●';
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            lblError.Visible = false;
            pnlGeneralError.Visible = false;
            BeginInvoke(new Action(() =>
            {
                if (chkRememberUsername.Checked && !string.IsNullOrWhiteSpace(txtUsername.Text))
                    txtPassword.Focus();
                else
                    txtUsername.Focus();
                UpdateCapsLockIndicator();
            }));
        }
    }
}
