using QuanLyNhaHang.Helpers;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Service;
using System.Globalization;
using System.Drawing.Imaging;
using System.Text;
using System.Xml.Linq;
using QRCoder;

namespace QuanLyNhaHang.Forms
{
    /// <summary>Dialog thanh toán; keeps the public result contract used by FormOrder.</summary>
    public partial class FormPayment : Form
    {
        public decimal Amount { get; private set; }
        public string Method { get; private set; } = "Tien mat";
        public decimal DiscountPct { get; private set; }
        public decimal DiscountAmount { get; private set; }
        public decimal CashReceived { get; private set; }
        public decimal ChangeAmount { get; private set; }
        public int PaymentId { get; private set; }

        private readonly Order _order;
        private int _lastSelectedMethod = -1;
        private Button[] methodButtons = Array.Empty<Button>();
        private readonly System.Windows.Forms.Timer _qrRefreshTimer = new() { Interval = 600 };
        private readonly System.Windows.Forms.Timer _qrCountdownTimer = new() { Interval = 1000 };
        private CancellationTokenSource? _qrCancellation;
        private readonly VietQrConfiguration _qrConfiguration;
        private decimal? _qrAmount;
        private decimal? _qrPendingAmount;
        private DateTimeOffset? _qrExpiresAt;
        private bool _qrReady, _qrExpired;
        private readonly decimal _initialDiscount;
        private readonly bool _discountIsAmount;
        private readonly decimal _gross;
        private bool _syncingDiscountText;
        private bool _cashInputValid = true;
        private bool _saving;
        private readonly OrderService _orderService = new();

        public FormPayment() : this(new Order())
        {
        }

        public FormPayment(Order order, decimal initialDiscount = 0, bool discountIsAmount = false)
        {
            _order = order;
            _initialDiscount = Math.Max(0, initialDiscount);
            _discountIsAmount = discountIsAmount;
            _gross = _order.Details.Where(d => d.ItemStatus != "Da huy").Sum(d => d.SubTotal);
            _qrConfiguration = VietQrConfiguration.Load();
            UITheme.ConfigureDpi(this);
            InitializeComponent();
            ConfigureDynamicUI();
            UITheme.EnableDoubleBuffering(this);
            _qrRefreshTimer.Tick += QrRefreshTimer_Tick;
            _qrCountdownTimer.Tick += QrCountdownTimer_Tick;
            FormClosed += (_, _) =>
            {
                _qrRefreshTimer.Stop();
                _qrRefreshTimer.Dispose();
                _qrCountdownTimer.Stop();
                _qrCountdownTimer.Dispose();
                _qrCancellation?.Cancel();
                picQr.Image?.Dispose();
            };
        }

        private void ConfigureDynamicUI()
        {
            methodButtons = new[] { btnMethodCash, btnMethodTransfer, btnMethodCard };
            cboDiscountMode.SelectedIndex = _discountIsAmount ? 1 : 0;
            numDiscount.Maximum = _discountIsAmount ? 100_000_000 : 100;
            numDiscount.DecimalPlaces = _discountIsAmount ? 0 : 2;
            numDiscount.Value = Math.Min(_initialDiscount, _discountIsAmount ? 100_000_000 : 100);
            cboMethod.SelectedIndex = 0;
            numCash.Value = Math.Min(1_000_000_000, (long)Math.Ceiling(_gross / 1000) * 1000);
            txtCash.Text = FormatCash(numCash.Value);
            txtDiscount.Text = FormatDiscount(_initialDiscount, _discountIsAmount);
            lblTotal.Text = $"Tạm tính: {_gross:N0} đ";
            lblFinal.Text = _gross.ToString("N0") + " đ";
            lblQrBankValue.Text = _qrConfiguration.BankName;
            lblQrAccountValue.Text = _qrConfiguration.AccountNumber;
            lblQrOwnerValue.Text = _qrConfiguration.AccountName;
            lblQrContentValue.Text = BuildTransferMemo();

            footer.Paint += new PaintEventHandler(footer_Paint);
            pnlCash.Paint += new PaintEventHandler(MethodPanel_Paint);
            pnlTransfer.Paint += new PaintEventHandler(MethodPanel_Paint);
            pnlTransfer.Resize += PnlTransfer_Resize;
            pnlCard.Paint += new PaintEventHandler(MethodPanel_Paint);
            foreach (var button in new[] { btnCancel, btnConfirm, btnCash50, btnCash100, btnCash200,
                btnCash500, btnCashExact, btnRefreshQr, btnCopyBank, btnCopyAccount, btnCopyOwner,
                btnCopyAmount, btnCopyContent })
            {
                UITheme.SetButtonInteraction(button, button.BackColor);
                UITheme.ApplyRoundedCorners(button);
            }
            foreach (var button in new[] { btnDiscountPercent, btnDiscountAmount })
            {
                UITheme.SetButtonInteraction(button, UITheme.AlternateRow,
                    Color.FromArgb(243, 244, 246), UITheme.Border);
                UITheme.ApplyRoundedCorners(button, UITheme.ButtonCornerRadius);
            }
            foreach (var button in methodButtons)
            {
                UITheme.SetButtonInteraction(button, UITheme.Surface, UITheme.PrimarySoft, UITheme.Selection);
                UITheme.ApplyRoundedCorners(button, UITheme.CornerRadius);
            }
            UITheme.ApplyRoundedCorners(lblCardIcon, UITheme.CornerRadius);
            footer_Resize(footer, EventArgs.Empty);
            LayoutTransferPanel();
            bodyViewport_ClientSizeChanged(bodyViewport, EventArgs.Empty);
            UpdateDiscountModeButtons();
            Recalculate(_gross);
            UpdateCashControls();
        }

        private void footer_Resize(object? sender, EventArgs e)
        {
            btnCancel.Left = footer.Padding.Left;
            btnCancel.Top = footer.ClientSize.Height - footer.Padding.Bottom - btnCancel.Height;
            btnConfirm.Left = footer.ClientSize.Width - footer.Padding.Right - btnConfirm.Width;
            btnConfirm.Top = footer.ClientSize.Height - footer.Padding.Bottom - btnConfirm.Height;
            chkQrReceived.Left = footer.Padding.Left;
            chkQrReceived.Top = footer.Padding.Top;
            lblConfirmHint.Left = btnCancel.Right + 8;
            lblConfirmHint.Top = btnConfirm.Top;
            lblConfirmHint.Width = Math.Max(72, btnConfirm.Left - lblConfirmHint.Left - 8);
            lblConfirmHint.Height = btnConfirm.Height;
        }

        private void bodyViewport_ClientSizeChanged(object? sender, EventArgs e) =>
            content.Height = Math.Max(572, bodyViewport.ClientSize.Height);

        private void PnlTransfer_Resize(object? sender, EventArgs e) => LayoutTransferPanel();

        private void LayoutTransferPanel()
        {
            if (pnlTransfer == null || pnlTransfer.ClientSize.Width <= 0) return;

            int width = pnlTransfer.ClientSize.Width;
            int buttonWidth = Math.Min(88, Math.Max(76, width / 6));
            int rightPadding = 12;
            int buttonLeft = Math.Max(12, width - rightPadding - buttonWidth);
            int valueLeft = 128;
            int valueWidth = Math.Max(100, buttonLeft - valueLeft - 8);

            picQr.Left = Math.Max(10, (width - picQr.Width) / 2);
            lblQrStatus.SetBounds(10, 254, Math.Max(1, width - 20), 24);
            lblQrCountdown.SetBounds(10, 278, Math.Max(1, width - 20), 20);

            LayoutQrInfoRow(lblQrBankCaption, lblQrBankValue, btnCopyBank, 308, valueLeft, valueWidth, buttonLeft, buttonWidth);
            LayoutQrInfoRow(lblQrAccountCaption, lblQrAccountValue, btnCopyAccount, 344, valueLeft, valueWidth, buttonLeft, buttonWidth);
            LayoutQrInfoRow(lblQrOwnerCaption, lblQrOwnerValue, btnCopyOwner, 380, valueLeft, valueWidth, buttonLeft, buttonWidth);
            LayoutQrInfoRow(lblQrAmountCaption, lblQrAmountValue, btnCopyAmount, 416, valueLeft, valueWidth, buttonLeft, buttonWidth);
            LayoutQrInfoRow(lblQrContentCaption, lblQrContentValue, btnCopyContent, 452, valueLeft, valueWidth, buttonLeft, buttonWidth);

            btnRefreshQr.Left = Math.Max(10, (width - btnRefreshQr.Width) / 2);
            lblQrNote.SetBounds(10, 526, Math.Max(1, width - 20), 24);
        }

        private static void LayoutQrInfoRow(Label caption, Label value, Button copyButton,
            int top, int valueLeft, int valueWidth, int buttonLeft, int buttonWidth)
        {
            caption.SetBounds(14, top + 3, 108, 22);
            value.SetBounds(valueLeft, top, valueWidth, 24);
            copyButton.SetBounds(buttonLeft, top - 3, buttonWidth, 28);
            copyButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        }

        private void footer_Paint(object? sender, PaintEventArgs e)
        {
            using var pen = new Pen(UITheme.Border);
            e.Graphics.DrawLine(pen, 0, 0, footer.Width, 0);
        }

        private void MethodPanel_Paint(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel panel) return;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var pen = new Pen(UITheme.Border);
            using var path = new System.Drawing.Drawing2D.GraphicsPath();
            int radius = UITheme.CornerRadius * 2;
            var rect = new Rectangle(0, 0, Math.Max(1, panel.Width - 1), Math.Max(1, panel.Height - 1));
            path.AddArc(rect.Left, rect.Top, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Top, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.Left, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            e.Graphics.DrawPath(pen, path);
        }

        private void btnCancel_Click(object? sender, EventArgs e) => DialogResult = DialogResult.Cancel;
        private void chkQrReceived_CheckedChanged(object? sender, EventArgs e) => UpdateConfirmButton();
        private void btnDiscountPercent_Click(object? sender, EventArgs e) => SetDiscountMode(false);
        private void btnDiscountAmount_Click(object? sender, EventArgs e) => SetDiscountMode(true);
        private void txtDiscount_Leave(object? sender, EventArgs e) => FormatDiscountText();
        private void numDiscount_ValueChanged(object? sender, EventArgs e) => Recalculate(_gross);
        private void numCash_ValueChanged(object? sender, EventArgs e) => Recalculate(_gross);
        private void txtCash_Leave(object? sender, EventArgs e) => FormatCashText();
        private void txtCardApprovalCode_TextChanged(object? sender, EventArgs e) => UpdateConfirmButton();
        private void btnCashExact_Click(object? sender, EventArgs e) => SetCashAmount(Amount);
        private void btnRefreshQr_Click(object? sender, EventArgs e) => RefreshQrNow();

        private void cboDiscountMode_SelectedIndexChanged(object? sender, EventArgs e)
        {
            bool amountMode = cboDiscountMode.SelectedIndex == 1;
            numDiscount.Maximum = amountMode ? 100_000_000 : 100;
            numDiscount.DecimalPlaces = amountMode ? 0 : 2;
            numDiscount.Value = 0;
            txtDiscount.Text = "0";
            UpdateDiscountModeButtons();
            Recalculate(_gross);
        }

        private void cboMethod_SelectedIndexChanged(object? sender, EventArgs e) => UpdateCashControls();

        private void methodButton_Click(object? sender, EventArgs e)
        {
            if (sender is Button button && button.Tag is int index)
                cboMethod.SelectedIndex = index;
        }

        private void cashQuickButton_Click(object? sender, EventArgs e)
        {
            if (sender is Button button && button.Tag is decimal amount)
                SetCashAmount(amount);
        }

        private void copyQrValue_Click(object? sender, EventArgs e)
        {
            if (sender is Button button && button.Tag is Label valueLabel &&
                !string.IsNullOrWhiteSpace(valueLabel.Text))
                try { Clipboard.SetText(valueLabel.Text); } catch { }
        }
        private void SetCashAmount(decimal amount)
        {
            if (numCash == null) return;
            numCash.Value = Math.Min(numCash.Maximum, Math.Max(numCash.Minimum, Math.Ceiling(amount)));
            if (txtCash != null) FormatCashText();
        }

        private void CashTextChanged(object? sender, EventArgs e)
        {
            if (numCash == null) return;
            string raw = txtCash.Text.Trim().Replace(",", string.Empty);
            if (raw.Length == 0)
            {
                _cashInputValid = true;
                numCash.Value = 0;
                return;
            }
            if (!long.TryParse(raw, NumberStyles.None, CultureInfo.InvariantCulture, out long value))
            {
                _cashInputValid = false;
                UpdateConfirmButton();
                return;
            }
            _cashInputValid = true;
            numCash.Value = Math.Clamp((decimal)value, numCash.Minimum, numCash.Maximum);
        }

        private void FormatCashText()
        {
            if (txtCash == null || numCash == null) return;
            _cashInputValid = true;
            txtCash.Text = FormatCash(numCash.Value);
            txtCash.SelectionStart = txtCash.Text.Length;
        }

        private static string FormatCash(decimal amount) =>
            amount.ToString("N0", CultureInfo.InvariantCulture);

        private void SetDiscountMode(bool amountMode)
        {
            cboDiscountMode.SelectedIndex = amountMode ? 1 : 0;
            UpdateDiscountModeButtons();
        }

        private void UpdateDiscountModeButtons()
        {
            bool amountMode = cboDiscountMode.SelectedIndex == 1;
            StyleToggle(btnDiscountPercent, !amountMode);
            StyleToggle(btnDiscountAmount, amountMode);
        }

        private static void StyleToggle(Button button, bool selected)
        {
            button.BackColor = selected ? UITheme.PrimarySoft : UITheme.AlternateRow;
            button.ForeColor = selected ? UITheme.Primary : UITheme.TextMuted;
            button.FlatAppearance.BorderColor = selected ? UITheme.Primary : UITheme.Border;
            UITheme.SetButtonInteraction(button, button.BackColor,
                selected ? UITheme.Selection : Color.FromArgb(243, 244, 246),
                selected ? Color.FromArgb(191, 219, 254) : UITheme.Border);
        }

        private string FormatDiscount(decimal value, bool amountMode) => amountMode
            ? value.ToString("N0", CultureInfo.InvariantCulture)
            : value.ToString("0.##", CultureInfo.InvariantCulture);

        private void DiscountTextChanged(object? sender, EventArgs e)
        {
            if (_syncingDiscountText || numDiscount == null) return;
            string raw = txtDiscount.Text.Trim().Replace(",", string.Empty);
            if (raw.Length == 0)
            {
                numDiscount.Value = 0;
                Recalculate(_gross);
                return;
            }
            if (!decimal.TryParse(raw, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal value))
                return;
            value = Math.Clamp(value, numDiscount.Minimum, numDiscount.Maximum);
            numDiscount.Value = value;
            Recalculate(_gross);
        }

        private void FormatDiscountText()
        {
            if (txtDiscount == null || numDiscount == null) return;
            _syncingDiscountText = true;
            txtDiscount.Text = FormatDiscount(numDiscount.Value, cboDiscountMode.SelectedIndex == 1);
            _syncingDiscountText = false;
        }

        private void UpdateCashControls()
        {
            int selected = Math.Clamp(cboMethod.SelectedIndex, 0, 2);
            if (_lastSelectedMethod != selected)
            {
                chkQrReceived.Checked = false;
                _lastSelectedMethod = selected;
            }
            pnlCash.Visible = selected == 0;
            pnlTransfer.Visible = selected == 1;
            pnlCard.Visible = selected == 2;
            numCash.Enabled = selected == 0;
            chkQrReceived.Text = "Tôi đã kiểm tra tiền vào tài khoản";
            if (selected == 1)
            {
                chkQrReceived.Visible = true;
            }
            else
            {
                chkQrReceived.Visible = false;
            }
            UpdateConfirmButton();
            if (selected == 1 && IsHandleCreated)
            {
                if (!_qrConfiguration.IsConfigured)
                {
                    lblQrStatus.Text = "Chưa cấu hình thông tin tài khoản VietQR trong App.config.";
                    btnRefreshQr.Enabled = false;
                }
                else
                {
                    btnRefreshQr.Enabled = true;
                    if (!_qrReady || _qrExpired || _qrAmount != Amount)
                        ScheduleQrRefresh();
                }
            }
            else
            {
                _qrCancellation?.Cancel();
                _qrRefreshTimer.Stop();
            }
            for (int i = 0; i < methodButtons.Length; i++)
            {
                bool active = i == selected;
                methodButtons[i].BackColor = active ? UITheme.PrimarySoft : UITheme.Surface;
                methodButtons[i].ForeColor = active ? UITheme.Primary : UITheme.TextPrimary;
                methodButtons[i].FlatAppearance.BorderColor = active ? UITheme.Primary : UITheme.Border;
                methodButtons[i].FlatAppearance.BorderSize = active ? 2 : 1;
                UITheme.SetButtonInteraction(methodButtons[i], methodButtons[i].BackColor,
                    active ? UITheme.Selection : UITheme.PrimarySoft,
                    active ? Color.FromArgb(191, 219, 254) : UITheme.Selection);
            }
        }

        private void Recalculate(decimal gross)
        {
            decimal previousAmount = Amount;
            decimal discAmt = cboDiscountMode.SelectedIndex == 1
                ? Math.Min(gross, numDiscount.Value)
                : Math.Round(gross * numDiscount.Value / 100, 0);
            decimal final = gross - discAmt;
            decimal cash = numCash.Value;
            decimal change = cash - final;

            lblDiscount.Text = discAmt.ToString("N0") + " đ";
            lblFinal.Text = final.ToString("N0") + " đ";
            lblChange.Text = change >= 0
                ? $"Tiền thừa: {change:N0} đ"
                : $"Còn thiếu: {Math.Abs(change):N0} đ";
            lblChange.ForeColor = change >= 0 ? UITheme.Success : UITheme.Danger;
            Amount = final;
            UpdateConfirmButton(cash, final);
            if (cboMethod != null && cboMethod.SelectedIndex == 1 && pnlTransfer != null)
            {
                bool qrAmountChanged = (_qrAmount.HasValue && _qrAmount.Value != final) ||
                    (_qrPendingAmount.HasValue && _qrPendingAmount.Value != final);
                if (qrAmountChanged)
                {
                    _qrCancellation?.Cancel();
                    _qrPendingAmount = null;
                    _qrRefreshTimer.Stop();
                    _qrCountdownTimer.Stop();
                    _qrReady = false;
                    _qrExpired = false;
                    _qrExpiresAt = null;
                    _qrAmount = null;
                    chkQrReceived.Checked = false;
                    picQr.Image?.Dispose();
                    picQr.Image = null;
                    lblQrStatus.Text = "Số tiền thay đổi, đang tạo lại QR...";
                    lblQrCountdown.Text = "";
                    if (IsHandleCreated && _qrConfiguration.IsConfigured) ScheduleQrRefresh();
                    UpdateConfirmButton(cash, final);
                }
            }
        }

        private void UpdateConfirmButton(decimal? cashAmount = null, decimal? finalAmount = null)
        {
            if (btnConfirm == null || cboMethod == null) return;
            decimal cash = cashAmount ?? numCash.Value;
            decimal final = finalAmount ?? Amount;
            bool authorized = AuthService.Instance.IsCashier || AuthService.Instance.IsAdmin;
            bool methodValid = cboMethod.SelectedIndex switch
            {
                0 => _cashInputValid && cash >= final,
                1 => _qrReady && !_qrExpired && _qrAmount == final && chkQrReceived.Checked,
                2 => IsCardApprovalCodeValid(txtCardApprovalCode?.Text),
                _ => false
            };
            bool canConfirm = authorized && final > 0 && methodValid && !_saving;
            btnConfirm.Enabled = canConfirm;
            btnConfirm.BackColor = canConfirm ? UITheme.Success : UITheme.Border;
            btnConfirm.ForeColor = canConfirm ? Color.White : UITheme.TextMuted;
            lblConfirmHint.Text = GetConfirmHint(authorized, final, cash, methodValid, canConfirm);
            lblConfirmHint.Visible = !canConfirm;
        }

        private string GetConfirmHint(bool authorized, decimal final, decimal cash, bool methodValid, bool canConfirm)
        {
            if (canConfirm) return string.Empty;
            if (!authorized) return "Chỉ Thu ngân hoặc Quản lý được xác nhận.";
            if (final <= 0) return "Tổng thanh toán phải lớn hơn 0.";
            return cboMethod.SelectedIndex switch
            {
                0 when !_cashInputValid => "Tiền khách đưa không hợp lệ.",
                0 when cash < final => $"Còn thiếu {final - cash:N0} đ.",
                1 when !_qrReady || _qrExpired || _qrAmount != final => "Cần QR còn hiệu lực cho số tiền này.",
                1 when !chkQrReceived.Checked => "Tick xác nhận đã nhận tiền.",
                2 when !methodValid => "Nhập mã chuẩn chi hợp lệ (4–12 ký tự).",
                _ => "Kiểm tra lại thông tin thanh toán."
            };
        }

        private static bool IsCardApprovalCodeValid(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            string code = value.Trim();
            return code.Length is >= 4 and <= 12 && code.All(char.IsAsciiLetterOrDigit);
        }

        private void PaymentForm_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (!_saving) DialogResult = DialogResult.Cancel;
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }
            if (ActiveControl is TextBoxBase) return;
            int methodIndex = e.KeyCode switch
            {
                Keys.D1 or Keys.NumPad1 => 0,
                Keys.D2 or Keys.NumPad2 => 1,
                Keys.D3 or Keys.NumPad3 => 2,
                _ => -1
            };
            if (methodIndex >= 0)
            {
                cboMethod.SelectedIndex = methodIndex;
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void ScheduleQrRefresh()
        {
            _qrRefreshTimer.Stop();
            _qrRefreshTimer.Start();
        }

        private void RefreshQrNow()
        {
            _qrCancellation?.Cancel();
            _qrRefreshTimer.Stop();
            _qrCountdownTimer.Stop();
            _qrReady = false;
            _qrExpired = false;
            _qrAmount = null;
            _qrExpiresAt = null;
            chkQrReceived.Checked = false;
            picQr.Image?.Dispose();
            picQr.Image = null;
            lblQrCountdown.Text = "";
            UpdateConfirmButton();
            if (!_qrConfiguration.IsConfigured)
            {
                lblQrStatus.Text = "Chưa cấu hình thông tin tài khoản VietQR trong App.config.";
                return;
            }
            lblQrStatus.Text = "Đang tạo mã QR thanh toán...";
            QrRefreshTimer_Tick(this, EventArgs.Empty);
        }

        private async void QrRefreshTimer_Tick(object? sender, EventArgs e)
        {
            _qrRefreshTimer.Stop();
            if (cboMethod.SelectedIndex != 1 || IsDisposed || !_qrConfiguration.IsConfigured) return;
            if (Amount <= 0)
            {
                lblQrStatus.Text = "Số tiền cần thanh toán phải lớn hơn 0 để tạo QR.";
                picQr.Image?.Dispose();
                picQr.Image = null;
                return;
            }

            var cancellation = new CancellationTokenSource();
            var previous = _qrCancellation;
            _qrCancellation = cancellation;
            previous?.Cancel();
            lblQrStatus.Text = "Đang tạo mã QR thanh toán...";
            decimal qrAmount = Amount;
            _qrPendingAmount = qrAmount;
            string transferInfo = BuildTransferMemo();
            lblQrBankValue.Text = _qrConfiguration.BankName;
            lblQrAccountValue.Text = _qrConfiguration.AccountNumber;
            lblQrOwnerValue.Text = _qrConfiguration.AccountName;
            lblQrAmountValue.Text = $"{qrAmount:N0} đ";
            lblQrContentValue.Text = transferInfo;

            try
            {
                byte[] imageBytes = await Task.Run(() => GenerateVietQrImage(_qrConfiguration, qrAmount,
                    transferInfo), cancellation.Token);
                using var stream = new MemoryStream(imageBytes);
                using var image = Image.FromStream(stream);
                var renderedImage = new Bitmap(image);

                if (IsDisposed || !ReferenceEquals(_qrCancellation, cancellation) || Amount != qrAmount || cboMethod.SelectedIndex != 1)
                {
                    renderedImage.Dispose();
                    return;
                }
                var oldImage = picQr.Image;
                picQr.Image = renderedImage;
                oldImage?.Dispose();
                _qrAmount = qrAmount;
                _qrPendingAmount = null;
                _qrExpiresAt = DateTimeOffset.UtcNow.AddMinutes(10);
                _qrReady = true;
                _qrExpired = false;
                chkQrReceived.Checked = false;
                lblQrStatus.Text = "Quét mã để chuyển khoản đúng hóa đơn này.";
                UpdateQrCountdown();
                _qrCountdownTimer.Start();
                UpdateConfirmButton();
            }
            catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
            {
            }
            catch (Exception)
            {
                if (!IsDisposed && ReferenceEquals(_qrCancellation, cancellation))
                    lblQrStatus.Text = "Không tạo được mã QR. Hãy kiểm tra cấu hình ngân hàng, số tài khoản và thử làm mới.";
            }
            finally
            {
                if (ReferenceEquals(_qrCancellation, cancellation))
                {
                    _qrCancellation = null;
                    _qrPendingAmount = null;
                }
                cancellation.Dispose();
            }
        }

        private void QrCountdownTimer_Tick(object? sender, EventArgs e)
        {
            if (_qrExpiresAt.HasValue && DateTimeOffset.UtcNow >= _qrExpiresAt.Value)
            {
                _qrCountdownTimer.Stop();
                _qrExpired = true;
                _qrReady = false;
                chkQrReceived.Checked = false;
                if (picQr.Image != null)
                {
                    var faded = CreateFadedImage(picQr.Image);
                    var old = picQr.Image;
                    picQr.Image = faded;
                    old.Dispose();
                }
                lblQrStatus.Text = "QR đã hết hạn";
                lblQrCountdown.Text = "Bấm Làm mới QR để tạo mã mới.";
                UpdateConfirmButton();
                return;
            }
            UpdateQrCountdown();
        }

        private void UpdateQrCountdown()
        {
            if (!_qrExpiresAt.HasValue) return;
            var remaining = _qrExpiresAt.Value - DateTimeOffset.UtcNow;
            if (remaining < TimeSpan.Zero) remaining = TimeSpan.Zero;
            lblQrCountdown.Text = $"QR còn hiệu lực: {(int)remaining.TotalMinutes:00}:{remaining.Seconds:00}";
        }

        private static Image CreateFadedImage(Image source)
        {
            var faded = new Bitmap(source.Width, source.Height);
            using var graphics = Graphics.FromImage(faded);
            using var attributes = new ImageAttributes();
            var matrix = new ColorMatrix
            {
                Matrix00 = 0.35f, Matrix11 = 0.35f, Matrix22 = 0.35f,
                Matrix33 = 0.45f, Matrix44 = 1f
            };
            attributes.SetColorMatrix(matrix);
            graphics.DrawImage(source, new Rectangle(0, 0, faded.Width, faded.Height),
                0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attributes);
            return faded;
        }

        private string BuildTransferMemo()
        {
            string digits = new string((_order.TableName ?? string.Empty).Where(char.IsDigit).ToArray());
            return $"HD{_order.OrderID:D5} BAN{(digits.Length == 0 ? "01" : digits)}";
        }

        private static byte[] GenerateVietQrImage(VietQrConfiguration configuration, decimal amount, string transferMemo)
        {
            string payload = BuildVietQrPayload(configuration, amount, transferMemo);
            using var generator = new QRCodeGenerator();
            using var data = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            var pngQrCode = new PngByteQRCode(data);
            return pngQrCode.GetGraphic(8);
        }

        private static string BuildVietQrPayload(VietQrConfiguration configuration, decimal amount, string transferMemo)
        {
            string bankBin = configuration.BankId.Trim();
            string accountNumber = configuration.AccountNumber.Trim();
            string accountName = RemoveDiacritics(configuration.AccountName).ToUpperInvariant().Trim();
            string memo = RemoveDiacritics(transferMemo).ToUpperInvariant().Trim();

            if (bankBin.Length != 6 || !bankBin.All(char.IsAsciiDigit))
                throw new InvalidDataException("Mã BIN ngân hàng phải gồm 6 chữ số.");
            if (accountNumber.Length is < 1 or > 19 || !accountNumber.All(char.IsAsciiLetterOrDigit))
                throw new InvalidDataException("Số tài khoản không hợp lệ.");
            if (accountName.Length is < 1 or > 25 || !accountName.All(c => c is >= ' ' and <= '~'))
                throw new InvalidDataException("Tên chủ tài khoản phải gồm tối đa 25 ký tự không dấu.");
            if (memo.Length is < 1 or > 25 || !memo.All(c => c is >= ' ' and <= '~'))
                throw new InvalidDataException("Nội dung chuyển khoản phải gồm tối đa 25 ký tự không dấu.");

            long amountVnd = decimal.ToInt64(decimal.Round(amount, 0, MidpointRounding.AwayFromZero));
            if (amountVnd <= 0)
                throw new InvalidDataException("Số tiền cần thanh toán phải lớn hơn 0.");

            string beneficiary = BuildTlv("00", bankBin) + BuildTlv("01", accountNumber);
            string merchantAccount = BuildTlv("00", "A000000727") +
                BuildTlv("01", beneficiary) +
                BuildTlv("02", "QRIBFTTA");
            string additionalData = BuildTlv("08", memo);
            string payload = BuildTlv("00", "01") +
                BuildTlv("01", "12") +
                BuildTlv("38", merchantAccount) +
                BuildTlv("52", "0000") +
                BuildTlv("53", "704") +
                BuildTlv("54", amountVnd.ToString(CultureInfo.InvariantCulture)) +
                BuildTlv("58", "VN") +
                BuildTlv("59", accountName) +
                BuildTlv("62", additionalData);

            string crcInput = payload + "6304";
            return crcInput + ComputeCrc16CcittFalse(crcInput).ToString("X4", CultureInfo.InvariantCulture);
        }

        private static string BuildTlv(string tag, string value)
        {
            if (value.Length > 99)
                throw new InvalidDataException($"Trường VietQR {tag} vượt quá 99 ký tự.");
            return tag + value.Length.ToString("D2", CultureInfo.InvariantCulture) + value;
        }

        private static ushort ComputeCrc16CcittFalse(string value)
        {
            ushort crc = 0xFFFF;
            foreach (byte currentByte in Encoding.ASCII.GetBytes(value))
            {
                crc ^= (ushort)(currentByte << 8);
                for (int bit = 0; bit < 8; bit++)
                    crc = (ushort)((crc & 0x8000) != 0 ? (crc << 1) ^ 0x1021 : crc << 1);
            }
            return crc;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var chars = normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
            return new string(chars.ToArray()).Normalize(NormalizationForm.FormC);
        }

        private sealed record VietQrConfiguration(string BankId, string BankName, string AccountNumber, string AccountName)
        {
            public bool IsConfigured => !string.IsNullOrWhiteSpace(BankId) &&
                !string.IsNullOrWhiteSpace(AccountNumber) && !string.IsNullOrWhiteSpace(AccountName);

            public static VietQrConfiguration Load()
            {
                try
                {
                    string path = Path.Combine(AppContext.BaseDirectory, "App.config");
                    var doc = XDocument.Load(path);
                    string Read(string key) => doc.Root?.Element("appSettings")?.Elements("add")
                        .FirstOrDefault(x => (string?)x.Attribute("key") == key)?.Attribute("value")?.Value?.Trim() ?? "";
                    return new VietQrConfiguration(Read("VietQrBankId"), Read("VietQrBankName"),
                        Read("VietQrAccountNumber"), Read("VietQrAccountName"));
                }
                catch
                {
                    return new VietQrConfiguration("", "", "", "");
                }
            }
        }

        private async void BtnOK_Click(object? s, EventArgs e)
        {
            if (_saving || !btnConfirm.Enabled) return;
            if (!AuthService.Instance.IsCashier && !AuthService.Instance.IsAdmin)
            {
                ValidationHelper.ShowError("Chỉ Thu ngân hoặc Quản lý được thanh toán.");
                return;
            }
            decimal gross = _order.Details.Where(d => d.ItemStatus != "Da huy").Sum(d => d.SubTotal);
            if (numCash.Enabled && (!_cashInputValid || numCash.Value < Amount))
            {
                ValidationHelper.ShowError("Tiền khách đưa không đủ!");
                return;
            }
            if (cboMethod.SelectedIndex == 1 &&
                (!_qrReady || _qrExpired || _qrAmount != Amount || !chkQrReceived.Checked))
            {
                ValidationHelper.ShowError("Hãy tạo QR còn hiệu lực và xác nhận đã kiểm tra tiền vào tài khoản.");
                return;
            }
            if (cboMethod.SelectedIndex == 2 && !IsCardApprovalCodeValid(txtCardApprovalCode.Text))
            {
                ValidationHelper.ShowError("Vui lòng nhập mã chuẩn chi hợp lệ (4–12 ký tự).");
                txtCardApprovalCode.Focus();
                return;
            }
            Method = cboMethod.SelectedIndex switch
            {
                1 => "Chuyen khoan",
                2 => "The",
                _ => "Tien mat"
            };
            DiscountAmount = Math.Min(gross, cboDiscountMode.SelectedIndex == 1
                ? numDiscount.Value : Math.Round(gross * numDiscount.Value / 100, 0));
            DiscountPct = gross > 0 ? DiscountAmount * 100 / gross : 0;
            int? managerApprovedBy = AuthService.Instance.IsAdmin
                ? AuthService.Instance.CurrentUser?.EmployeeID
                : null;
            if (gross > 0 && DiscountAmount / gross > 0.20m && managerApprovedBy == null)
            {
                managerApprovedBy = await RequestManagerApprovalAsync();
                if (managerApprovedBy == null)
                {
                    ValidationHelper.ShowError("Giảm giá trên 20% cần Quản lý xác nhận.");
                    return;
                }
            }
            CashReceived = cboMethod.SelectedIndex == 0 ? numCash.Value : Amount;
            ChangeAmount = Math.Max(0, CashReceived - Amount);

            _saving = true;
            btnConfirm.Enabled = false;
            btnConfirm.Text = "Đang xử lý...";
            UseWaitCursor = true;
            Enabled = false;
            try
            {
                PaymentId = await _orderService.ProcessPaymentAsync(
                    _order.OrderID, Amount, Method, CashReceived, ChangeAmount, DiscountAmount,
                    managerApprovedBy,
                    transactionCode: cboMethod.SelectedIndex == 2
                        ? txtCardApprovalCode.Text.Trim()
                        : null);
            }
            catch (Exception ex)
            {
                Enabled = true;
                btnConfirm.Text = "✔  Xác nhận thanh toán";
                UseWaitCursor = false;
                _saving = false;
                UpdateConfirmButton();
                ValidationHelper.ShowError("Không thể lưu thanh toán; dữ liệu đã được hoàn tác. " + ex.Message);
                return;
            }

            UseWaitCursor = false;
            Enabled = true;
            using var success = new FormPaymentSuccess(_order, Amount, Method,
                DiscountPct, DiscountAmount, ChangeAmount, DateTime.Now);
            if (success.ShowDialog(this) == DialogResult.OK)
                DialogResult = DialogResult.OK;
        }

        private Task<int?> RequestManagerApprovalAsync()
        {
            using var approval = new FormXacNhanQuanLy
            {
                MoTaThaoTac = $"Giảm giá {DiscountPct:0.##}% cho HD{_order.OrderID:D5}"
            };
            if (approval.ShowDialog(this) != DialogResult.OK)
                return Task.FromResult<int?>(null);
            return Task.FromResult(approval.MaQuanLyDuyet);
        }
    }
}
