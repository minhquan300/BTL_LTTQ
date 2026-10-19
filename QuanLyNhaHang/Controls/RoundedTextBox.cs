using QuanLyNhaHang.Helpers;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Drawing2D;

namespace QuanLyNhaHang.Controls;

/// <summary>Rounded input with a leading icon, focus/error border, and optional password reveal.</summary>
public sealed class RoundedTextBox : UserControl
{
    private readonly TextBox _textBox;
    private readonly Label _icon;
    private readonly Button? _togglePassword;
    private bool _focused;
    private bool _hasError;
    private bool _passwordVisible;
    private string _iconText = string.Empty;
    private string _placeholderText = string.Empty;

    [Browsable(false)]
    public event KeyEventHandler? InputKeyDown;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string IconText
    {
        get => _iconText;
        set { _iconText = value ?? string.Empty; _icon.Text = _iconText; }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PlaceholderText
    {
        get => _placeholderText;
        set { _placeholderText = value ?? string.Empty; _textBox.PlaceholderText = _placeholderText; }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool UseSystemPasswordChar
    {
        get => _textBox.UseSystemPasswordChar;
        set
        {
            _textBox.UseSystemPasswordChar = value && !_passwordVisible;
            _passwordVisible = false;
            UpdateEyeIcon();
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool HasError
    {
        get => _hasError;
        set { _hasError = value; Invalidate(); }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [AllowNull]
    public override string Text
    {
        get => _textBox.Text;
        set => _textBox.Text = value ?? string.Empty;
    }

    public RoundedTextBox() : this(false) { }

    public RoundedTextBox(bool showPasswordToggle)
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        DoubleBuffered = true;
        BackColor = UITheme.Surface;
        Size = new Size(372, 42);
        MinimumSize = new Size(180, 42);
        TabStop = false;

        _icon = new Label
        {
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = UITheme.TextMuted,
            Font = new Font("Segoe UI Emoji", 12f),
            Location = new Point(7, 4),
            Size = new Size(28, 34),
            BackColor = UITheme.Surface
        };
        _textBox = new TextBox
        {
            BorderStyle = BorderStyle.None,
            Font = UITheme.FontNormal,
            ForeColor = UITheme.TextPrimary,
            BackColor = UITheme.Surface,
            Location = new Point(39, 11),
            Height = 22,
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
            TabStop = true
        };
        _textBox.GotFocus += (_, _) => { _focused = true; Invalidate(); };
        _textBox.LostFocus += (_, _) => { _focused = false; Invalidate(); };
        _textBox.TextChanged += (_, e) => OnTextChanged(e);
        _textBox.KeyDown += (_, e) => InputKeyDown?.Invoke(this, e);
        Controls.Add(_icon);
        Controls.Add(_textBox);

        if (showPasswordToggle)
        {
            _togglePassword = new Button
            {
                Text = "◉",
                FlatStyle = FlatStyle.Flat,
                BackColor = UITheme.Surface,
                ForeColor = UITheme.TextMuted,
                Font = new Font(UITheme.FontFamily, 11f),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(30, 30),
                Location = new Point(Width - 36, 6),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                TabStop = false,
                Cursor = Cursors.Hand,
                AccessibleName = "Hiện hoặc ẩn mật khẩu"
            };
            _togglePassword.FlatAppearance.BorderSize = 0;
            _togglePassword.FlatAppearance.MouseOverBackColor = UITheme.PrimarySoft;
            _togglePassword.FlatAppearance.MouseDownBackColor = UITheme.Selection;
            _togglePassword.Click += (_, _) =>
            {
                _passwordVisible = !_passwordVisible;
                _textBox.UseSystemPasswordChar = !_passwordVisible;
                UpdateEyeIcon();
                _textBox.Focus();
            };
            Controls.Add(_togglePassword);
        }
        Resize += (_, _) => LayoutInput();
        LayoutInput();
    }

    public new bool Focus() => _textBox.Focus();

    public void Clear() => _textBox.Clear();

    protected override void OnClick(EventArgs e)
    {
        base.OnClick(e);
        _textBox.Focus();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = new Rectangle(1, 1, Width - 3, Height - 3);
        float dpiScale = DeviceDpi / 96f;
        int radius = (int)Math.Round(UITheme.CornerRadius * 2 * dpiScale);
        using var path = new GraphicsPath();
        path.AddArc(rect.Left, rect.Top, radius, radius, 180, 90);
        path.AddArc(rect.Right - radius, rect.Top, radius, radius, 270, 90);
        path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
        path.AddArc(rect.Left, rect.Bottom - radius, radius, radius, 90, 90);
        path.CloseFigure();
        Color border = _hasError ? UITheme.Danger : _focused ? UITheme.Primary : UITheme.InputBorder;
        using var pen = new Pen(border, (_focused || _hasError ? 2f : 1f) * dpiScale);
        e.Graphics.DrawPath(pen, path);
    }

    private void LayoutInput()
    {
        int Scale(int value) => (int)Math.Round(value * DeviceDpi / 96f);
        int textHeight = Scale(22);
        _textBox.Height = textHeight;
        _textBox.Location = new Point(Scale(39), Math.Max(Scale(8), (Height - _textBox.Height) / 2));
        int rightInset = Scale(_togglePassword == null ? 12 : 42);
        _textBox.Width = Math.Max(Scale(50), Width - _textBox.Left - rightInset);
        _icon.Size = new Size(Scale(28), Scale(34));
        _icon.Location = new Point(Scale(7), Math.Max(Scale(3), (Height - _icon.Height) / 2));
        if (_togglePassword != null)
        {
            _togglePassword.Size = new Size(Scale(30), Scale(30));
            _togglePassword.Location = new Point(Width - _togglePassword.Width - Scale(6),
                (Height - _togglePassword.Height) / 2);
        }
    }

    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);
        LayoutInput();
        Invalidate();
    }

    private void UpdateEyeIcon()
    {
        if (_togglePassword != null)
            _togglePassword.Text = _passwordVisible ? "◎" : "◉";
    }
}
