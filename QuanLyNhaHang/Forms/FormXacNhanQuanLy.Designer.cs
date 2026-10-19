#nullable enable
using QuanLyNhaHang.Helpers;
using QuanLyNhaHang.Controls;
using System.ComponentModel;

namespace QuanLyNhaHang.Forms;

partial class FormXacNhanQuanLy
{
    private IContainer? components;
    private Label lblBieuTuong = null!;
    private Label lblTieuDe = null!;
    private Label lblMoTaPhu = null!;
    private Panel pnlThaoTac = null!;
    private Label lblMoTa = null!;
    private Label lblTaiKhoan = null!;
    private RoundedTextBox txtTaiKhoan = null!;
    private Label lblTaiKhoanLoi = null!;
    private Label lblMatKhau = null!;
    private RoundedTextBox txtMatKhau = null!;
    private Label lblLoi = null!;
    private Button btnXacNhan = null!;
    private Button btnHuy = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new Container();
        lblBieuTuong = new Label();
        lblTieuDe = new Label();
        lblMoTaPhu = new Label();
        pnlThaoTac = new Panel();
        lblMoTa = new Label();
        lblTaiKhoan = new Label();
        txtTaiKhoan = new RoundedTextBox();
        lblTaiKhoanLoi = new Label();
        lblMatKhau = new Label();
        txtMatKhau = new RoundedTextBox(true);
        lblLoi = new Label();
        btnXacNhan = new Button();
        btnHuy = new Button();
        SuspendLayout();

        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = UITheme.Surface;
        ClientSize = new Size(420, 340);
        Font = UITheme.FontNormal;
        ForeColor = UITheme.TextPrimary;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FormXacNhanQuanLy";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Cần quản lý xác nhận";

        lblBieuTuong.BackColor = UITheme.WarningSoft;
        lblBieuTuong.Font = new Font("Segoe UI Emoji", 20F, FontStyle.Regular);
        lblBieuTuong.ForeColor = UITheme.Warning;
        lblBieuTuong.Location = new Point(24, 18);
        lblBieuTuong.Size = new Size(54, 54);
        lblBieuTuong.Text = "🔒";
        lblBieuTuong.TextAlign = ContentAlignment.MiddleCenter;
        UITheme.ApplyRoundedCorners(lblBieuTuong, 27);

        lblTieuDe.AutoSize = true;
        lblTieuDe.Font = new Font(UITheme.FontFamily, 14F, FontStyle.Bold);
        lblTieuDe.ForeColor = UITheme.TextPrimary;
        lblTieuDe.Location = new Point(91, 22);
        lblTieuDe.Text = "Cần quản lý xác nhận";

        lblMoTaPhu.Font = UITheme.FontSmall;
        lblMoTaPhu.ForeColor = UITheme.TextMuted;
        lblMoTaPhu.Location = new Point(92, 49);
        lblMoTaPhu.Size = new Size(300, 34);
        lblMoTaPhu.Text = "Nhập tài khoản quản lý để xác thực thao tác này.";

        pnlThaoTac.BackColor = UITheme.WarningSoft;
        pnlThaoTac.Location = new Point(24, 90);
        pnlThaoTac.Size = new Size(372, 48);
        pnlThaoTac.Paint += (_, e) =>
        {
            using var pen = new Pen(UITheme.Warning, 1);
            e.Graphics.DrawRectangle(pen, 0, 0, pnlThaoTac.Width - 1, pnlThaoTac.Height - 1);
        };
        lblMoTa.Dock = DockStyle.Fill;
        lblMoTa.Font = new Font(UITheme.FontFamily, 9.5F, FontStyle.Bold);
        lblMoTa.ForeColor = UITheme.TextPrimary;
        lblMoTa.Padding = new Padding(12, 6, 10, 6);
        lblMoTa.TextAlign = ContentAlignment.MiddleLeft;
        lblMoTa.AutoEllipsis = true;
        pnlThaoTac.Controls.Add(lblMoTa);

        lblTaiKhoan.AutoSize = true;
        lblTaiKhoan.Font = UITheme.FontSmall;
        lblTaiKhoan.ForeColor = UITheme.TextPrimary;
        lblTaiKhoan.Location = new Point(24, 145);
        lblTaiKhoan.Text = "Tài khoản quản lý";
        txtTaiKhoan.IconText = "👤";
        txtTaiKhoan.Location = new Point(24, 160);
        txtTaiKhoan.PlaceholderText = "Tên đăng nhập quản lý";
        txtTaiKhoan.Size = new Size(372, 42);
        txtTaiKhoan.TabIndex = 0;

        lblTaiKhoanLoi.Font = UITheme.FontSmall;
        lblTaiKhoanLoi.ForeColor = UITheme.Danger;
        lblTaiKhoanLoi.Location = new Point(24, 203);
        lblTaiKhoanLoi.Size = new Size(372, 14);

        lblMatKhau.AutoSize = true;
        lblMatKhau.Font = UITheme.FontSmall;
        lblMatKhau.ForeColor = UITheme.TextPrimary;
        lblMatKhau.Location = new Point(24, 219);
        lblMatKhau.Text = "Mật khẩu";
        txtMatKhau.IconText = "🔒";
        txtMatKhau.Location = new Point(24, 234);
        txtMatKhau.PlaceholderText = "Mật khẩu";
        txtMatKhau.UseSystemPasswordChar = true;
        txtMatKhau.Size = new Size(372, 42);
        txtMatKhau.TabIndex = 1;

        lblLoi.Font = UITheme.FontSmall;
        lblLoi.ForeColor = UITheme.Danger;
        lblLoi.Location = new Point(24, 277);
        lblLoi.Size = new Size(372, 14);

        btnXacNhan = UITheme.CreateButton("Xác nhận", UITheme.Primary);
        btnXacNhan.Location = new Point(224, 292);
        btnXacNhan.Size = new Size(172, 44);
        btnXacNhan.TabIndex = 2;
        btnXacNhan.Click += btnXacNhan_Click;

        btnHuy = UITheme.CreateButton("Hủy", UITheme.Surface, UITheme.TextMuted);
        btnHuy.FlatAppearance.BorderSize = 1;
        btnHuy.FlatAppearance.BorderColor = UITheme.InputBorder;
        btnHuy.Location = new Point(104, 292);
        btnHuy.Size = new Size(108, 44);
        btnHuy.TabIndex = 3;
        btnHuy.Click += btnHuy_Click;

        AcceptButton = btnXacNhan;
        CancelButton = btnHuy;
        Controls.AddRange(new Control[] { lblBieuTuong, lblTieuDe, lblMoTaPhu, pnlThaoTac,
            lblTaiKhoan, txtTaiKhoan, lblTaiKhoanLoi, lblMatKhau, txtMatKhau, lblLoi, btnXacNhan, btnHuy });
        ResumeLayout(false);
        PerformLayout();
    }
}
