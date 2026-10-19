namespace QuanLyNhaHang.Helpers
{
    public static class ValidationHelper
    {
        public static bool IsNotEmpty(string? value) => !string.IsNullOrWhiteSpace(value);

        public static bool IsPositiveDecimal(string value, out decimal result)
        {
            result = 0;
            return decimal.TryParse(value.Replace(",", ""), out result) && result > 0;
        }

        public static bool IsPositiveInt(string value, out int result)
        {
            result = 0;
            return int.TryParse(value, out result) && result > 0;
        }

        public static void ShowError(string message) =>
            MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

        public static void ShowSuccess(string message) =>
            MessageBox.Show(message, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

        public static bool Confirm(string message) =>
            MessageBox.Show(message, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

        public static string? ShowInputDialog(string prompt, string title, string defaultValue = "")
        {
            using var form = new Form
            {
                Width = 400,
                Height = 185,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = title,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.White
            };

            var textLabel = new Label
            {
                Left = 20,
                Top = 16,
                Text = prompt,
                AutoSize = true,
                Font = UITheme.FontNormal,
                ForeColor = UITheme.TextPrimary
            };

            var textBox = new TextBox
            {
                Left = 20,
                Top = 42,
                Width = 345,
                Text = defaultValue,
                Font = UITheme.FontNormal,
                BorderStyle = BorderStyle.FixedSingle
            };

            var btnOk = UITheme.CreateButton("✔ Đồng ý", UITheme.Success);
            btnOk.Size = new Size(110, 34);
            btnOk.Location = new Point(135, 90);
            btnOk.Click += (sender, e) => { form.DialogResult = DialogResult.OK; form.Close(); };

            var btnCancel = UITheme.CreateButton("✖ Hủy", UITheme.Danger);
            btnCancel.Size = new Size(95, 34);
            btnCancel.Location = new Point(255, 90);
            btnCancel.Click += (sender, e) => { form.DialogResult = DialogResult.Cancel; form.Close(); };

            form.Controls.AddRange(new Control[] { textLabel, textBox, btnOk, btnCancel });
            form.AcceptButton = btnOk;
            form.CancelButton = btnCancel;

            return form.ShowDialog() == DialogResult.OK ? textBox.Text : null;
        }
    }
}
