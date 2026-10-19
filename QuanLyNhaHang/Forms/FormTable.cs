using QuanLyNhaHang.Helpers;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Service;

namespace QuanLyNhaHang.Forms
{
    public partial class FormTable : Form
    {
        private readonly TableService _svc = new();
        private readonly OrderService _orderService = new();
        private List<Table> _tables = new();
        private Table? _selectedTable;

        public FormTable()
        {
            InitializeComponent();
            ApplyToolbarTheme(btnAdd, UITheme.Success);
            ApplyToolbarTheme(btnEdit, UITheme.Primary);
            ApplyToolbarTheme(btnDelete, UITheme.Danger);
            ApplyToolbarTheme(btnOrder, UITheme.Primary);
            ApplyToolbarTheme(btnRefresh, UITheme.Warning);
            ApplyToolbarTheme(btnReprintInvoice, UITheme.Primary);
            btnReprintInvoice.Visible = AuthService.Instance.IsCashier || AuthService.Instance.IsAdmin;
            UITheme.EnableDoubleBuffering(this);
        }

        private static void ApplyToolbarTheme(Button button, Color color)
        {
            button.BackColor = color;
            button.Font = new Font(UITheme.FontFamily, 9.5f, FontStyle.Bold);
            button.FlatAppearance.BorderSize = 0;
            UITheme.SetButtonInteraction(button, color);
            UITheme.ApplyRoundedCorners(button, UITheme.ButtonCornerRadius);
        }

        private async void FormTable_Load(object sender, EventArgs e)
        {
            await LoadTablesAsync();
        }

        private async Task LoadTablesAsync()
        {
            try
            {
                _tables = await _svc.GetAllTablesAsync();
                RenderTableGrid();
                UpdateStats();
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError("Lỗi tải dữ liệu bàn: " + ex.Message);
            }
        }

        private void RenderTableGrid()
        {
            pnlTableGrid.Controls.Clear();
            const int cardW = 180, cardH = 135;

            foreach (var t in _tables)
            {
                var card = new Panel
                {
                    Size = new Size(cardW, cardH),
                    Margin = new Padding(10),
                    BackColor = t.StatusColor,
                    Cursor = Cursors.Hand,
                    Tag = t
                };

                // Border paint if selected
                card.Paint += (s, e) =>
                {
                    if (_selectedTable != null && _selectedTable.TableID == t.TableID)
                    {
                        using var pen = new Pen(Color.FromArgb(241, 196, 15), 3);
                        e.Graphics.DrawRectangle(pen, 1, 1, card.Width - 3, card.Height - 3);
                    }
                };

                var lblIcon = new Label
                {
                    Text = "🪑",
                    Font = new Font("Segoe UI Emoji", 14f),
                    ForeColor = Color.White,
                    AutoSize = false,
                    Size = new Size(cardW, 25),
                    Location = new Point(0, 10),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                var lblName = new Label
                {
                    Text = t.TableName,
                    Font = new Font("Segoe UI", 12.5f, FontStyle.Bold),
                    ForeColor = Color.White,
                    AutoSize = false,
                    Size = new Size(cardW, 30),
                    Location = new Point(0, 35),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                var lblStatus = new Label
                {
                    Text = t.StatusDisplay,
                    Font = UITheme.FontSmall,
                    ForeColor = Color.FromArgb(235, 255, 255, 255),
                    AutoSize = false,
                    Size = new Size(cardW, 22),
                    Location = new Point(0, 68),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                var lblCap = new Label
                {
                    Text = $"👥 Sức chứa: {t.Capacity} người",
                    Font = UITheme.FontSmall,
                    ForeColor = Color.FromArgb(215, 255, 255, 255),
                    AutoSize = false,
                    Size = new Size(cardW, 20),
                    Location = new Point(0, 92),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                card.Controls.AddRange(new Control[] { lblIcon, lblName, lblStatus, lblCap });

                // Forward clicks
                card.Click += card_Click;
                card.DoubleClick += card_DoubleClick;
                foreach (Control c in card.Controls)
                {
                    c.Click += (s, e) => card_Click(card, e);
                    c.DoubleClick += (s, e) => card_DoubleClick(card, e);
                }

                pnlTableGrid.Controls.Add(card);
            }
        }

        private void card_Click(object? sender, EventArgs e)
        {
            if (sender is Panel card && card.Tag is Table t)
            {
                _selectedTable = t;
                pnlTableGrid.Invalidate(true);
                UpdateActionButtons();
            }
        }

        private void card_DoubleClick(object? sender, EventArgs e)
        {
            if (_selectedTable != null)
                btnOrder_Click(sender, e);
        }

        private void UpdateActionButtons()
        {
            bool sel = _selectedTable != null;
            bool admin = AuthService.Instance.IsAdmin;
            bool canPrintInvoices = admin || AuthService.Instance.IsCashier;
            btnEdit.Enabled = sel && admin;
            btnDelete.Enabled = sel && admin && _selectedTable?.TableStatus == "Trong";
            btnOrder.Enabled = sel && (_selectedTable?.TableStatus == "Trong" || _selectedTable?.TableStatus == "Dang phuc vu");
            btnReprintInvoice.Enabled = sel && canPrintInvoices;
        }

        private void UpdateStats()
        {
            lblTotal.Text = $"Tổng bàn: {_tables.Count}";
            lblAvailable.Text = $"Trống: {_tables.Count(t => t.TableStatus == "Trong")}";
            lblOccupied.Text = $"Đang phục vụ: {_tables.Count(t => t.TableStatus == "Dang phuc vu")}";
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            using var dlg = new FormTableEdit(null);
            if (dlg.ShowDialog() == DialogResult.OK && dlg.ResultTable != null)
            {
                var (ok, msg) = await _svc.AddTableAsync(dlg.ResultTable.TableName, dlg.ResultTable.Capacity);
                if (ok) { ValidationHelper.ShowSuccess(msg); await LoadTablesAsync(); }
                else ValidationHelper.ShowError(msg);
            }
        }

        private async void btnEdit_Click(object sender, EventArgs e)
        {
            if (_selectedTable == null) return;
            using var dlg = new FormTableEdit(_selectedTable);
            if (dlg.ShowDialog() == DialogResult.OK && dlg.ResultTable != null)
            {
                var (ok, msg) = await _svc.UpdateTableAsync(dlg.ResultTable);
                if (ok) { ValidationHelper.ShowSuccess(msg); await LoadTablesAsync(); }
                else ValidationHelper.ShowError(msg);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedTable == null) return;
            if (!ValidationHelper.Confirm($"Bạn có chắc muốn xóa bàn '{_selectedTable.TableName}'?")) return;
            var (ok, msg) = await _svc.DeleteTableAsync(_selectedTable.TableID);
            if (ok) { ValidationHelper.ShowSuccess(msg); _selectedTable = null; await LoadTablesAsync(); }
            else ValidationHelper.ShowError(msg);
        }

        private void btnOrder_Click(object? sender, EventArgs e)
        {
            if (_selectedTable == null) return;
            var fOrder = new FormOrder(_selectedTable);
            if (Parent?.FindForm() is FormMain main)
            {
                main.NavigateTo(fOrder);
                return;
            }

            fOrder.TopLevel = false;
            fOrder.FormBorderStyle = FormBorderStyle.None;
            fOrder.Dock = DockStyle.Fill;
            if (this.Parent is Panel pnl)
            {
                pnl.Controls.Clear();
                pnl.Controls.Add(fOrder);
                fOrder.Show();
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e) => await LoadTablesAsync();

        private async void btnReprintInvoice_Click(object? sender, EventArgs e)
        {
            var auth = AuthService.Instance;
            if (auth.CurrentUser == null || (!auth.IsAdmin && !auth.IsCashier))
            {
                ValidationHelper.ShowError("Chỉ Thu ngân hoặc Quản lý được in lại hóa đơn.");
                return;
            }
            if (_selectedTable == null) return;

            try
            {
                UseWaitCursor = true;
                var invoices = await _orderService.GetPaidInvoicesForTableAsync(_selectedTable.TableID);
                if (invoices.Count == 0)
                {
                    ValidationHelper.ShowError("Bàn này chưa có hóa đơn đã thanh toán.");
                    return;
                }

                using var picker = new FormPaidInvoicePicker(_selectedTable.TableName, invoices);
                if (picker.ShowDialog(this) != DialogResult.OK || picker.SelectedInvoiceId is not int invoiceId)
                    return;

                UseWaitCursor = false;
                while (true)
                {
                    try
                    {
                        using var printer = new InvoicePrinter(invoiceId);
                        var result = await printer.ReprintAsync();
                        if (result.Cancelled) return;
                        string message = "Đã gửi bản in lại tới máy in đã chọn.";
                        if (!result.PrintLogSaved)
                            message += " Chưa lưu được nhật ký in; hãy cài bảng InvoicePrintLog bằng script đi kèm.";
                        ValidationHelper.ShowSuccess(message);
                        return;
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        ValidationHelper.ShowError(ex.Message);
                        return;
                    }
                    catch (Exception ex)
                    {
                        using var recovery = new PrinterRecoveryDialog(ex.Message);
                        recovery.ShowDialog(this);
                        if (recovery.Action == PrinterRecoveryAction.Retry) continue;
                        if (recovery.Action == PrinterRecoveryAction.SavePdf)
                            await SaveInvoicePdfAsync(invoiceId);
                        return;
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                ValidationHelper.ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError("Không thể tải danh sách hóa đơn. " + ex.Message);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private async Task SaveInvoicePdfAsync(int invoiceId)
        {
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "QuanLyNhaHang", "HoaDon");
            Directory.CreateDirectory(folder);
            using var dialog = new SaveFileDialog
            {
                Title = "Lưu hóa đơn PDF",
                Filter = "Tệp PDF (*.pdf)|*.pdf",
                InitialDirectory = folder,
                FileName = $"HoaDon_HD{invoiceId:D5}_{DateTime.Now:yyyyMMdd_HHmm}.pdf",
                AddExtension = true,
                DefaultExt = "pdf",
                RestoreDirectory = true,
                OverwritePrompt = true
            };
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            try
            {
                UseWaitCursor = true;
                using var printer = new InvoicePrinter(invoiceId);
                await printer.SaveAsPdfAsync(dialog.FileName);
                using var saved = new FormPaymentSuccess.PdfSavedDialog(folder, dialog.FileName);
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

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            var q = txtSearch.Text.Trim().ToLower();
            var filtered = string.IsNullOrEmpty(q)
                ? _tables
                : _tables.Where(t => t.TableName.ToLower().Contains(q)).ToList();

            var backup = _tables;
            _tables = filtered;
            RenderTableGrid();
            _tables = backup;
        }
    }
}
