namespace QuanLyNhaHang.Forms
{
    partial class FormTableEdit
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblHead = new Label();
            lblName = new Label();
            txtName = new TextBox();
            lblCap = new Label();
            numCapacity = new NumericUpDown();
            lblStatus = new Label();
            cboStatus = new ComboBox();
            btnOK = new Button();
            btnCancel = new Button();
            ((System.ComponentModel.ISupportInitialize)numCapacity).BeginInit();
            SuspendLayout();
            // 
            // lblHead
            // 
            lblHead.BackColor = Color.FromArgb(26, 82, 118);
            lblHead.Dock = DockStyle.Top;
            lblHead.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblHead.ForeColor = Color.White;
            lblHead.Location = new Point(0, 0);
            lblHead.Name = "lblHead";
            lblHead.Size = new Size(406, 45);
            lblHead.TabIndex = 0;
            lblHead.Text = "\U0001fa91 THÔNG TIN BÀN ĂN";
            lblHead.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblName.ForeColor = Color.FromArgb(44, 62, 80);
            lblName.Location = new Point(30, 65);
            lblName.Name = "lblName";
            lblName.Size = new Size(71, 19);
            lblName.TabIndex = 1;
            lblName.Text = "Tên bàn *";
            // 
            // txtName
            // 
            txtName.BorderStyle = BorderStyle.FixedSingle;
            txtName.Font = new Font("Segoe UI", 10.5F);
            txtName.Location = new Point(30, 88);
            txtName.Name = "txtName";
            txtName.PlaceholderText = "Ví dụ: Bàn 01";
            txtName.Size = new Size(340, 26);
            txtName.TabIndex = 2;
            // 
            // lblCap
            // 
            lblCap.AutoSize = true;
            lblCap.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblCap.ForeColor = Color.FromArgb(44, 62, 80);
            lblCap.Location = new Point(30, 130);
            lblCap.Name = "lblCap";
            lblCap.Size = new Size(132, 19);
            lblCap.TabIndex = 3;
            lblCap.Text = "Sức chứa (người) *";
            // 
            // numCapacity
            // 
            numCapacity.Font = new Font("Segoe UI", 10.5F);
            numCapacity.Location = new Point(30, 153);
            numCapacity.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
            numCapacity.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numCapacity.Name = "numCapacity";
            numCapacity.Size = new Size(340, 26);
            numCapacity.TabIndex = 4;
            numCapacity.Value = new decimal(new int[] { 4, 0, 0, 0 });
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblStatus.ForeColor = Color.FromArgb(44, 62, 80);
            lblStatus.Location = new Point(30, 195);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(76, 19);
            lblStatus.TabIndex = 5;
            lblStatus.Text = "Trạng thái";
            // 
            // cboStatus
            // 
            cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cboStatus.Font = new Font("Segoe UI", 10F);
            cboStatus.FormattingEnabled = true;
            cboStatus.Items.AddRange(new object[] { "Trong", "Dat truoc", "Ngung su dung" });
            cboStatus.Location = new Point(30, 218);
            cboStatus.Name = "cboStatus";
            cboStatus.Size = new Size(340, 25);
            cboStatus.TabIndex = 6;
            // 
            // btnOK
            // 
            btnOK.BackColor = Color.FromArgb(39, 174, 96);
            btnOK.Cursor = Cursors.Hand;
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.FlatStyle = FlatStyle.Flat;
            btnOK.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnOK.ForeColor = Color.White;
            btnOK.Location = new Point(30, 270);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(160, 40);
            btnOK.TabIndex = 7;
            btnOK.Text = "✔ Lưu";
            btnOK.UseVisualStyleBackColor = false;
            btnOK.Click += BtnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.FromArgb(231, 76, 60);
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCancel.ForeColor = Color.White;
            btnCancel.Location = new Point(210, 270);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(160, 40);
            btnCancel.TabIndex = 8;
            btnCancel.Text = "✖ Hủy";
            btnCancel.UseVisualStyleBackColor = false;
            // 
            // FormTableEdit
            // 
            AcceptButton = btnOK;
            BackColor = Color.White;
            CancelButton = btnCancel;
            ClientSize = new Size(406, 333);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Controls.Add(cboStatus);
            Controls.Add(lblStatus);
            Controls.Add(numCapacity);
            Controls.Add(lblCap);
            Controls.Add(txtName);
            Controls.Add(lblName);
            Controls.Add(lblHead);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormTableEdit";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Thông tin bàn";
            ((System.ComponentModel.ISupportInitialize)numCapacity).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        private System.Windows.Forms.Label lblHead;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblCap;
        private System.Windows.Forms.NumericUpDown numCapacity;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox cboStatus;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}
