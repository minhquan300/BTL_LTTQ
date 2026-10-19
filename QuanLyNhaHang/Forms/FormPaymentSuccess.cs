using QuanLyNhaHang.Helpers;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Properties;
using System.Diagnostics;

namespace QuanLyNhaHang.Forms;

public sealed class FormPaymentSuccess : Form
{
    private readonly Order _order;
    private readonly decimal _amount;
    private readonly string _method;
    private readonly decimal _discountPct;
    private readonly decimal _discountAmount;
    private readonly decimal _changeAmount;
    private readonly DateTime _paidAt;
    private CheckBox chkAutoPrint = null!;

    public FormPaymentSuccess(Order order, decimal amount, string method, decimal discountPct,
        decimal discountAmount, decimal changeAmount = 0, DateTime? paidAt = null)
    {
        _order = order;
        _amount = amount;
        _method = method;
        _discountPct = discountPct;
        _discountAmount = discountAmount;
        _changeAmount = changeAmount;
        _paidAt = paidAt ?? DateTime.Now;
        UITheme.ConfigureDpi(this);
        BuildUi();
        UITheme.EnableDoubleBuffering(this);
    }

    private void BuildUi()
    {
        Text = "Thanh toán thành công";
        ClientSize = new Size(500, 520);
        MinimumSize = new Size(500, 520);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ControlBox = false;
        ShowInTaskbar = false;
        BackColor = UITheme.Surface;
        Font = UITheme.FontNormal;

        var check = new Label
        {
            Text = "✓", Font = new Font(UITheme.FontFamily, 46, FontStyle.Bold),
            ForeColor = Color.White, BackColor = UITheme.Success, TextAlign = ContentAlignment.MiddleCenter,
            Size = new Size(88, 88), Location = new Point(206, 20)
        };
        UITheme.ApplyRoundedCorners(check, 44);

        var title = new Label
        {
            Text = "Thanh toán thành công", Font = new Font(UITheme.FontFamily, 19, FontStyle.Bold),
            ForeColor = UITheme.TextPrimary, TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(24, 116), Size = new Size(452, 36)
        };
        var amount = new Label
        {
            Text = InvoicePrinter.FormatMoney(_amount), Font = new Font(UITheme.FontFamily, 24, FontStyle.Bold),
            ForeColor = UITheme.Success, TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(24, 154), Size = new Size(452, 42)
        };
        string displayMethod = _method switch
        {
            "Tien mat" => "Tiền mặt",
            "Chuyen khoan" => "Chuyển khoản QR",
            "The" or "Card" => "Thẻ",
            _ => _method
        };
        string changeLine = _changeAmount > 0 ? $"\nTiền thừa: {_changeAmount:N0} đ" : string.Empty;
        var details = new Label
        {
            Text = $"Mã hóa đơn: HD{_order.OrderID:D5}\nPhương thức: {displayMethod}{changeLine}\nGiờ thanh toán: {_paidAt:dd/MM/yyyy HH:mm:ss}",
            Font = UITheme.FontNormal, ForeColor = UITheme.TextMuted,
            TextAlign = ContentAlignment.MiddleCenter, Location = new Point(24, 204), Size = new Size(452, 88)
        };

        chkAutoPrint = new CheckBox
        {
            Text = "Tự động in sau khi thanh toán",
            Checked = Settings.Default.AutoPrintAfterPayment,
            AutoSize = true,
            Font = UITheme.FontNormal,
            ForeColor = UITheme.TextPrimary,
            Location = new Point(86, 310),
            Cursor = Cursors.Hand
        };
        chkAutoPrint.CheckedChanged += (_, _) =>
        {
            Settings.Default.AutoPrintAfterPayment = chkAutoPrint.Checked;
            try { Settings.Default.Save(); }
            catch (Exception ex) { ValidationHelper.ShowError("Không lưu được cài đặt tự động in. " + ex.Message); }
        };

        var print = UITheme.CreateButton("In hóa đơn", UITheme.Primary);
        print.Size = new Size(108, 40);
        print.Location = new Point(24, 414);
        print.Click += async (_, _) => await PrintReceiptAsync();

        var preview = UITheme.CreateButton("Xem trước", UITheme.AlternateRow, UITheme.TextPrimary);
        preview.Size = new Size(100, 40);
        preview.Location = new Point(140, 414);
        preview.Click += async (_, _) =>
        {
            try
            {
                using var printer = new InvoicePrinter(_order.OrderID);
                await printer.PreviewAsync(this);
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError("Không thể xem trước hóa đơn. " + ex.Message);
            }
        };

        var savePdf = UITheme.CreateButton("Lưu PDF", UITheme.AlternateRow, UITheme.TextPrimary);
        savePdf.Size = new Size(92, 40);
        savePdf.Location = new Point(248, 414);
        savePdf.Click += async (_, _) => await SaveReceiptPdfAsync();

        var close = UITheme.CreateButton("Đóng", UITheme.TextMuted);
        close.Size = new Size(104, 40);
        close.Location = new Point(348, 414);
        close.Click += (_, _) => DialogResult = DialogResult.OK;

        var footer = new Panel { Dock = DockStyle.Bottom, Height = 64, BackColor = UITheme.Surface };
        footer.Paint += (_, e) =>
        {
            using var pen = new Pen(UITheme.Border);
            e.Graphics.DrawLine(pen, 0, 0, footer.Width, 0);
        };
        foreach (var button in new[] { print, preview, savePdf, close })
        {
            button.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            footer.Controls.Add(button);
        }
        void LayoutFooter(object? sender, EventArgs e)
        {
            int right = footer.ClientSize.Width - 16;
            close.Location = new Point(right - close.Width, 12); right = close.Left - 8;
            savePdf.Location = new Point(right - savePdf.Width, 12); right = savePdf.Left - 8;
            preview.Location = new Point(right - preview.Width, 12); right = preview.Left - 8;
            print.Location = new Point(right - print.Width, 12);
        }
        footer.Resize += LayoutFooter;
        LayoutFooter(footer, EventArgs.Empty);

        Controls.AddRange(new Control[] { check, title, amount, details, chkAutoPrint, footer });
        footer.BringToFront();
        CancelButton = close;
        FormClosing += (_, _) => DialogResult = DialogResult.OK;
        Shown += (_, _) =>
        {
            if (chkAutoPrint.Checked)
                BeginInvoke(new Action(async () => await PrintReceiptAsync()));
        };
    }

    private async Task PrintReceiptAsync()
    {
        UseWaitCursor = true;
        try
        {
            while (true)
            {
                try
                {
                    using var printer = new InvoicePrinter(_order.OrderID);
                    var result = await printer.PrintAsync();
                    if (result.Cancelled) return;
                    string message = result.IsReprint
                        ? "Đã gửi bản in lại tới máy in mặc định."
                        : "Đã gửi hóa đơn tới máy in mặc định.";
                    if (!result.PrintLogSaved)
                        message += " Chưa lưu được nhật ký in; hãy cài bảng InvoicePrintLog bằng script đi kèm.";
                    ValidationHelper.ShowSuccess(message);
                    return;
                }
                catch (Exception ex)
                {
                    using var recovery = new PrinterRecoveryDialog(ex.Message);
                    recovery.ShowDialog(this);
                    if (recovery.Action == PrinterRecoveryAction.Retry) continue;
                    if (recovery.Action == PrinterRecoveryAction.SavePdf)
                        await SaveReceiptPdfAsync();
                    return;
                }
            }
        }
        finally
        {
            UseWaitCursor = false;
        }
    }

    private async Task SaveReceiptPdfAsync()
    {
        string folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "QuanLyNhaHang", "HoaDon");
        Directory.CreateDirectory(folder);
        using var dialog = new SaveFileDialog
        {
            Title = "Lưu hóa đơn PDF",
            Filter = "Tệp PDF (*.pdf)|*.pdf",
            InitialDirectory = folder,
            FileName = $"HoaDon_HD{_order.OrderID:D5}_{DateTime.Now:yyyyMMdd_HHmm}.pdf",
            AddExtension = true,
            DefaultExt = "pdf",
            RestoreDirectory = true,
            OverwritePrompt = true
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            UseWaitCursor = true;
            using var printer = new InvoicePrinter(_order.OrderID);
            await printer.SaveAsPdfAsync(dialog.FileName);
            using var saved = new PdfSavedDialog(folder, dialog.FileName);
            saved.ShowDialog(this);
        }
        catch (Exception ex)
        {
            ValidationHelper.ShowError("Không thể lưu hóa đơn PDF. " + ex.Message);
        }
        finally
        {
            UseWaitCursor = false;
        }
    }

    internal sealed class PdfSavedDialog : Form
    {
        private readonly string _folder;

        public PdfSavedDialog(string folder, string filePath)
        {
            UITheme.ConfigureDpi(this);
            _folder = folder;
            Text = "Đã lưu hóa đơn PDF";
            ClientSize = new Size(440, 170);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            BackColor = UITheme.Surface;
            Font = UITheme.FontNormal;
            var message = new Label
            {
                Text = $"Đã lưu hóa đơn thành công:\n{Path.GetFileName(filePath)}",
                Location = new Point(20, 20),
                Size = new Size(400, 54),
                ForeColor = UITheme.TextPrimary
            };
            var close = UITheme.CreateButton("Đóng", UITheme.AlternateRow, UITheme.TextPrimary);
            close.Size = new Size(84, 38);
            close.Click += (_, _) => Close();
            var open = UITheme.CreateButton("Mở thư mục chứa file", UITheme.Primary);
            open.Size = new Size(170, 38);
            open.Click += (_, _) =>
            {
                try
                {
                    Process.Start(new ProcessStartInfo(_folder) { UseShellExecute = true });
                    Close();
                }
                catch (Exception ex)
                {
                    ValidationHelper.ShowError("Không thể mở thư mục chứa hóa đơn. " + ex.Message);
                }
            };
            // Right-aligned actions inside the fixed dialog.
            close.Location = new Point(174, 108);
            open.Location = new Point(266, 108);
            Controls.AddRange(new Control[] { message, close, open });
            UITheme.EnableDoubleBuffering(this);
        }
    }
}
