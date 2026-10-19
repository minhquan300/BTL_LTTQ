using Microsoft.Data.SqlClient;
using QuanLyNhaHang.Data;
using QuanLyNhaHang.Properties;
using QuanLyNhaHang.Service;
using System.Drawing.Printing;
using System.Globalization;
using System.Xml.Linq;

namespace QuanLyNhaHang.Helpers;

public sealed class InvoicePrinter : IDisposable
{
    private readonly int _orderId;
    private readonly PrintDocument _document;
    private InvoiceData? _invoice;
    private ReceiptSettings _settings = null!;
    private bool _isReprint;

    public InvoicePrinter(int invoiceId)
    {
        if (invoiceId <= 0) throw new ArgumentOutOfRangeException(nameof(invoiceId));
        _orderId = invoiceId;
        _document = new PrintDocument { DocumentName = $"HoaDon_{invoiceId}" };
        _document.PrintPage += PrintPage;
    }

    /// <summary>Prints an invoice and records the print when the optional log table exists.</summary>
    public Task<InvoicePrintResult> PrintAsync() => PrintCoreAsync(forceReprint: false);

    public Task<InvoicePrintResult> ReprintAsync()
    {
        var auth = AuthService.Instance;
        if (auth.CurrentUser == null || (!auth.IsAdmin && !auth.IsCashier))
            throw new UnauthorizedAccessException("Chỉ Thu ngân hoặc Quản lý được in lại hóa đơn.");
        return PrintCoreAsync(forceReprint: true);
    }

    private async Task<InvoicePrintResult> PrintCoreAsync(bool forceReprint)
    {
        bool logAvailable = await PrepareDocumentAsync();
        if (forceReprint && !_invoice!.OrderStatus.Equals("Da thanh toan", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Chỉ có thể in lại hóa đơn đã thanh toán.");
        _isReprint |= forceReprint;
        if (!SelectPrinterIfNeeded()) return new InvoicePrintResult(_isReprint, false, true);
        try
        {
            await Task.Run(_document.Print);
        }
        catch (Exception ex)
        {
            if (logAvailable)
            {
                try
                {
                    await LogPrintAsync(_orderId, _isReprint, false, ex.Message,
                        AuthService.Instance.CurrentUser?.EmployeeID);
                }
                catch { }
            }
            throw;
        }

        bool logSaved = false;
        if (logAvailable)
        {
            try
            {
                await LogPrintAsync(_orderId, _isReprint, true, null,
                    AuthService.Instance.CurrentUser?.EmployeeID);
                logSaved = true;
            }
            catch
            {
                // A missing or unavailable audit table must not invalidate a receipt already sent to the printer.
            }
        }
        return new InvoicePrintResult(_isReprint, logSaved);
    }

    public async Task PreviewAsync(IWin32Window owner)
    {
        await PrepareDocumentAsync();
        using var preview = new PrintPreviewDialog
        {
            Document = _document,
            Width = 800,
            Height = 700,
            StartPosition = FormStartPosition.CenterParent,
            WindowState = FormWindowState.Maximized,
            UseAntiAlias = true
        };
        preview.PrintPreviewControl.AutoZoom = true;
        preview.ShowDialog(owner);
    }

    public async Task SaveAsPdfAsync(string filePath)
    {
        await PrepareDocumentAsync();
        string? pdfPrinter = PrinterSettings.InstalledPrinters.Cast<string>()
            .FirstOrDefault(name => name.Contains("Microsoft Print to PDF", StringComparison.OrdinalIgnoreCase));
        if (pdfPrinter == null)
            throw new InvalidOperationException("Máy in Microsoft Print to PDF chưa được cài đặt trên máy này.");

        var printerSettings = _document.PrinterSettings;
        string originalPrinter = printerSettings.PrinterName;
        bool originalPrintToFile = printerSettings.PrintToFile;
        string originalFileName = printerSettings.PrintFileName;
        var originalController = _document.PrintController;
        try
        {
            printerSettings.PrinterName = pdfPrinter;
            printerSettings.PrintToFile = true;
            printerSettings.PrintFileName = filePath;
            _document.PrintController = new StandardPrintController();
            await Task.Run(_document.Print);
            if (!File.Exists(filePath)) throw new IOException("Không tạo được tệp PDF.");
        }
        finally
        {
            printerSettings.PrinterName = originalPrinter;
            printerSettings.PrintToFile = originalPrintToFile;
            printerSettings.PrintFileName = originalFileName;
            _document.PrintController = originalController;
        }
    }

    private bool SelectPrinterIfNeeded()
    {
        var installed = PrinterSettings.InstalledPrinters.Cast<string>().ToList();
        string configured = Settings.Default.InvoicePrinterName;
        string? printerName = installed.FirstOrDefault(name =>
            name.Equals(configured, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(printerName))
        {
            _document.PrinterSettings.PrinterName = printerName;
            if (_document.PrinterSettings.IsValid) return true;
        }

        using var dialog = new PrintDialog
        {
            Document = _document,
            AllowCurrentPage = false,
            AllowSelection = false,
            AllowSomePages = false,
            AllowPrintToFile = false,
            UseEXDialog = true
        };
        if (dialog.ShowDialog() != DialogResult.OK) return false;
        string selected = _document.PrinterSettings.PrinterName;
        if (string.IsNullOrWhiteSpace(selected) || !_document.PrinterSettings.IsValid)
            throw new InvalidOperationException("Máy in được chọn không còn khả dụng.");
        Settings.Default.InvoicePrinterName = selected;
        try { Settings.Default.Save(); }
        catch (Exception ex)
        {
            ValidationHelper.ShowError("Máy in đã được chọn nhưng không lưu được cài đặt. " + ex.Message);
        }
        return true;
    }

    private async Task<bool> PrepareDocumentAsync()
    {
        _settings = ReceiptSettings.Load();
        _invoice = await LoadInvoiceAsync(_orderId, _settings.VatRate);
        bool logAvailable = await IsPrintLogAvailableAsync();
        _isReprint = logAvailable && await HasPriorSuccessfulPrintAsync(_orderId);
        _document.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
        _document.DefaultPageSettings.Landscape = false;
        SetMeasuredPaperSize();
        return logAvailable;
    }

    public static string FormatMoney(decimal amount) =>
        amount.ToString("#,##0", CultureInfo.InvariantCulture) + " đ";

    private static async Task<InvoiceData> LoadInvoiceAsync(int orderId, decimal vatRate)
    {
        using var connection = DatabaseConnection.GetConnection();
        await connection.OpenAsync();
        const string headerSql = @"
            SELECT o.OrderID, o.OrderCode, o.OrderStatus, o.OrderTime, t.TableName,
                   COALESCE(e.FullName, N'—') AS CashierName,
                   COALESCE(o.PaymentMethod, p.PaymentMethod, N'') AS PaymentMethod,
                   o.CashReceived, o.ChangeAmount, o.DiscountAmount,
                   p.Amount, p.PaymentTime, p.TransactionCode
            FROM Orders o
            INNER JOIN DiningTables t ON t.TableID=o.TableID
            LEFT JOIN Employees e ON e.EmployeeID=o.EmployeeID
            OUTER APPLY (
                SELECT TOP (1) p.Amount, p.PaymentMethod, p.PaymentTime, p.TransactionCode
                FROM Payments p
                WHERE p.OrderID=o.OrderID AND p.PaymentStatus=N'Thanh cong'
                ORDER BY p.PaymentTime DESC, p.PaymentID DESC
            ) p
            WHERE o.OrderID=@OrderID";
        using var headerCommand = new SqlCommand(headerSql, connection);
        headerCommand.Parameters.Add("@OrderID", System.Data.SqlDbType.Int).Value = orderId;
        InvoiceData? invoice = null;
        using (var reader = await headerCommand.ExecuteReaderAsync())
        {
            if (await reader.ReadAsync())
            {
                invoice = new InvoiceData
                {
                    Id = reader.GetInt32(0),
                    Code = reader.IsDBNull(1) ? $"HD{orderId:D5}" : reader.GetString(1),
                    OrderStatus = reader.GetString(2),
                    OrderTime = reader.GetDateTime(3),
                    TableName = reader.GetString(4),
                    CashierName = reader.GetString(5),
                    PaymentMethod = reader.GetString(6),
                    CashReceived = reader.GetDecimal(7),
                    ChangeAmount = reader.GetDecimal(8),
                    Discount = reader.GetDecimal(9),
                    PaidAmount = reader.IsDBNull(10) ? null : reader.GetDecimal(10),
                    PaidAt = reader.IsDBNull(11) ? null : reader.GetDateTime(11),
                    TransactionCode = reader.IsDBNull(12) ? null : reader.GetString(12)
                };
            }
        }
        if (invoice == null) throw new InvalidOperationException("Không tìm thấy hóa đơn cần in.");

        const string detailsSql = @"
            SELECT m.ItemName, od.Quantity, od.UnitPrice, od.Note
            FROM OrderDetails od
            INNER JOIN MenuItems m ON m.MenuItemID=od.MenuItemID
            WHERE od.OrderID=@OrderID AND od.ItemStatus<>N'Da huy'
            ORDER BY od.OrderedAt, od.OrderDetailID";
        using var detailsCommand = new SqlCommand(detailsSql, connection);
        detailsCommand.Parameters.Add("@OrderID", System.Data.SqlDbType.Int).Value = orderId;
        using (var reader = await detailsCommand.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                invoice.Items.Add(new InvoiceLine
                {
                    Name = reader.GetString(0),
                    Quantity = reader.GetInt32(1),
                    UnitPrice = reader.GetDecimal(2),
                    Note = reader.IsDBNull(3) ? null : reader.GetString(3)
                });
            }
        }
        invoice.Subtotal = invoice.Items.Sum(item => item.Quantity * item.UnitPrice);
        invoice.Discount = Math.Clamp(invoice.Discount, 0, invoice.Subtotal);
        invoice.Vat = Math.Round(Math.Max(0, invoice.Subtotal - invoice.Discount) * vatRate / 100m, 0);
        invoice.Total = invoice.PaidAmount ?? invoice.Subtotal - invoice.Discount + invoice.Vat;
        return invoice;
    }

    private void SetMeasuredPaperSize()
    {
        using var bitmap = new Bitmap(1, 1);
        bitmap.SetResolution(203, 203);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.PageUnit = GraphicsUnit.Pixel;
        float widthPixels = GetContentWidthPixels(graphics.DpiX);
        float contentHeight = LayoutReceipt(graphics, widthPixels, false, 0);
        int pageWidth = MmToHundredthsInch(_settings.PaperWidthMm);
        int pageHeight = Math.Max(MmToHundredthsInch(45), (int)Math.Ceiling(contentHeight / graphics.DpiY * 100f) + MmToHundredthsInch(4));
        _document.DefaultPageSettings.PaperSize = new PaperSize($"Thermal {_settings.PaperWidthMm}mm", pageWidth, pageHeight);
    }

    private void PrintPage(object? sender, PrintPageEventArgs e)
    {
        if (_invoice == null) throw new InvalidOperationException("Dữ liệu hóa đơn chưa được nạp.");
        var graphics = e.Graphics!;
        graphics.PageUnit = GraphicsUnit.Pixel;
        float width = GetContentWidthPixels(graphics.DpiX);
        float pageWidthPixels = _settings.PaperWidthMm / 25.4f * graphics.DpiX;
        float x = Math.Max(0, (pageWidthPixels - width) / 2f);
        LayoutReceipt(graphics, width, true, x);
        e.HasMorePages = false;
    }

    private float LayoutReceipt(Graphics graphics, float width, bool draw, float x)
    {
        var invoice = _invoice!;
        float y = MmToPixels(2, graphics.DpiY);
        float small = _settings.PaperWidthMm == 58 ? 7.2f : 8.2f;
        using var normal = new Font("Segoe UI", small, FontStyle.Regular, GraphicsUnit.Point);
        using var bold = new Font("Segoe UI", small, FontStyle.Bold, GraphicsUnit.Point);
        using var title = new Font("Segoe UI", small + 2.2f, FontStyle.Bold, GraphicsUnit.Point);
        using var totalFont = new Font("Segoe UI", small + 2.5f, FontStyle.Bold, GraphicsUnit.Point);
        using var italic = new Font("Segoe UI", small - 0.2f, FontStyle.Italic, GraphicsUnit.Point);
        var center = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
        var right = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Near };

        void Text(string value, Font font, StringFormat? format = null, float gap = 1.5f)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            var measured = graphics.MeasureString(value, font, new SizeF(width, 10000), format ?? StringFormat.GenericDefault);
            if (draw) graphics.DrawString(value, font, Brushes.Black, new RectangleF(x, y, width, measured.Height + 2), format ?? StringFormat.GenericDefault);
            y += measured.Height + MmToPixels(gap, graphics.DpiY);
        }
        void Rule()
        {
            float lineHeight = MmToPixels(2, graphics.DpiY);
            if (draw) graphics.DrawLine(Pens.Black, x, y + lineHeight / 2, x + width, y + lineHeight / 2);
            y += lineHeight;
        }
        void Pair(string label, string value, Font font, bool isBold = false)
        {
            using var leftFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };
            using var rightFormat = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Near };
            float lineHeight = Math.Max(graphics.MeasureString(label, font).Height, graphics.MeasureString(value, font).Height);
            if (draw)
            {
                graphics.DrawString(label, font, Brushes.Black, new RectangleF(x, y, width * 0.62f, lineHeight + 2), leftFormat);
                graphics.DrawString(value, font, Brushes.Black, new RectangleF(x + width * 0.48f, y, width * 0.52f, lineHeight + 2), rightFormat);
            }
            y += lineHeight + MmToPixels(1, graphics.DpiY);
        }

        if (_settings.LogoPath is { Length: > 0 } logoPath && File.Exists(logoPath))
        {
            try
            {
                using var logo = Image.FromFile(logoPath);
                float logoHeight = Math.Min(MmToPixels(18, graphics.DpiY), width * 0.22f);
                float logoWidth = logoHeight * logo.Width / Math.Max(1f, logo.Height);
                if (draw) graphics.DrawImage(logo, x + (width - logoWidth) / 2f, y, logoWidth, logoHeight);
                y += logoHeight + MmToPixels(1, graphics.DpiY);
            }
            catch { /* Ignore an invalid optional logo and still print the invoice. */ }
        }
        Text(_settings.RestaurantName, title, center, 0.5f);
        Text(_settings.Address, normal, center, 0.5f);
        if (!string.IsNullOrWhiteSpace(_settings.Phone)) Text("SĐT: " + _settings.Phone, normal, center, 0.5f);
        if (!string.IsNullOrWhiteSpace(_settings.TaxId)) Text("MST: " + _settings.TaxId, normal, center, 1f);
        Rule();
        Text("HÓA ĐƠN THANH TOÁN", bold, center, 1f);
        if (_isReprint) Text("BẢN IN LẠI", bold, center, 1f);
        Pair("Số HĐ:", invoice.Code, normal);
        Pair("Ngày giờ:", (invoice.PaidAt ?? invoice.OrderTime).ToString("dd/MM/yyyy HH:mm", CultureInfo.GetCultureInfo("vi-VN")), normal);
        Pair("Bàn:", invoice.TableName, normal);
        Pair("Thu ngân:", invoice.CashierName, normal);
        Rule();

        float qtyWidth = width * 0.12f;
        float unitWidth = width * 0.26f;
        float amountWidth = width * 0.28f;
        float nameWidth = width - qtyWidth - unitWidth - amountWidth;
        using var left = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };
        using var middle = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
        using var rightAligned = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Near };
        float headingsHeight = new[] { "Món", "SL", "Đơn giá", "T.Tiền" }.Max(s => graphics.MeasureString(s, bold).Height);
        if (draw)
        {
            graphics.DrawString("Món", bold, Brushes.Black, new RectangleF(x, y, nameWidth, headingsHeight), left);
            graphics.DrawString("SL", bold, Brushes.Black, new RectangleF(x + nameWidth, y, qtyWidth, headingsHeight), middle);
            graphics.DrawString("Đơn giá", bold, Brushes.Black, new RectangleF(x + nameWidth + qtyWidth, y, unitWidth, headingsHeight), rightAligned);
            graphics.DrawString("T.Tiền", bold, Brushes.Black, new RectangleF(x + nameWidth + qtyWidth + unitWidth, y, amountWidth, headingsHeight), rightAligned);
        }
        y += headingsHeight + MmToPixels(1, graphics.DpiY);
        foreach (var item in invoice.Items)
        {
            var nameSize = graphics.MeasureString(item.Name, normal, new SizeF(width, 10000));
            if (draw) graphics.DrawString(item.Name, normal, Brushes.Black, new RectangleF(x, y, width, nameSize.Height + 2));
            y += nameSize.Height + MmToPixels(0.5f, graphics.DpiY);
            float rowHeight = Math.Max(graphics.MeasureString(item.Quantity.ToString(CultureInfo.InvariantCulture), normal).Height,
                Math.Max(graphics.MeasureString(FormatMoney(item.UnitPrice), normal, new SizeF(unitWidth, 10000)).Height,
                    graphics.MeasureString(FormatMoney(item.Quantity * item.UnitPrice), normal, new SizeF(amountWidth, 10000)).Height));
            if (draw)
            {
                graphics.DrawString(item.Quantity.ToString(CultureInfo.InvariantCulture), normal, Brushes.Black,
                    new RectangleF(x + nameWidth, y, qtyWidth, rowHeight), middle);
                graphics.DrawString(FormatMoney(item.UnitPrice), normal, Brushes.Black,
                    new RectangleF(x + nameWidth + qtyWidth, y, unitWidth, rowHeight), rightAligned);
                graphics.DrawString(FormatMoney(item.Quantity * item.UnitPrice), normal, Brushes.Black,
                    new RectangleF(x + nameWidth + qtyWidth + unitWidth, y, amountWidth, rowHeight), rightAligned);
            }
            y += rowHeight + MmToPixels(0.8f, graphics.DpiY);
            if (!string.IsNullOrWhiteSpace(item.Note))
            {
                string note = "Ghi chú: " + item.Note.Trim();
                var noteSize = graphics.MeasureString(note, italic, new SizeF(width - MmToPixels(2, graphics.DpiX), 10000));
                if (draw) graphics.DrawString(note, italic, Brushes.Black,
                    new RectangleF(x + MmToPixels(2, graphics.DpiX), y, width - MmToPixels(2, graphics.DpiX), noteSize.Height + 2));
                y += noteSize.Height + MmToPixels(0.8f, graphics.DpiY);
            }
        }
        Rule();
        Pair("Tạm tính", FormatMoney(invoice.Subtotal), normal);
        if (invoice.Discount > 0) Pair("Giảm giá", "- " + FormatMoney(invoice.Discount), normal);
        if (invoice.Vat > 0) Pair($"VAT ({_settings.VatRate:0.##}%)", FormatMoney(invoice.Vat), normal);
        Pair("TỔNG CỘNG", FormatMoney(invoice.Total), totalFont, true);
        Rule();
        Pair("Thanh toán:", FormatPaymentMethod(invoice.PaymentMethod), normal);
        if (invoice.PaymentMethod.Equals("Tien mat", StringComparison.OrdinalIgnoreCase))
        {
            Pair("Khách đưa:", FormatMoney(invoice.CashReceived), normal);
            Pair("Tiền thừa:", FormatMoney(invoice.ChangeAmount), normal);
        }
        else if (invoice.PaymentMethod is "The" or "Card")
        {
            if (!string.IsNullOrWhiteSpace(invoice.TransactionCode))
                Pair("Mã chuẩn chi (4 cuối):", MaskReference(invoice.TransactionCode), normal);
        }
        else if (!string.IsNullOrWhiteSpace(invoice.TransactionCode))
        {
            Pair("Mã giao dịch:", MaskReference(invoice.TransactionCode), normal);
        }
        Rule();
        Text("CẢM ƠN QUÝ KHÁCH!", bold, center, 0.5f);
        Text("Hẹn gặp lại quý khách", normal, center, 1f);
        if (!string.IsNullOrWhiteSpace(_settings.QrImagePath) && File.Exists(_settings.QrImagePath))
        {
            try
            {
                using var qr = Image.FromFile(_settings.QrImagePath);
                float qrSize = Math.Min(width * 0.52f, MmToPixels(35, graphics.DpiX));
                if (draw) graphics.DrawImage(qr, x + (width - qrSize) / 2, y, qrSize, qrSize);
                y += qrSize + MmToPixels(1, graphics.DpiY);
                if (!string.IsNullOrWhiteSpace(_settings.QrCaption)) Text(_settings.QrCaption, normal, center, 1f);
            }
            catch { /* Ignore an invalid optional QR image. */ }
        }
        return y + MmToPixels(2, graphics.DpiY);
    }

    private float GetContentWidthPixels(float dpiX)
    {
        float contentMm = _settings.PaperWidthMm == 58 ? 54 : 72;
        return MmToPixels(contentMm, dpiX);
    }

    private static string FormatPaymentMethod(string method) => method switch
    {
        "Tien mat" => "Tiền mặt",
        "Chuyen khoan" => "Chuyển khoản",
        "The" or "Card" => "Thẻ",
        _ => string.IsNullOrWhiteSpace(method) ? "Chưa ghi nhận" : method
    };

    private static string MaskReference(string value)
    {
        string trimmed = value.Trim();
        string lastFour = trimmed.Length > 4 ? trimmed[^4..] : trimmed;
        return "•••• " + lastFour;
    }

    private static int MmToHundredthsInch(int mm) => (int)Math.Round(mm / 25.4 * 100);
    private static float MmToPixels(float mm, float dpi) => mm / 25.4f * dpi;

    private static async Task<bool> IsPrintLogAvailableAsync()
    {
        using var connection = DatabaseConnection.GetConnection();
        await connection.OpenAsync();
        using var command = new SqlCommand("SELECT CASE WHEN OBJECT_ID(N'dbo.InvoicePrintLog', N'U') IS NULL THEN 0 ELSE 1 END", connection);
        return Convert.ToInt32(await command.ExecuteScalarAsync(), CultureInfo.InvariantCulture) == 1;
    }

    private static async Task<bool> HasPriorSuccessfulPrintAsync(int orderId)
    {
        using var connection = DatabaseConnection.GetConnection();
        await connection.OpenAsync();
        using var command = new SqlCommand(@"SELECT CASE WHEN EXISTS
            (SELECT 1 FROM dbo.InvoicePrintLog WHERE OrderID=@OrderID AND PrintSucceeded=1) THEN 1 ELSE 0 END", connection);
        command.Parameters.Add("@OrderID", System.Data.SqlDbType.Int).Value = orderId;
        return Convert.ToInt32(await command.ExecuteScalarAsync(), CultureInfo.InvariantCulture) == 1;
    }

    private static async Task LogPrintAsync(int orderId, bool isReprint, bool succeeded, string? error, int? employeeId)
    {
        using var connection = DatabaseConnection.GetConnection();
        await connection.OpenAsync();
        using var command = new SqlCommand(@"INSERT INTO dbo.InvoicePrintLog
            (OrderID, EmployeeID, PrintedAt, IsReprint, PrintSucceeded, ErrorMessage)
            VALUES (@OrderID, @EmployeeID, SYSDATETIME(), @IsReprint, @Succeeded, @Error)", connection);
        command.Parameters.Add("@OrderID", System.Data.SqlDbType.Int).Value = orderId;
        command.Parameters.Add("@EmployeeID", System.Data.SqlDbType.Int).Value = (object?)employeeId ?? DBNull.Value;
        command.Parameters.Add("@IsReprint", System.Data.SqlDbType.Bit).Value = isReprint;
        command.Parameters.Add("@Succeeded", System.Data.SqlDbType.Bit).Value = succeeded;
        command.Parameters.Add("@Error", System.Data.SqlDbType.NVarChar, 500).Value = (object?)error ?? DBNull.Value;
        await command.ExecuteNonQueryAsync();
    }

    private static ReceiptSettingsConfig ReadConfig()
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (string path in new[]
        {
            Path.Combine(AppContext.BaseDirectory, "App.config"),
            (Environment.ProcessPath ?? string.Empty) + ".config"
        }.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!File.Exists(path)) continue;
            try
            {
                var document = XDocument.Load(path);
                foreach (var element in document.Root?.Element("appSettings")?.Elements("add") ?? Enumerable.Empty<XElement>())
                {
                    var key = (string?)element.Attribute("key");
                    var value = (string?)element.Attribute("value");
                    if (key != null && value != null) values[key] = value;
                }
            }
            catch { }
        }
        string Get(string key, string fallback = "") => values.TryGetValue(key, out var value) ? value : fallback;
        int.TryParse(Get("InvoicePaperWidthMm"), NumberStyles.Integer, CultureInfo.InvariantCulture, out int width);
        decimal.TryParse(Get("InvoiceVatRate"), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal vat);
        return new ReceiptSettingsConfig(
            Get("InvoiceRestaurantName", "NHÀ HÀNG"), Get("InvoiceAddress"), Get("InvoicePhone"),
            Get("InvoiceTaxId"), Get("InvoiceLogoPath"), Get("InvoiceQrImagePath"), Get("InvoiceQrCaption"),
            width is 58 or 80 ? width : 80, Math.Clamp(vat, 0, 100));
    }

    public void Dispose() => _document.Dispose();

    private sealed class InvoiceData
    {
        public int Id { get; init; }
        public string Code { get; init; } = string.Empty;
        public string OrderStatus { get; init; } = string.Empty;
        public DateTime OrderTime { get; init; }
        public string TableName { get; init; } = string.Empty;
        public string CashierName { get; init; } = string.Empty;
        public string PaymentMethod { get; init; } = string.Empty;
        public decimal CashReceived { get; init; }
        public decimal ChangeAmount { get; init; }
        public decimal Discount { get; set; }
        public decimal? PaidAmount { get; init; }
        public DateTime? PaidAt { get; init; }
        public string? TransactionCode { get; init; }
        public List<InvoiceLine> Items { get; } = new();
        public decimal Subtotal { get; set; }
        public decimal Vat { get; set; }
        public decimal Total { get; set; }
    }

    private sealed class InvoiceLine
    {
        public string Name { get; init; } = string.Empty;
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public string? Note { get; init; }
    }

    private sealed record ReceiptSettingsConfig(string RestaurantName, string Address, string Phone, string TaxId,
        string LogoPath, string QrImagePath, string QrCaption, int PaperWidthMm, decimal VatRate);

    private sealed class ReceiptSettings
    {
        public string RestaurantName { get; private init; } = "NHÀ HÀNG";
        public string Address { get; private init; } = string.Empty;
        public string Phone { get; private init; } = string.Empty;
        public string TaxId { get; private init; } = string.Empty;
        public string LogoPath { get; private init; } = string.Empty;
        public string QrImagePath { get; private init; } = string.Empty;
        public string QrCaption { get; private init; } = string.Empty;
        public int PaperWidthMm { get; private init; } = 80;
        public decimal VatRate { get; private init; }

        public static ReceiptSettings Load()
        {
            var config = ReadConfig();
            return new ReceiptSettings
            {
                RestaurantName = config.RestaurantName,
                Address = config.Address,
                Phone = config.Phone,
                TaxId = config.TaxId,
                LogoPath = ResolvePath(config.LogoPath),
                QrImagePath = ResolvePath(config.QrImagePath),
                QrCaption = config.QrCaption,
                PaperWidthMm = config.PaperWidthMm,
                VatRate = config.VatRate
            };
        }

        private static string ResolvePath(string path) => string.IsNullOrWhiteSpace(path)
            ? string.Empty
            : Path.GetFullPath(Path.IsPathRooted(path) ? path : Path.Combine(AppContext.BaseDirectory, path));
    }

    public sealed record InvoicePrintResult(bool IsReprint, bool PrintLogSaved, bool Cancelled = false);
}
