using QuanLyNhaHang.Helpers;
using QuanLyNhaHang.Service;
using System.ComponentModel;

namespace QuanLyNhaHang.Forms;

/// <summary>Reusable management credential prompt that validates credentials before returning OK.</summary>
public partial class FormXacNhanQuanLy : Form
{
    private readonly System.Windows.Forms.Timer _spinnerTimer = new() { Interval = 90 };
    private readonly System.Windows.Forms.Timer _lockoutTimer = new() { Interval = 250 };
    private readonly System.Windows.Forms.Timer _shakeTimer = new() { Interval = 12 };
    private static readonly object AttemptSync = new();
    private static readonly Dictionary<string, AttemptState> Attempts = new(StringComparer.OrdinalIgnoreCase);
    private static readonly string[] SpinnerFrames = { "◐", "◓", "◑", "◒" };
    private int _spinnerFrame;
    private int _shakeStep;
    private Point _shakeOrigin;
    private bool _isChecking;
    private DateTimeOffset? _activeLockoutUntil;

    private readonly record struct AttemptState(int Failures, DateTimeOffset? LockedUntil);

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string MoTaThaoTac
    {
        get => lblMoTa.Text;
        set
        {
            lblMoTa.Text = value?.Trim() ?? string.Empty;
            UpdateDescriptionLayout();
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int? MaQuanLyDuyet { get; private set; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? TenQuanLyDuyet { get; private set; }

    public FormXacNhanQuanLy()
    {
        UITheme.ConfigureDpi(this);
        InitializeComponent();
        UpdateDescriptionLayout();
        UITheme.EnableDoubleBuffering(this);
        _spinnerTimer.Tick += (_, _) =>
        {
            _spinnerFrame = (_spinnerFrame + 1) % SpinnerFrames.Length;
            btnXacNhan.Text = SpinnerFrames[_spinnerFrame] + "  Đang kiểm tra...";
        };
        _lockoutTimer.Tick += (_, _) => RefreshLockoutDisplay();
        _shakeTimer.Tick += ShakeTimer_Tick;
        txtTaiKhoan.InputKeyDown += txtTaiKhoan_InputKeyDown;
        txtTaiKhoan.TextChanged += txtTaiKhoan_TextChanged;
        txtMatKhau.TextChanged += (_, _) =>
        {
            lblLoi.Text = string.Empty;
            txtMatKhau.HasError = false;
        };
        Shown += (_, _) =>
        {
            RefreshLockoutDisplay();
            txtTaiKhoan.Focus();
        };
        FormClosed += (_, _) =>
        {
            _spinnerTimer.Dispose();
            _lockoutTimer.Dispose();
            _shakeTimer.Dispose();
            txtMatKhau.Clear();
        };
    }

    private void UpdateDescriptionLayout()
    {
        bool showDescription = !string.IsNullOrWhiteSpace(lblMoTa.Text);
        pnlThaoTac.Visible = showDescription;
        int offset = showDescription ? 0 : -54;
        lblTaiKhoan.Top = 145 + offset;
        txtTaiKhoan.Top = 160 + offset;
        lblTaiKhoanLoi.Top = 203 + offset;
        lblMatKhau.Top = 219 + offset;
        txtMatKhau.Top = 234 + offset;
        lblLoi.Top = 277 + offset;
        if (IsHandleCreated) PerformLayout();
    }

    private async void btnXacNhan_Click(object? sender, EventArgs e)
    {
        if (_isChecking) return;
        RefreshLockoutDisplay();
        if (_activeLockoutUntil.HasValue && _activeLockoutUntil.Value > DateTimeOffset.UtcNow) return;

        bool accountMissing = string.IsNullOrWhiteSpace(txtTaiKhoan.Text);
        bool passwordMissing = string.IsNullOrEmpty(txtMatKhau.Text);
        lblTaiKhoanLoi.Text = accountMissing ? "Nhập tên đăng nhập quản lý." : string.Empty;
        lblLoi.Text = passwordMissing ? "Nhập mật khẩu quản lý." : string.Empty;
        txtTaiKhoan.HasError = accountMissing;
        txtMatKhau.HasError = passwordMissing;
        if (accountMissing || passwordMissing)
        {
            if (accountMissing) txtTaiKhoan.Focus();
            else txtMatKhau.Focus();
            return;
        }

        string username = txtTaiKhoan.Text.Trim();
        string submittedPassword = txtMatKhau.Text;
        bool focusPasswordAfterCheck = false;
        bool focusAccountAfterCheck = false;
        txtMatKhau.Clear();
        SetChecking(true);
        try
        {
            var result = await AuthService.Instance.AuthenticateManagerForApprovalAsync(username, submittedPassword);
            switch (result.Status)
            {
                case ManagerApprovalStatus.Approved:
                    await AuthService.Instance.LogManagerApprovalAsync(
                        result.EmployeeID!.Value,
                        MoTaThaoTac);
                    ResetAttempts(username);
                    MaQuanLyDuyet = result.EmployeeID;
                    TenQuanLyDuyet = result.FullName;
                    DialogResult = DialogResult.OK;
                    return;

                case ManagerApprovalStatus.NotManager:
                    ResetAttempts(username);
                    lblTaiKhoanLoi.Text = "Tài khoản này không có quyền xác nhận.";
                    txtTaiKhoan.HasError = true;
                    focusAccountAfterCheck = true;
                    break;

                default:
                    lblLoi.Text = "Thông tin xác nhận không đúng.";
                    txtMatKhau.HasError = true;
                    focusPasswordAfterCheck = true;
                    DateTimeOffset? lockedUntil = RegisterInvalidAttempt(username);
                    if (lockedUntil.HasValue)
                    {
                        _activeLockoutUntil = lockedUntil;
                        _lockoutTimer.Start();
                        _ = AuthService.Instance.LogManagerApprovalLockoutAsync(username);
                    }
                    StartShake();
                    break;
            }
        }
        catch
        {
            lblLoi.Text = "Không thể xác thực hoặc ghi nhật ký duyệt lúc này. Vui lòng thử lại.";
            focusPasswordAfterCheck = true;
        }
        finally
        {
            submittedPassword = string.Empty;
            txtMatKhau.Clear();
            SetChecking(false);
        }
        if (focusAccountAfterCheck) txtTaiKhoan.Focus();
        else if (focusPasswordAfterCheck) txtMatKhau.Focus();
    }

    private void btnHuy_Click(object? sender, EventArgs e) => DialogResult = DialogResult.Cancel;

    private void txtTaiKhoan_InputKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter) return;
        e.SuppressKeyPress = true;
        e.Handled = true;
        txtMatKhau.Focus();
    }

    private void txtTaiKhoan_TextChanged(object? sender, EventArgs e)
    {
        lblTaiKhoanLoi.Text = string.Empty;
        txtTaiKhoan.HasError = false;
        lblLoi.Text = string.Empty;
        txtMatKhau.HasError = false;
        RefreshLockoutDisplay();
    }

    private void SetChecking(bool checking)
    {
        _isChecking = checking;
        txtTaiKhoan.Enabled = !checking;
        txtMatKhau.Enabled = !checking;
        btnHuy.Enabled = !checking;
        ControlBox = !checking;
        if (checking)
        {
            _spinnerFrame = 0;
            btnXacNhan.Text = SpinnerFrames[0] + "  Đang kiểm tra...";
            btnXacNhan.Enabled = false;
            _spinnerTimer.Start();
        }
        else
        {
            _spinnerTimer.Stop();
            RefreshLockoutDisplay();
        }
    }

    private void RefreshLockoutDisplay()
    {
        if (IsDisposed || btnXacNhan == null) return;
        string username = txtTaiKhoan.Text.Trim();
        _activeLockoutUntil = GetLockoutUntil(username);
        if (_activeLockoutUntil.HasValue)
        {
            TimeSpan remaining = _activeLockoutUntil.Value - DateTimeOffset.UtcNow;
            int seconds = Math.Max(1, (int)Math.Ceiling(remaining.TotalSeconds));
            btnXacNhan.Text = $"Thử lại sau {seconds}s";
            lblLoi.Text = $"Đã tạm khóa xác nhận. Còn {seconds} giây.";
            btnXacNhan.Enabled = false;
            _lockoutTimer.Start();
        }
        else
        {
            _lockoutTimer.Stop();
            if (!_isChecking)
            {
                btnXacNhan.Text = "Xác nhận";
                btnXacNhan.Enabled = true;
            }
        }
    }

    private static DateTimeOffset? RegisterInvalidAttempt(string username)
    {
        string key = username.Trim();
        lock (AttemptSync)
        {
            var state = Attempts.TryGetValue(key, out var current) ? current : new AttemptState(0, null);
            if (state.LockedUntil.HasValue && state.LockedUntil.Value > DateTimeOffset.UtcNow)
                return state.LockedUntil;
            int failures = state.Failures + 1;
            if (failures >= 3)
            {
                var until = DateTimeOffset.UtcNow.AddSeconds(30);
                Attempts[key] = new AttemptState(0, until);
                return until;
            }
            Attempts[key] = new AttemptState(failures, null);
            return null;
        }
    }

    private static DateTimeOffset? GetLockoutUntil(string username)
    {
        string key = username.Trim();
        if (key.Length == 0) return null;
        lock (AttemptSync)
        {
            if (!Attempts.TryGetValue(key, out var state)) return null;
            if (state.LockedUntil is null) return null;
            if (state.LockedUntil.Value > DateTimeOffset.UtcNow) return state.LockedUntil;
            Attempts.Remove(key);
            return null;
        }
    }

    private static void ResetAttempts(string username)
    {
        lock (AttemptSync) Attempts.Remove(username.Trim());
    }

    private void StartShake()
    {
        _shakeOrigin = Location;
        _shakeStep = 0;
        _shakeTimer.Start();
    }

    private void ShakeTimer_Tick(object? sender, EventArgs e)
    {
        _shakeStep++;
        if (_shakeStep >= 25)
        {
            _shakeTimer.Stop();
            Location = _shakeOrigin;
            return;
        }
        int offset = (int)Math.Round(Math.Sin(_shakeStep * 2.4) * (9 - (_shakeStep * 9d / 25)));
        Location = new Point(_shakeOrigin.X + offset, _shakeOrigin.Y);
    }
}
