namespace QuanLyNhaHang.Forms
{
    partial class FormOrder
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
            pnlHeader = new Panel();
            btnBack = new Button();
            lblTitle = new Label();
            lblTableName = new Label();
            cboTableSelect = new ComboBox();
            lblNoteTitle = new Label();
            txtItemNote = new TextBox();
            split = new SplitContainer();
            flowMenu = new FlowLayoutPanel();
            tabCategories = new TabControl();
            txtMenuSearch = new TextBox();
            dgvOrder = new DataGridView();
            colName = new DataGridViewTextBoxColumn();
            colQty = new DataGridViewTextBoxColumn();
            colPrice = new DataGridViewTextBoxColumn();
            colSub = new DataGridViewTextBoxColumn();
            colStatus = new DataGridViewTextBoxColumn();
            colID = new DataGridViewTextBoxColumn();
            pnlBottom = new Panel();
            btnDecrease = new Button();
            btnIncrease = new Button();
            btnRemoveItem = new Button();
            lblTotal = new Label();
            btnCancelOrder = new Button();
            btnCheckout = new Button();
            pnlOrderStats = new Panel();
            lblItemCount = new Label();
            lblOrderTitle = new Label();
            pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)split).BeginInit();
            split.Panel1.SuspendLayout();
            split.Panel2.SuspendLayout();
            split.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvOrder).BeginInit();
            pnlBottom.SuspendLayout();
            pnlOrderStats.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.White;
            pnlHeader.BorderStyle = BorderStyle.FixedSingle;
            pnlHeader.Controls.Add(btnBack);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblTableName);
            pnlHeader.Controls.Add(cboTableSelect);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1100, 60);
            pnlHeader.TabIndex = 0;
            // 
            // btnBack
            // 
            btnBack.BackColor = Color.FromArgb(44, 62, 80);
            btnBack.Cursor = Cursors.Hand;
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnBack.ForeColor = Color.White;
            btnBack.Location = new Point(15, 12);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(125, 34);
            btnBack.TabIndex = 0;
            btnBack.Text = "← Quay lại bàn";
            btnBack.UseVisualStyleBackColor = false;
            btnBack.Click += btnBack_Click;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(26, 82, 118);
            lblTitle.Location = new Point(284, 13);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(100, 30);
            lblTitle.TabIndex = 1;
            lblTitle.Text = "Gọi món";
            // 
            // lblTableName
            // 
            lblTableName.BackColor = Color.FromArgb(236, 253, 245);
            lblTableName.BorderStyle = BorderStyle.FixedSingle;
            lblTableName.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTableName.ForeColor = Color.FromArgb(22, 163, 74);
            lblTableName.Location = new Point(390, 17);
            lblTableName.Name = "lblTableName";
            lblTableName.Size = new Size(310, 26);
            lblTableName.TabIndex = 2;
            lblTableName.Text = "Chọn bàn • Trống • Số khách: 0";
            lblTableName.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cboTableSelect
            // 
            cboTableSelect.DrawMode = DrawMode.OwnerDrawFixed;
            cboTableSelect.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTableSelect.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            cboTableSelect.FormattingEnabled = true;
            cboTableSelect.Location = new Point(890, 16);
            cboTableSelect.Name = "cboTableSelect";
            cboTableSelect.Size = new Size(195, 27);
            cboTableSelect.TabIndex = 3;
            cboTableSelect.DrawItem += cboTableSelect_DrawItem;
            cboTableSelect.SelectedIndexChanged += cboTableSelect_SelectedIndexChanged;
            // 
            // lblNoteTitle
            // 
            lblNoteTitle.Dock = DockStyle.Top;
            lblNoteTitle.Font = new Font("Segoe UI", 8.5F);
            lblNoteTitle.ForeColor = Color.FromArgb(127, 140, 141);
            lblNoteTitle.Location = new Point(0, 61);
            lblNoteTitle.Name = "lblNoteTitle";
            lblNoteTitle.Padding = new Padding(10, 3, 0, 0);
            lblNoteTitle.Size = new Size(650, 20);
            lblNoteTitle.TabIndex = 4;
            lblNoteTitle.Text = "Ghi chú cho món tiếp theo";
            // 
            // txtItemNote
            // 
            txtItemNote.BorderStyle = BorderStyle.FixedSingle;
            txtItemNote.Dock = DockStyle.Top;
            txtItemNote.Font = new Font("Segoe UI", 9.5F);
            txtItemNote.Location = new Point(0, 81);
            txtItemNote.Name = "txtItemNote";
            txtItemNote.PlaceholderText = "Nhập ghi chú món khi thêm (vd: ít cay, không hành)";
            txtItemNote.Size = new Size(650, 24);
            txtItemNote.TabIndex = 5;
            // 
            // split
            // 
            split.BackColor = Color.FromArgb(220, 220, 220);
            split.Dock = DockStyle.Fill;
            split.Location = new Point(0, 60);
            split.Name = "split";
            // 
            // split.Panel1
            // 
            split.Panel1.BackColor = Color.FromArgb(245, 247, 250);
            split.Panel1.Controls.Add(flowMenu);
            split.Panel1.Controls.Add(txtItemNote);
            split.Panel1.Controls.Add(lblNoteTitle);
            split.Panel1.Controls.Add(tabCategories);
            split.Panel1.Controls.Add(txtMenuSearch);
            split.Panel1MinSize = 420;
            // 
            // split.Panel2
            // 
            split.Panel2.BackColor = Color.White;
            split.Panel2.Controls.Add(dgvOrder);
            split.Panel2.Controls.Add(pnlBottom);
            split.Panel2.Controls.Add(pnlOrderStats);
            split.Panel2.Controls.Add(lblOrderTitle);
            split.Panel2MinSize = 420;
            split.Size = new Size(1100, 640);
            split.SplitterDistance = 650;
            split.TabIndex = 1;
            // 
            // flowMenu
            // 
            flowMenu.AutoScroll = true;
            flowMenu.BackColor = Color.FromArgb(245, 247, 250);
            flowMenu.Dock = DockStyle.Fill;
            flowMenu.Location = new Point(0, 105);
            flowMenu.Name = "flowMenu";
            flowMenu.Padding = new Padding(8);
            flowMenu.Size = new Size(650, 535);
            flowMenu.TabIndex = 2;
            // 
            // tabCategories
            // 
            tabCategories.Dock = DockStyle.Top;
            tabCategories.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            tabCategories.Location = new Point(0, 25);
            tabCategories.Name = "tabCategories";
            tabCategories.SelectedIndex = 0;
            tabCategories.Size = new Size(650, 36);
            tabCategories.TabIndex = 1;
            tabCategories.SelectedIndexChanged += tabCategories_SelectedIndexChanged;
            // 
            // txtMenuSearch
            // 
            txtMenuSearch.BorderStyle = BorderStyle.FixedSingle;
            txtMenuSearch.Dock = DockStyle.Top;
            txtMenuSearch.Font = new Font("Segoe UI", 10F);
            txtMenuSearch.Location = new Point(0, 0);
            txtMenuSearch.Name = "txtMenuSearch";
            txtMenuSearch.PlaceholderText = "🔍 Tìm món ăn theo tên...";
            txtMenuSearch.Size = new Size(650, 25);
            txtMenuSearch.TabIndex = 0;
            txtMenuSearch.TextChanged += txtMenuSearch_TextChanged;
            // 
            // dgvOrder
            // 
            dgvOrder.AllowUserToAddRows = false;
            dgvOrder.AllowUserToDeleteRows = false;
            dgvOrder.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvOrder.BackgroundColor = Color.White;
            dgvOrder.BorderStyle = BorderStyle.None;
            dgvOrder.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvOrder.Columns.AddRange(new DataGridViewColumn[] { colName, colQty, colPrice, colSub, colStatus, colID });
            dgvOrder.Dock = DockStyle.Fill;
            dgvOrder.Location = new Point(0, 66);
            dgvOrder.MultiSelect = false;
            dgvOrder.Name = "dgvOrder";
            dgvOrder.ReadOnly = true;
            dgvOrder.RowHeadersVisible = false;
            dgvOrder.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvOrder.Size = new Size(446, 439);
            dgvOrder.TabIndex = 2;
            // 
            // colName
            // 
            colName.FillWeight = 34F;
            colName.HeaderText = "Món ăn";
            colName.Name = "colName";
            colName.ReadOnly = true;
            // 
            // colQty
            // 
            colQty.FillWeight = 10F;
            colQty.HeaderText = "SL";
            colQty.Name = "colQty";
            colQty.ReadOnly = true;
            // 
            // colPrice
            // 
            colPrice.FillWeight = 22F;
            colPrice.HeaderText = "Đơn giá";
            colPrice.Name = "colPrice";
            colPrice.ReadOnly = true;
            // 
            // colSub
            // 
            colSub.FillWeight = 24F;
            colSub.HeaderText = "Thành tiền";
            colSub.Name = "colSub";
            colSub.ReadOnly = true;
            // 
            // colStatus
            // 
            colStatus.FillWeight = 20F;
            colStatus.HeaderText = "Trạng thái";
            colStatus.Name = "colStatus";
            colStatus.ReadOnly = true;
            // 
            // colID
            // 
            colID.HeaderText = "ID";
            colID.Name = "colID";
            colID.ReadOnly = true;
            colID.Visible = false;
            // 
            // pnlBottom
            // 
            pnlBottom.BackColor = Color.White;
            pnlBottom.BorderStyle = BorderStyle.FixedSingle;
            pnlBottom.Controls.Add(btnDecrease);
            pnlBottom.Controls.Add(btnIncrease);
            pnlBottom.Controls.Add(btnRemoveItem);
            pnlBottom.Controls.Add(lblTotal);
            pnlBottom.Controls.Add(btnCancelOrder);
            pnlBottom.Controls.Add(btnCheckout);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 505);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Padding = new Padding(10, 5, 10, 5);
            pnlBottom.Size = new Size(446, 135);
            pnlBottom.TabIndex = 3;
            // 
            // btnDecrease
            // 
            btnDecrease.BackColor = Color.FromArgb(243, 156, 18);
            btnDecrease.Cursor = Cursors.Hand;
            btnDecrease.FlatAppearance.BorderSize = 0;
            btnDecrease.FlatStyle = FlatStyle.Flat;
            btnDecrease.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnDecrease.ForeColor = Color.White;
            btnDecrease.Location = new Point(10, 8);
            btnDecrease.Name = "btnDecrease";
            btnDecrease.Size = new Size(38, 32);
            btnDecrease.TabIndex = 0;
            btnDecrease.Text = "−";
            btnDecrease.UseVisualStyleBackColor = false;
            btnDecrease.Click += btnDecrease_Click;
            // 
            // btnIncrease
            // 
            btnIncrease.BackColor = Color.FromArgb(41, 128, 185);
            btnIncrease.Cursor = Cursors.Hand;
            btnIncrease.FlatAppearance.BorderSize = 0;
            btnIncrease.FlatStyle = FlatStyle.Flat;
            btnIncrease.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnIncrease.ForeColor = Color.White;
            btnIncrease.Location = new Point(54, 8);
            btnIncrease.Name = "btnIncrease";
            btnIncrease.Size = new Size(38, 32);
            btnIncrease.TabIndex = 1;
            btnIncrease.Text = "+";
            btnIncrease.UseVisualStyleBackColor = false;
            btnIncrease.Click += btnIncrease_Click;
            // 
            // btnRemoveItem
            // 
            btnRemoveItem.BackColor = Color.FromArgb(231, 76, 60);
            btnRemoveItem.Cursor = Cursors.Hand;
            btnRemoveItem.FlatAppearance.BorderSize = 0;
            btnRemoveItem.FlatStyle = FlatStyle.Flat;
            btnRemoveItem.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnRemoveItem.ForeColor = Color.White;
            btnRemoveItem.Location = new Point(98, 8);
            btnRemoveItem.Name = "btnRemoveItem";
            btnRemoveItem.Size = new Size(110, 32);
            btnRemoveItem.TabIndex = 2;
            btnRemoveItem.Text = "🗑 Hủy món";
            btnRemoveItem.UseVisualStyleBackColor = false;
            btnRemoveItem.Click += btnRemoveItem_Click;
            // 
            // lblTotal
            // 
            lblTotal.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTotal.ForeColor = Color.FromArgb(231, 76, 60);
            lblTotal.Location = new Point(10, 48);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(320, 32);
            lblTotal.TabIndex = 3;
            lblTotal.Text = "Tổng tiền: 0 đ";
            lblTotal.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnCancelOrder
            // 
            btnCancelOrder.BackColor = Color.FromArgb(231, 76, 60);
            btnCancelOrder.Cursor = Cursors.Hand;
            btnCancelOrder.FlatAppearance.BorderSize = 0;
            btnCancelOrder.FlatStyle = FlatStyle.Flat;
            btnCancelOrder.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnCancelOrder.ForeColor = Color.White;
            btnCancelOrder.Location = new Point(10, 86);
            btnCancelOrder.Name = "btnCancelOrder";
            btnCancelOrder.Size = new Size(130, 40);
            btnCancelOrder.TabIndex = 4;
            btnCancelOrder.Text = "❌ Hủy đơn";
            btnCancelOrder.UseVisualStyleBackColor = false;
            btnCancelOrder.Click += btnCancelOrder_Click;
            // 
            // btnCheckout
            // 
            btnCheckout.BackColor = Color.FromArgb(39, 174, 96);
            btnCheckout.Cursor = Cursors.Hand;
            btnCheckout.FlatAppearance.BorderSize = 0;
            btnCheckout.FlatStyle = FlatStyle.Flat;
            btnCheckout.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCheckout.ForeColor = Color.White;
            btnCheckout.Location = new Point(150, 86);
            btnCheckout.Name = "btnCheckout";
            btnCheckout.Size = new Size(180, 40);
            btnCheckout.TabIndex = 5;
            btnCheckout.Text = "💳 Thanh toán";
            btnCheckout.UseVisualStyleBackColor = false;
            btnCheckout.Click += btnCheckout_Click;
            // 
            // pnlOrderStats
            // 
            pnlOrderStats.BackColor = Color.FromArgb(240, 248, 255);
            pnlOrderStats.Controls.Add(lblItemCount);
            pnlOrderStats.Dock = DockStyle.Top;
            pnlOrderStats.Location = new Point(0, 36);
            pnlOrderStats.Name = "pnlOrderStats";
            pnlOrderStats.Size = new Size(446, 30);
            pnlOrderStats.TabIndex = 1;
            // 
            // lblItemCount
            // 
            lblItemCount.AutoSize = true;
            lblItemCount.Font = new Font("Segoe UI", 9F);
            lblItemCount.ForeColor = Color.FromArgb(127, 140, 141);
            lblItemCount.Location = new Point(10, 7);
            lblItemCount.Name = "lblItemCount";
            lblItemCount.Size = new Size(60, 15);
            lblItemCount.TabIndex = 0;
            lblItemCount.Text = "Số món: 0";
            // 
            // lblOrderTitle
            // 
            lblOrderTitle.BackColor = Color.White;
            lblOrderTitle.Dock = DockStyle.Top;
            lblOrderTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblOrderTitle.ForeColor = Color.FromArgb(26, 82, 118);
            lblOrderTitle.Location = new Point(0, 0);
            lblOrderTitle.Name = "lblOrderTitle";
            lblOrderTitle.Padding = new Padding(10, 0, 0, 0);
            lblOrderTitle.Size = new Size(446, 36);
            lblOrderTitle.TabIndex = 0;
            lblOrderTitle.Text = "\U0001f9fe Danh Sách Món Đang Gọi";
            lblOrderTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // FormOrder
            // 
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(1100, 700);
            Controls.Add(split);
            Controls.Add(pnlHeader);
            Name = "FormOrder";
            Text = "Gọi Món";
            Load += FormOrder_Load;
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            split.Panel1.ResumeLayout(false);
            split.Panel1.PerformLayout();
            split.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)split).EndInit();
            split.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvOrder).EndInit();
            pnlBottom.ResumeLayout(false);
            pnlOrderStats.ResumeLayout(false);
            pnlOrderStats.PerformLayout();
            ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblTableName;
        private System.Windows.Forms.ComboBox cboTableSelect;
        private System.Windows.Forms.Label lblNoteTitle;
        private System.Windows.Forms.TextBox txtItemNote;
        private System.Windows.Forms.SplitContainer split;
        private System.Windows.Forms.TextBox txtMenuSearch;
        private System.Windows.Forms.TabControl tabCategories;
        private System.Windows.Forms.FlowLayoutPanel flowMenu;
        private System.Windows.Forms.Label lblOrderTitle;
        private System.Windows.Forms.Panel pnlOrderStats;
        private System.Windows.Forms.Label lblItemCount;
        private System.Windows.Forms.DataGridView dgvOrder;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSub;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colID;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Button btnDecrease;
        private System.Windows.Forms.Button btnIncrease;
        private System.Windows.Forms.Button btnRemoveItem;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnCancelOrder;
        private System.Windows.Forms.Button btnCheckout;
    }
}
