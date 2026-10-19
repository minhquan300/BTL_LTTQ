using QuanLyNhaHang.Helpers;

namespace QuanLyNhaHang.Forms;

internal enum PrinterRecoveryAction
{
    Close,
    Retry,
    SavePdf
}

internal sealed class PrinterRecoveryDialog : Form
{
    public PrinterRecoveryAction Action { get; private set; } = PrinterRecoveryAction.Close;

    public PrinterRecoveryDialog(string details)
    {
        UITheme.ConfigureDpi(this);
        Text = "Không thể in hóa đơn";
        ClientSize = new Size(510, 220);
        MinimumSize = Size;
        MaximumSize = Size;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        BackColor = UITheme.Surface;
        Font = UITheme.FontNormal;

        var heading = new Label
        {
            Text = "Máy in đang gặp sự cố",
            Font = new Font(UITheme.FontFamily, 13, FontStyle.Bold),
            ForeColor = UITheme.TextPrimary,
            Location = new Point(22, 18),
            Size = new Size(466, 28)
        };
        var message = new Label
        {
            Text = "Kiểm tra máy in, giấy và kết nối rồi thử lại. Bạn cũng có thể lưu hóa đơn PDF.\n\n" + details,
            ForeColor = UITheme.TextMuted,
            Location = new Point(22, 52),
            Size = new Size(466, 88),
            AutoEllipsis = true
        };
        var close = UITheme.CreateButton("Đóng", UITheme.AlternateRow, UITheme.TextPrimary);
        close.Size = new Size(76, 38);
        close.Click += (_, _) => { Action = PrinterRecoveryAction.Close; DialogResult = DialogResult.Cancel; };
        var pdf = UITheme.CreateButton("Lưu PDF thay thế", UITheme.Success);
        pdf.Size = new Size(142, 38);
        pdf.Click += (_, _) => { Action = PrinterRecoveryAction.SavePdf; DialogResult = DialogResult.Yes; };
        var retry = UITheme.CreateButton("Thử lại", UITheme.Primary);
        retry.Size = new Size(84, 38);
        retry.Click += (_, _) => { Action = PrinterRecoveryAction.Retry; DialogResult = DialogResult.Retry; };

        // Place the three actions with enough spacing inside the fixed dialog.
        close.Location = new Point(96, 164);
        pdf.Location = new Point(180, 164);
        retry.Location = new Point(330, 164);
        Controls.AddRange(new Control[] { heading, message, close, pdf, retry });
        CancelButton = close;
        UITheme.EnableDoubleBuffering(this);
    }
}
