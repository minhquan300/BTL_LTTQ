using System.Drawing.Text;

namespace QuanLyNhaHang.Controls;

/// <summary>Transparent label that paints a subtle one-pixel shadow under its text.</summary>
public sealed class ShadowLabel : Label
{
    public ShadowLabel()
    {
        SetStyle(ControlStyles.UserPaint |
                 ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.SupportsTransparentBackColor, true);
        UpdateStyles();
        DoubleBuffered = true;
        BackColor = Color.Transparent;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        if (string.IsNullOrEmpty(Text)) return;
        e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        var rectangle = ClientRectangle;
        rectangle.Inflate(-Padding.Left, -Padding.Top);
        if (rectangle.Width <= 0 || rectangle.Height <= 0) return;

        using var format = new StringFormat(StringFormat.GenericTypographic)
        {
            Alignment = GetHorizontalAlignment(TextAlign),
            LineAlignment = GetVerticalAlignment(TextAlign),
            Trimming = StringTrimming.EllipsisCharacter,
            FormatFlags = StringFormatFlags.LineLimit,
        };
        using var shadowBrush = new SolidBrush(Color.FromArgb(80, Color.Black));
        using var textBrush = new SolidBrush(ForeColor);
        var shadowBounds = rectangle;
        int shadowOffset = Math.Max(1, (int)Math.Round(DeviceDpi / 96f));
        shadowBounds.Offset(shadowOffset, shadowOffset);
        e.Graphics.DrawString(Text, Font, shadowBrush, shadowBounds, format);
        e.Graphics.DrawString(Text, Font, textBrush, rectangle, format);
    }

    private static StringAlignment GetHorizontalAlignment(ContentAlignment alignment) => alignment switch
    {
        ContentAlignment.TopCenter or ContentAlignment.MiddleCenter or ContentAlignment.BottomCenter => StringAlignment.Center,
        ContentAlignment.TopRight or ContentAlignment.MiddleRight or ContentAlignment.BottomRight => StringAlignment.Far,
        _ => StringAlignment.Near,
    };

    private static StringAlignment GetVerticalAlignment(ContentAlignment alignment) => alignment switch
    {
        ContentAlignment.MiddleLeft or ContentAlignment.MiddleCenter or ContentAlignment.MiddleRight => StringAlignment.Center,
        ContentAlignment.BottomLeft or ContentAlignment.BottomCenter or ContentAlignment.BottomRight => StringAlignment.Far,
        _ => StringAlignment.Near,
    };
}
