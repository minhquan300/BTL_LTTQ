using QuanLyNhaHang.Helpers;
using QuanLyNhaHang.Models;
using System.Drawing.Printing;

namespace QuanLyNhaHang.Forms
{
    public class FormReceiptPreview : Form
    {
        private readonly Order _order;
        private readonly decimal _amount;
        private readonly string _method;
        private readonly decimal _discountPct;
        private readonly decimal? _discountAmount;
        private string _receiptText = string.Empty;
        private PrintDocument? _printDocument;

        public FormReceiptPreview(Order order, decimal amount, string method, decimal discountPct = 0, decimal? discountAmount = null)
        {
            _order = order;
            _amount = amount;
            _method = method switch
            {
                "Tien mat" => "Tiền mặt",
                "Chuyen khoan" => "Chuyển khoản QR",
                "The" or "Card" => "Thẻ",
                _ => method
            };
            _discountPct = discountPct;
            _discountAmount = discountAmount;
            UITheme.ConfigureDpi(this);
            BuildUI();
            UITheme.EnableDoubleBuffering(this);
        }

        private void BuildUI()
        {
            this.Text = $"Hóa đơn #{_order.OrderID} - {_order.TableName}";
            this.ClientSize = new Size(420, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 45, BackColor = UITheme.Primary };
            new Label
            {
                Text = "🧾 HÓA ĐƠN THANH TOÁN",
                Font = UITheme.FontHeader,
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            }.Also(l => pnlTop.Controls.Add(l));
            this.Controls.Add(pnlTop);

            var rtb = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = UITheme.Surface,
                BorderStyle = BorderStyle.None,
                Font = UITheme.FontNormal,
                Padding = new Padding(15)
            };
            this.Controls.Add(rtb);

            // Construct receipt text
            decimal gross = _order.Details.Where(d => d.ItemStatus != "Da huy").Sum(d => d.SubTotal);
            decimal discountAmt = _discountAmount ?? Math.Round(gross * _discountPct / 100, 0);

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("         QUẢN LÝ NHÀ HÀNG");
            sb.AppendLine("      123 Đường Ẩm Thực, Quận 1");
            sb.AppendLine("         Hotline: 1900 8888");
            sb.AppendLine("----------------------------------------");
            sb.AppendLine($"Hóa đơn số: #{_order.OrderID,-10} Bàn: {_order.TableName}");
            sb.AppendLine($"Thời gian: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine($"Thu ngân : {_order.EmployeeName}");
            sb.AppendLine("========================================");
            sb.AppendLine(string.Format("{0,-18} {1,4} {2,8} {3,8}", "Tên món", "SL", "Đ.Giá", "T.Tiền"));
            sb.AppendLine("----------------------------------------");

            foreach (var d in _order.Details.Where(d => d.ItemStatus != "Da huy"))
            {
                string name = d.ItemName.Length > 18 ? d.ItemName.Substring(0, 15) + "..." : d.ItemName;
                sb.AppendLine(string.Format("{0,-18} {1,4} {2,8:N0} {3,8:N0}", name, d.Quantity, d.UnitPrice, d.SubTotal));
            }

            sb.AppendLine("----------------------------------------");
            sb.AppendLine(string.Format("{0,-25} {1,14:N0} đ", "Tổng tiền món:", gross));
            if (_discountPct > 0)
            {
                string discountLabel = _discountAmount.HasValue ? "Giảm giá:" : $"Giảm giá ({_discountPct:0.##}%):";
                sb.AppendLine(string.Format("{0,-25} -{1,13:N0} đ", discountLabel, discountAmt));
            }
            sb.AppendLine(string.Format("{0,-25} {1,14:N0} đ", "THANH TOÁN:", _amount));
            sb.AppendLine(string.Format("{0,-25} {1,14}", "Phương thức:", _method));
            sb.AppendLine("========================================");
            sb.AppendLine("       CẢM ƠN QUÝ KHÁCH!");
            sb.AppendLine("         HẸN GẶP LẠI!");

            _receiptText = sb.ToString();
            rtb.Text = _receiptText;
            _printDocument = new PrintDocument { DocumentName = $"HoaDon_{_order.OrderID}" };
            _printDocument.DefaultPageSettings.PaperSize = new PaperSize("80mm", 315, 1400);
            _printDocument.DefaultPageSettings.Margins = new Margins(12, 12, 12, 12);
            _printDocument.PrintPage += (_, e) =>
            {
                using var printFont = new Font("Consolas", 9f);
                e.Graphics?.DrawString(_receiptText, printFont, Brushes.Black, e.MarginBounds);
                e.HasMorePages = false;
            };
            FormClosed += (_, _) => _printDocument?.Dispose();

            // Bottom action buttons
            var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = UITheme.Surface };
            pnlBottom.Paint += (s, e) => { using var p = new Pen(UITheme.Border); e.Graphics.DrawLine(p, 0, 0, pnlBottom.Width, 0); };
            this.Controls.Add(pnlBottom);

            var btnPrint = UITheme.CreateButton("🖨️ In Hóa Đơn", UITheme.Success);
            btnPrint.Size = new Size(140, 36); btnPrint.Location = new Point(60, 7);
            btnPrint.Click += (s, e) =>
            {
                if (PrintReceipt()) this.Close();
            };
            pnlBottom.Controls.Add(btnPrint);

            var btnClose = UITheme.CreateButton("Đóng", UITheme.Dark);
            btnClose.Size = new Size(110, 36); btnClose.Location = new Point(220, 7);
            btnClose.Click += (s, e) => this.Close();
            pnlBottom.Controls.Add(btnClose);

            this.Controls.SetChildIndex(rtb, 0);
            this.Controls.SetChildIndex(pnlBottom, 1);
            this.Controls.SetChildIndex(pnlTop, 2);
        }

        public bool PrintReceipt()
        {
            try
            {
                _printDocument?.Print();
                ValidationHelper.ShowSuccess("Đã gửi hóa đơn tới máy in mặc định.");
                return true;
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError("Không thể in hóa đơn. Hãy kiểm tra máy in mặc định. " + ex.Message);
                return false;
            }
        }

        public bool SaveReceiptAsPdf(string filePath)
        {
            if (_printDocument == null) return false;
            string? pdfPrinter = PrinterSettings.InstalledPrinters
                .Cast<string>()
                .FirstOrDefault(name => name.Contains("Microsoft Print to PDF", StringComparison.OrdinalIgnoreCase));
            if (pdfPrinter == null)
                throw new InvalidOperationException("Máy in Microsoft Print to PDF chưa được cài đặt trên máy này.");

            var settings = _printDocument.PrinterSettings;
            string originalPrinter = settings.PrinterName;
            bool originalPrintToFile = settings.PrintToFile;
            string originalFileName = settings.PrintFileName;
            var originalController = _printDocument.PrintController;
            try
            {
                settings.PrinterName = pdfPrinter;
                settings.PrintToFile = true;
                settings.PrintFileName = filePath;
                _printDocument.PrintController = new StandardPrintController();
                _printDocument.Print();
                return File.Exists(filePath);
            }
            finally
            {
                settings.PrinterName = originalPrinter;
                settings.PrintToFile = originalPrintToFile;
                settings.PrintFileName = originalFileName;
                _printDocument.PrintController = originalController;
            }
        }
    }
}
