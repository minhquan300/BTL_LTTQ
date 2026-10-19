using System.Drawing.Drawing2D;
using System.Reflection;

namespace QuanLyNhaHang.Helpers
{
    /// <summary>Shared colors, typography, spacing, and reusable control styling.</summary>
    public static class UITheme
    {
        public static readonly Color Navy = Color.FromArgb(30, 58, 95);          // #1E3A5F
        public static readonly Color Primary = Color.FromArgb(37, 99, 235);       // #2563EB
        public static readonly Color PrimaryDark = Navy;
        public static readonly Color PrimarySoft = Color.FromArgb(239, 246, 255);
        public static readonly Color Success = Color.FromArgb(22, 163, 74);       // #16A34A
        public static readonly Color SuccessSoft = Color.FromArgb(220, 252, 231);
        public static readonly Color Danger = Color.FromArgb(220, 38, 38);        // #DC2626
        public static readonly Color DangerSoft = Color.FromArgb(254, 226, 226);
        public static readonly Color Warning = Color.FromArgb(245, 158, 11);      // #F59E0B
        public static readonly Color WarningSoft = Color.FromArgb(254, 243, 199);
        public static readonly Color Info = Color.FromArgb(59, 130, 246);
        public static readonly Color Light = Color.White;
        public static readonly Color Dark = Navy;
        public static readonly Color White = Color.White;
        public static readonly Color TextPrimary = Color.FromArgb(31, 41, 55);    // #1F2937
        public static readonly Color TextMuted = Color.FromArgb(107, 114, 128);  // #6B7280
        public static readonly Color Border = Color.FromArgb(229, 231, 235);      // #E5E7EB
        public static readonly Color InputBorder = Color.FromArgb(209, 213, 219); // #D1D5DB
        public static readonly Color Surface = Color.White;
        public static readonly Color AlternateRow = Color.FromArgb(249, 250, 251);
        public static readonly Color Selection = Color.FromArgb(219, 234, 254);
        public static readonly Color SidebarBg = Navy;
        public static readonly Color SidebarHover = Color.FromArgb(41, 72, 110);

        public const string FontFamily = "Segoe UI";
        public static readonly Font FontTitle = new(FontFamily, 16f, FontStyle.Bold);
        public static readonly Font FontHeader = new(FontFamily, 16f, FontStyle.Bold);
        public static readonly Font FontNormal = new(FontFamily, 10f);
        public static readonly Font FontSmall = new(FontFamily, 9f);
        public static readonly Font FontLarge = new(FontFamily, 20f, FontStyle.Bold);
        public const int SpaceSmall = 8;
        public const int SpaceStandard = 16;
        public const int CornerRadius = 8;
        public const int ButtonCornerRadius = CornerRadius;

        private static readonly PropertyInfo? DoubleBufferedProperty =
            typeof(Control).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>Call before InitializeComponent so WinForms scales designer dimensions from 96 DPI.</summary>
        public static void ConfigureDpi(Form form)
        {
            form.AutoScaleMode = AutoScaleMode.Dpi;
            form.AutoScaleDimensions = new SizeF(96f, 96f);
            form.Font = FontNormal;
            form.ForeColor = TextPrimary;
            form.BackColor = Surface;
        }

        /// <summary>Enable buffered painting on a form and its current/future child controls.</summary>
        public static void EnableDoubleBuffering(Control root)
        {
            SetDoubleBuffered(root);
            root.ControlAdded += (_, e) =>
            {
                if (e.Control != null) EnableDoubleBuffering(e.Control);
            };
            foreach (Control child in root.Controls)
                EnableDoubleBuffering(child);
        }

        private static void SetDoubleBuffered(Control control)
        {
            try { DoubleBufferedProperty?.SetValue(control, true); }
            catch (TargetInvocationException) { }
            catch (ArgumentException) { }
        }

        public static Button CreateButton(string text, Color backColor, Color foreColor = default)
        {
            var btn = new Button
            {
                Text = text,
                BackColor = backColor,
                ForeColor = foreColor == default ? Color.White : foreColor,
                FlatStyle = FlatStyle.Flat,
                Font = FontNormal,
                Height = 36,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            SetButtonInteraction(btn, btn.BackColor);
            ApplyRoundedCorners(btn);
            return btn;
        }

        public static void SetButtonInteraction(Button button, Color normalColor,
            Color? hoverColor = null, Color? pressedColor = null)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.UseVisualStyleBackColor = false;
            button.FlatAppearance.MouseOverBackColor = hoverColor ?? ControlPaint.Dark(normalColor, 0.06f);
            button.FlatAppearance.MouseDownBackColor = pressedColor ?? ControlPaint.Dark(normalColor, 0.14f);
            button.Cursor = Cursors.Hand;
        }

        public static void ApplyHoverEffect(Control control, Color normalColor, Color? hoverColor = null)
        {
            var over = hoverColor ?? ControlPaint.Dark(normalColor, 0.05f);
            control.Cursor = Cursors.Hand;
            control.MouseEnter += (_, _) => { if (control.Enabled) control.BackColor = over; };
            control.MouseLeave += (_, _) => control.BackColor = normalColor;
        }

        public static void ApplyRoundedCorners(Control control, int radius = CornerRadius)
        {
            void UpdateRegion(object? sender, EventArgs e)
            {
                if (control.Width <= 0 || control.Height <= 0) return;
                int scaledRadius = (int)Math.Round(radius * control.DeviceDpi / 96f);
                var diameter = Math.Min(scaledRadius * 2, Math.Min(control.Width, control.Height));
                var bounds = new Rectangle(0, 0, control.Width, control.Height);
                using var path = new GraphicsPath();
                path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
                path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
                path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
                path.CloseFigure();

                var oldRegion = control.Region;
                control.Region = new Region(path);
                oldRegion?.Dispose();
            }

            control.Resize += UpdateRegion;
            control.HandleCreated += UpdateRegion;
            if (control.IsHandleCreated) UpdateRegion(control, EventArgs.Empty);
        }

        public static Panel CreateCard(Control parent)
        {
            var panel = new Panel { BackColor = Surface, Padding = new Padding(12) };
            panel.Paint += (_, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var pen = new Pen(Border, 1);
                using var path = RoundedPath(new Rectangle(0, 0, panel.Width - 1, panel.Height - 1), CornerRadius);
                e.Graphics.DrawPath(pen, path);
            };
            return panel;
        }

        private static GraphicsPath RoundedPath(Rectangle bounds, int radius)
        {
            var diameter = Math.Max(1, Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height)));
            var path = new GraphicsPath();
            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        public static Label CreateLabel(string text, Font? font = null, Color color = default) => new()
        {
            Text = text,
            Font = font ?? FontNormal,
            ForeColor = color == default ? TextPrimary : color,
            AutoSize = true
        };

        public static TextBox CreateTextBox(bool isPassword = false)
        {
            var tb = new TextBox { Font = FontNormal, BorderStyle = BorderStyle.FixedSingle, Height = 30 };
            if (isPassword) tb.PasswordChar = '●';
            return tb;
        }

        public static DataGridView CreateDataGrid()
        {
            var dgv = new DataGridView
            {
                BackgroundColor = Surface,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = FontNormal,
                GridColor = Border,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersHeight = 38,
                RowTemplate = { Height = 34 }
            };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Primary;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font(FontFamily, 10f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Primary;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.DefaultCellStyle.SelectionBackColor = Selection;
            dgv.DefaultCellStyle.SelectionForeColor = TextPrimary;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = AlternateRow;
            return dgv;
        }
    }
}
