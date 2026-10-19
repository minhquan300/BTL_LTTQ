using QuanLyNhaHang.Helpers;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Forms
{
    public partial class FormTableEdit : Form
    {
        public Table? ResultTable { get; private set; }
        private readonly Table? _existing;

        public FormTableEdit(Table? existing)
        {
            _existing = existing;
            InitializeComponent();

            this.Text = _existing == null ? "Thêm bàn mới" : "Sửa bàn";
            lblHead.Text = _existing == null ? "➕ THÊM BÀN ĂN MỚI" : "✏️ CHỈNH SỬA BÀN ĂN";

            if (_existing != null)
            {
                FillData(_existing);
            }
            else
            {
                cboStatus.Visible = false;
                lblStatus.Visible = false;
            }
        }

        private void FillData(Table t)
        {
            txtName.Text = t.TableName;
            numCapacity.Value = t.Capacity;
            var idx = cboStatus.Items.IndexOf(t.TableStatus);
            if (idx >= 0) cboStatus.SelectedIndex = idx;
        }

        private void BtnOK_Click(object? s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                ValidationHelper.ShowError("Tên bàn không được để trống!");
                return;
            }

            ResultTable = new Table
            {
                TableID = _existing?.TableID ?? 0,
                TableName = txtName.Text.Trim(),
                Capacity = (int)numCapacity.Value,
                TableStatus = _existing != null ? cboStatus.SelectedItem?.ToString() ?? "Trong" : "Trong",
                QRCode = _existing?.QRCode
            };
            this.DialogResult = DialogResult.OK;
        }
    }
}
