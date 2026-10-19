using QuanLyNhaHang.Helpers;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Forms;

internal sealed class FormPaidInvoicePicker : Form
{
    private readonly IReadOnlyList<PaidInvoiceSummary> _invoices;
    private readonly DataGridView _grid;
    private readonly Button _printButton;

    public int? SelectedInvoiceId { get; private set; }

    public FormPaidInvoicePicker(string tableName, IReadOnlyList<PaidInvoiceSummary> invoices)
    {
        UITheme.ConfigureDpi(this);
        _invoices = invoices;
        Text = $"Hóa đơn đã thanh toán — {tableName}";
        ClientSize = new Size(820, 480);
        MinimumSize = new Size(700, 400);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.Sizable;
        BackColor = UITheme.Light;
        Font = UITheme.FontNormal;

        var title = new Label
        {
            Text = "Chọn hóa đơn đã thanh toán để in lại",
            Dock = DockStyle.Top,
            Height = 54,
            Padding = new Padding(18, 0, 0, 0),
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font(UITheme.FontFamily, 14, FontStyle.Bold),
            ForeColor = UITheme.TextPrimary,
            BackColor = Color.White
        };
        _grid = UITheme.CreateDataGrid();
        _grid.Dock = DockStyle.Fill;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.MultiSelect = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.RowHeadersVisible = false;
        _grid.AutoGenerateColumns = false;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mã hóa đơn", DataPropertyName = nameof(PaidInvoiceSummary.OrderCode), FillWeight = 24 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Thời gian thanh toán", DataPropertyName = nameof(PaidInvoiceSummary.PaymentTime), FillWeight = 28, DefaultCellStyle = { Format = "dd/MM/yyyy HH:mm" } });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Phương thức", DataPropertyName = nameof(PaidInvoiceSummary.PaymentMethodDisplay), FillWeight = 20 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Thu ngân", DataPropertyName = nameof(PaidInvoiceSummary.CashierName), FillWeight = 22 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Số tiền", DataPropertyName = nameof(PaidInvoiceSummary.Amount), FillWeight = 22, DefaultCellStyle = { Format = "#,##0 'đ'", Alignment = DataGridViewContentAlignment.MiddleRight } });
        _grid.DataSource = _invoices.ToList();
        _grid.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) SelectInvoice(); };

        var footer = new Panel { Dock = DockStyle.Bottom, Height = 64, BackColor = Color.White, Padding = new Padding(12) };
        _printButton = UITheme.CreateButton("In lại hóa đơn", UITheme.Primary);
        _printButton.Size = new Size(150, 38);
        _printButton.Dock = DockStyle.Right;
        _printButton.Click += (_, _) => SelectInvoice();
        _grid.SelectionChanged += (_, _) => _printButton.Enabled = _grid.CurrentRow != null;
        var cancel = UITheme.CreateButton("Đóng", UITheme.AlternateRow, UITheme.TextPrimary);
        cancel.Size = new Size(90, 38);
        cancel.Dock = DockStyle.Right;
        cancel.Margin = new Padding(0, 0, 8, 0);
        cancel.Click += (_, _) => DialogResult = DialogResult.Cancel;
        footer.Controls.Add(_printButton);
        footer.Controls.Add(cancel);

        Controls.Add(_grid);
        Controls.Add(footer);
        Controls.Add(title);
        CancelButton = cancel;
        _printButton.Enabled = _invoices.Count > 0;
        UITheme.EnableDoubleBuffering(this);
    }

    private void SelectInvoice()
    {
        if (_grid.CurrentRow?.DataBoundItem is not PaidInvoiceSummary invoice) return;
        SelectedInvoiceId = invoice.OrderID;
        DialogResult = DialogResult.OK;
    }
}
