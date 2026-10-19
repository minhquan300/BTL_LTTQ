namespace QuanLyNhaHang.Forms
{
    partial class FormKitchen
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
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblAuto = new System.Windows.Forms.Label();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabKds = new System.Windows.Forms.TabPage();
            this.tablePanel = new System.Windows.Forms.TableLayoutPanel();
            this.colNew = new System.Windows.Forms.Panel();
            this.pnlColHead1 = new System.Windows.Forms.Panel();
            this.lblColTitle1 = new System.Windows.Forms.Label();
            this.lblCountNew = new System.Windows.Forms.Label();
            this.pnlNew = new System.Windows.Forms.FlowLayoutPanel();
            this.colCooking = new System.Windows.Forms.Panel();
            this.pnlColHead2 = new System.Windows.Forms.Panel();
            this.lblColTitle2 = new System.Windows.Forms.Label();
            this.lblCountCooking = new System.Windows.Forms.Label();
            this.pnlCooking = new System.Windows.Forms.FlowLayoutPanel();
            this.colReady = new System.Windows.Forms.Panel();
            this.pnlColHead3 = new System.Windows.Forms.Panel();
            this.lblColTitle3 = new System.Windows.Forms.Label();
            this.lblCountReady = new System.Windows.Forms.Label();
            this.pnlReady = new System.Windows.Forms.FlowLayoutPanel();
            this.tabTiming = new System.Windows.Forms.TabPage();
            this.dgvTiming = new System.Windows.Forms.DataGridView();
            this.colOrderCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTable = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDish = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQuantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colItemStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOrderTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWaitTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCookTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDeliverTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTotalTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSpeed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlFilter = new System.Windows.Forms.Panel();
            this.lblFrom = new System.Windows.Forms.Label();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.lblTo = new System.Windows.Forms.Label();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.lblFilterStatus = new System.Windows.Forms.Label();
            this.cboStatusFilter = new System.Windows.Forms.ComboBox();
            this.btnFilterLogs = new System.Windows.Forms.Button();
            this.btnLogsToday = new System.Windows.Forms.Button();
            this.pnlKpi = new System.Windows.Forms.Panel();
            this.cardKpi1 = new System.Windows.Forms.Panel();
            this.lblKpiT1 = new System.Windows.Forms.Label();
            this.lblKpiWait = new System.Windows.Forms.Label();
            this.cardKpi2 = new System.Windows.Forms.Panel();
            this.lblKpiT2 = new System.Windows.Forms.Label();
            this.lblKpiCook = new System.Windows.Forms.Label();
            this.cardKpi3 = new System.Windows.Forms.Panel();
            this.lblKpiT3 = new System.Windows.Forms.Label();
            this.lblKpiDeliver = new System.Windows.Forms.Label();
            this.cardKpi4 = new System.Windows.Forms.Panel();
            this.lblKpiT4 = new System.Windows.Forms.Label();
            this.lblKpiOverdue = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.tabControlMain.SuspendLayout();
            this.tabKds.SuspendLayout();
            this.tablePanel.SuspendLayout();
            this.colNew.SuspendLayout();
            this.pnlColHead1.SuspendLayout();
            this.colCooking.SuspendLayout();
            this.pnlColHead2.SuspendLayout();
            this.colReady.SuspendLayout();
            this.pnlColHead3.SuspendLayout();
            this.tabTiming.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTiming)).BeginInit();
            this.pnlFilter.SuspendLayout();
            this.pnlKpi.SuspendLayout();
            this.cardKpi1.SuspendLayout();
            this.cardKpi2.SuspendLayout();
            this.cardKpi3.SuspendLayout();
            this.cardKpi4.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.White;
            this.pnlHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.btnRefresh);
            this.pnlHeader.Controls.Add(this.lblAuto);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1100, 56);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(82)))), ((int)(((byte)(118)))));
            this.lblTitle.Location = new System.Drawing.Point(18, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(401, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "👨‍🍳 Bếp & Theo Dõi Thời Gian Chế Biến";
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(440, 11);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(150, 34);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "🔄 Làm mới dữ liệu";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lblAuto
            // 
            this.lblAuto.AutoSize = true;
            this.lblAuto.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblAuto.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(140)))), ((int)(((byte)(141)))));
            this.lblAuto.Location = new System.Drawing.Point(605, 20);
            this.lblAuto.Name = "lblAuto";
            this.lblAuto.Size = new System.Drawing.Size(169, 15);
            this.lblAuto.TabIndex = 2;
            this.lblAuto.Text = "⚡ Tự động làm mới mỗi 5 giây";
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabKds);
            this.tabControlMain.Controls.Add(this.tabTiming);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.tabControlMain.Location = new System.Drawing.Point(0, 56);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.Padding = new System.Drawing.Point(16, 8);
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(1100, 644);
            this.tabControlMain.TabIndex = 1;
            // 
            // tabKds
            // 
            this.tabKds.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.tabKds.Controls.Add(this.tablePanel);
            this.tabKds.Location = new System.Drawing.Point(4, 36);
            this.tabKds.Name = "tabKds";
            this.tabKds.Padding = new System.Windows.Forms.Padding(3);
            this.tabKds.Size = new System.Drawing.Size(1092, 604);
            this.tabKds.TabIndex = 0;
            this.tabKds.Text = "👨‍🍳 Điều Phối Bếp (KDS)";
            // 
            // tablePanel
            // 
            this.tablePanel.ColumnCount = 3;
            this.tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tablePanel.Controls.Add(this.colNew, 0, 0);
            this.tablePanel.Controls.Add(this.colCooking, 1, 0);
            this.tablePanel.Controls.Add(this.colReady, 2, 0);
            this.tablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tablePanel.Location = new System.Drawing.Point(3, 3);
            this.tablePanel.Name = "tablePanel";
            this.tablePanel.Padding = new System.Windows.Forms.Padding(6);
            this.tablePanel.RowCount = 1;
            this.tablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tablePanel.Size = new System.Drawing.Size(1086, 598);
            this.tablePanel.TabIndex = 0;
            // 
            // colNew
            // 
            this.colNew.BackColor = System.Drawing.Color.White;
            this.colNew.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colNew.Controls.Add(this.pnlNew);
            this.colNew.Controls.Add(this.pnlColHead1);
            this.colNew.Dock = System.Windows.Forms.DockStyle.Fill;
            this.colNew.Location = new System.Drawing.Point(9, 9);
            this.colNew.Name = "colNew";
            this.colNew.Size = new System.Drawing.Size(352, 580);
            this.colNew.TabIndex = 0;
            // 
            // pnlColHead1
            // 
            this.pnlColHead1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(126)))), ((int)(((byte)(34)))));
            this.pnlColHead1.Controls.Add(this.lblCountNew);
            this.pnlColHead1.Controls.Add(this.lblColTitle1);
            this.pnlColHead1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlColHead1.Location = new System.Drawing.Point(0, 0);
            this.pnlColHead1.Name = "pnlColHead1";
            this.pnlColHead1.Size = new System.Drawing.Size(350, 44);
            this.pnlColHead1.TabIndex = 0;
            // 
            // lblColTitle1
            // 
            this.lblColTitle1.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblColTitle1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblColTitle1.ForeColor = System.Drawing.Color.White;
            this.lblColTitle1.Location = new System.Drawing.Point(0, 0);
            this.lblColTitle1.Name = "lblColTitle1";
            this.lblColTitle1.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.lblColTitle1.Size = new System.Drawing.Size(200, 44);
            this.lblColTitle1.TabIndex = 0;
            this.lblColTitle1.Text = "🟡 Chờ chế biến";
            this.lblColTitle1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCountNew
            // 
            this.lblCountNew.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblCountNew.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCountNew.ForeColor = System.Drawing.Color.White;
            this.lblCountNew.Location = new System.Drawing.Point(230, 0);
            this.lblCountNew.Name = "lblCountNew";
            this.lblCountNew.Padding = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.lblCountNew.Size = new System.Drawing.Size(120, 44);
            this.lblCountNew.TabIndex = 1;
            this.lblCountNew.Text = "0 món";
            this.lblCountNew.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlNew
            // 
            this.pnlNew.AutoScroll = true;
            this.pnlNew.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.pnlNew.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlNew.Location = new System.Drawing.Point(0, 44);
            this.pnlNew.Name = "pnlNew";
            this.pnlNew.Padding = new System.Windows.Forms.Padding(8);
            this.pnlNew.Size = new System.Drawing.Size(350, 534);
            this.pnlNew.TabIndex = 1;
            // 
            // colCooking
            // 
            this.colCooking.BackColor = System.Drawing.Color.White;
            this.colCooking.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colCooking.Controls.Add(this.pnlCooking);
            this.colCooking.Controls.Add(this.pnlColHead2);
            this.colCooking.Dock = System.Windows.Forms.DockStyle.Fill;
            this.colCooking.Location = new System.Drawing.Point(367, 9);
            this.colCooking.Name = "colCooking";
            this.colCooking.Size = new System.Drawing.Size(352, 580);
            this.colCooking.TabIndex = 1;
            // 
            // pnlColHead2
            // 
            this.pnlColHead2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(84)))), ((int)(((byte)(0)))));
            this.pnlColHead2.Controls.Add(this.lblCountCooking);
            this.pnlColHead2.Controls.Add(this.lblColTitle2);
            this.pnlColHead2.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlColHead2.Location = new System.Drawing.Point(0, 0);
            this.pnlColHead2.Name = "pnlColHead2";
            this.pnlColHead2.Size = new System.Drawing.Size(350, 44);
            this.pnlColHead2.TabIndex = 0;
            // 
            // lblColTitle2
            // 
            this.lblColTitle2.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblColTitle2.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblColTitle2.ForeColor = System.Drawing.Color.White;
            this.lblColTitle2.Location = new System.Drawing.Point(0, 0);
            this.lblColTitle2.Name = "lblColTitle2";
            this.lblColTitle2.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.lblColTitle2.Size = new System.Drawing.Size(200, 44);
            this.lblColTitle2.TabIndex = 0;
            this.lblColTitle2.Text = "🟠 Đang chế biến";
            this.lblColTitle2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCountCooking
            // 
            this.lblCountCooking.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblCountCooking.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCountCooking.ForeColor = System.Drawing.Color.White;
            this.lblCountCooking.Location = new System.Drawing.Point(230, 0);
            this.lblCountCooking.Name = "lblCountCooking";
            this.lblCountCooking.Padding = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.lblCountCooking.Size = new System.Drawing.Size(120, 44);
            this.lblCountCooking.TabIndex = 1;
            this.lblCountCooking.Text = "0 món";
            this.lblCountCooking.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlCooking
            // 
            this.pnlCooking.AutoScroll = true;
            this.pnlCooking.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.pnlCooking.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCooking.Location = new System.Drawing.Point(0, 44);
            this.pnlCooking.Name = "pnlCooking";
            this.pnlCooking.Padding = new System.Windows.Forms.Padding(8);
            this.pnlCooking.Size = new System.Drawing.Size(350, 534);
            this.pnlCooking.TabIndex = 1;
            // 
            // colReady
            // 
            this.colReady.BackColor = System.Drawing.Color.White;
            this.colReady.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colReady.Controls.Add(this.pnlReady);
            this.colReady.Controls.Add(this.pnlColHead3);
            this.colReady.Dock = System.Windows.Forms.DockStyle.Fill;
            this.colReady.Location = new System.Drawing.Point(725, 9);
            this.colReady.Name = "colReady";
            this.colReady.Size = new System.Drawing.Size(352, 580);
            this.colReady.TabIndex = 2;
            // 
            // pnlColHead3
            // 
            this.pnlColHead3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.pnlColHead3.Controls.Add(this.lblCountReady);
            this.pnlColHead3.Controls.Add(this.lblColTitle3);
            this.pnlColHead3.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlColHead3.Location = new System.Drawing.Point(0, 0);
            this.pnlColHead3.Name = "pnlColHead3";
            this.pnlColHead3.Size = new System.Drawing.Size(350, 44);
            this.pnlColHead3.TabIndex = 0;
            // 
            // lblColTitle3
            // 
            this.lblColTitle3.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblColTitle3.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblColTitle3.ForeColor = System.Drawing.Color.White;
            this.lblColTitle3.Location = new System.Drawing.Point(0, 0);
            this.lblColTitle3.Name = "lblColTitle3";
            this.lblColTitle3.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.lblColTitle3.Size = new System.Drawing.Size(200, 44);
            this.lblColTitle3.TabIndex = 0;
            this.lblColTitle3.Text = "🟢 Sẵn sàng phục vụ";
            this.lblColTitle3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCountReady
            // 
            this.lblCountReady.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblCountReady.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCountReady.ForeColor = System.Drawing.Color.White;
            this.lblCountReady.Location = new System.Drawing.Point(230, 0);
            this.lblCountReady.Name = "lblCountReady";
            this.lblCountReady.Padding = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.lblCountReady.Size = new System.Drawing.Size(120, 44);
            this.lblCountReady.TabIndex = 1;
            this.lblCountReady.Text = "0 món";
            this.lblCountReady.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlReady
            // 
            this.pnlReady.AutoScroll = true;
            this.pnlReady.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.pnlReady.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlReady.Location = new System.Drawing.Point(0, 44);
            this.pnlReady.Name = "pnlReady";
            this.pnlReady.Padding = new System.Windows.Forms.Padding(8);
            this.pnlReady.Size = new System.Drawing.Size(350, 534);
            this.pnlReady.TabIndex = 1;
            // 
            // tabTiming
            // 
            this.tabTiming.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.tabTiming.Controls.Add(this.dgvTiming);
            this.tabTiming.Controls.Add(this.pnlFilter);
            this.tabTiming.Controls.Add(this.pnlKpi);
            this.tabTiming.Location = new System.Drawing.Point(4, 36);
            this.tabTiming.Name = "tabTiming";
            this.tabTiming.Padding = new System.Windows.Forms.Padding(3);
            this.tabTiming.Size = new System.Drawing.Size(1092, 604);
            this.tabTiming.TabIndex = 1;
            this.tabTiming.Text = "⏱️ Theo Dõi Thời Gian Chế Biến & Phục Vụ";
            // 
            // dgvTiming
            // 
            this.dgvTiming.AllowUserToAddRows = false;
            this.dgvTiming.AllowUserToDeleteRows = false;
            this.dgvTiming.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTiming.BackgroundColor = System.Drawing.Color.White;
            this.dgvTiming.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvTiming.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTiming.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colOrderCode,
            this.colTable,
            this.colDish,
            this.colQuantity,
            this.colItemStatus,
            this.colOrderTime,
            this.colWaitTime,
            this.colCookTime,
            this.colDeliverTime,
            this.colTotalTime,
            this.colSpeed});
            this.dgvTiming.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTiming.Location = new System.Drawing.Point(3, 137);
            this.dgvTiming.MultiSelect = false;
            this.dgvTiming.Name = "dgvTiming";
            this.dgvTiming.ReadOnly = true;
            this.dgvTiming.RowHeadersVisible = false;
            this.dgvTiming.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTiming.Size = new System.Drawing.Size(1086, 464);
            this.dgvTiming.TabIndex = 0;
            // 
            // colOrderCode
            // 
            this.colOrderCode.FillWeight = 12F;
            this.colOrderCode.HeaderText = "Mã Đơn";
            this.colOrderCode.Name = "colOrderCode";
            this.colOrderCode.ReadOnly = true;
            // 
            // colTable
            // 
            this.colTable.FillWeight = 14F;
            this.colTable.HeaderText = "Bàn";
            this.colTable.Name = "colTable";
            this.colTable.ReadOnly = true;
            // 
            // colDish
            // 
            this.colDish.FillWeight = 26F;
            this.colDish.HeaderText = "Món Ăn";
            this.colDish.Name = "colDish";
            this.colDish.ReadOnly = true;
            // 
            // colQuantity
            // 
            this.colQuantity.FillWeight = 8F;
            this.colQuantity.HeaderText = "SL";
            this.colQuantity.Name = "colQuantity";
            this.colQuantity.ReadOnly = true;
            // 
            // colItemStatus
            // 
            this.colItemStatus.FillWeight = 16F;
            this.colItemStatus.HeaderText = "Trạng Thái";
            this.colItemStatus.Name = "colItemStatus";
            this.colItemStatus.ReadOnly = true;
            // 
            // colOrderTime
            // 
            this.colOrderTime.FillWeight = 14F;
            this.colOrderTime.HeaderText = "Giờ Đặt";
            this.colOrderTime.Name = "colOrderTime";
            this.colOrderTime.ReadOnly = true;
            // 
            // colWaitTime
            // 
            this.colWaitTime.FillWeight = 14F;
            this.colWaitTime.HeaderText = "TG Chờ (p)";
            this.colWaitTime.Name = "colWaitTime";
            this.colWaitTime.ReadOnly = true;
            // 
            // colCookTime
            // 
            this.colCookTime.FillWeight = 14F;
            this.colCookTime.HeaderText = "TG Nấu (p)";
            this.colCookTime.Name = "colCookTime";
            this.colCookTime.ReadOnly = true;
            // 
            // colDeliverTime
            // 
            this.colDeliverTime.FillWeight = 14F;
            this.colDeliverTime.HeaderText = "TG Bưng (p)";
            this.colDeliverTime.Name = "colDeliverTime";
            this.colDeliverTime.ReadOnly = true;
            // 
            // colTotalTime
            // 
            this.colTotalTime.FillWeight = 14F;
            this.colTotalTime.HeaderText = "Tổng TG (p)";
            this.colTotalTime.Name = "colTotalTime";
            this.colTotalTime.ReadOnly = true;
            // 
            // colSpeed
            // 
            this.colSpeed.FillWeight = 16F;
            this.colSpeed.HeaderText = "Đánh Giá";
            this.colSpeed.Name = "colSpeed";
            this.colSpeed.ReadOnly = true;
            // 
            // pnlFilter
            // 
            this.pnlFilter.BackColor = System.Drawing.Color.White;
            this.pnlFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFilter.Controls.Add(this.lblFrom);
            this.pnlFilter.Controls.Add(this.dtpFrom);
            this.pnlFilter.Controls.Add(this.lblTo);
            this.pnlFilter.Controls.Add(this.dtpTo);
            this.pnlFilter.Controls.Add(this.lblFilterStatus);
            this.pnlFilter.Controls.Add(this.cboStatusFilter);
            this.pnlFilter.Controls.Add(this.btnFilterLogs);
            this.pnlFilter.Controls.Add(this.btnLogsToday);
            this.pnlFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFilter.Location = new System.Drawing.Point(3, 87);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Padding = new System.Windows.Forms.Padding(10, 8, 10, 8);
            this.pnlFilter.Size = new System.Drawing.Size(1086, 50);
            this.pnlFilter.TabIndex = 1;
            // 
            // lblFrom
            // 
            this.lblFrom.AutoSize = true;
            this.lblFrom.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblFrom.Location = new System.Drawing.Point(15, 15);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(26, 17);
            this.lblFrom.TabIndex = 0;
            this.lblFrom.Text = "Từ:";
            // 
            // dtpFrom
            // 
            this.dtpFrom.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFrom.Location = new System.Drawing.Point(45, 11);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(115, 24);
            this.dtpFrom.TabIndex = 1;
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblTo.Location = new System.Drawing.Point(175, 15);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(34, 17);
            this.lblTo.TabIndex = 2;
            this.lblTo.Text = "Đến:";
            // 
            // dtpTo
            // 
            this.dtpTo.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTo.Location = new System.Drawing.Point(215, 11);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(115, 24);
            this.dtpTo.TabIndex = 3;
            // 
            // lblFilterStatus
            // 
            this.lblFilterStatus.AutoSize = true;
            this.lblFilterStatus.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblFilterStatus.Location = new System.Drawing.Point(345, 15);
            this.lblFilterStatus.Name = "lblFilterStatus";
            this.lblFilterStatus.Size = new System.Drawing.Size(69, 17);
            this.lblFilterStatus.TabIndex = 4;
            this.lblFilterStatus.Text = "Trạng thái:";
            // 
            // cboStatusFilter
            // 
            this.cboStatusFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStatusFilter.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cboStatusFilter.FormattingEnabled = true;
            this.cboStatusFilter.Items.AddRange(new object[] {
            "Tất cả",
            "Moi",
            "Dang che bien",
            "San sang",
            "Da phuc vu",
            "Da huy"});
            this.cboStatusFilter.Location = new System.Drawing.Point(420, 11);
            this.cboStatusFilter.Name = "cboStatusFilter";
            this.cboStatusFilter.Size = new System.Drawing.Size(130, 24);
            this.cboStatusFilter.TabIndex = 5;
            // 
            // btnFilterLogs
            // 
            this.btnFilterLogs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnFilterLogs.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFilterLogs.FlatAppearance.BorderSize = 0;
            this.btnFilterLogs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilterLogs.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnFilterLogs.ForeColor = System.Drawing.Color.White;
            this.btnFilterLogs.Location = new System.Drawing.Point(565, 9);
            this.btnFilterLogs.Name = "btnFilterLogs";
            this.btnFilterLogs.Size = new System.Drawing.Size(130, 30);
            this.btnFilterLogs.TabIndex = 6;
            this.btnFilterLogs.Text = "🔍 Xem nhật ký";
            this.btnFilterLogs.UseVisualStyleBackColor = false;
            this.btnFilterLogs.Click += new System.EventHandler(this.btnFilterLogs_Click);
            // 
            // btnLogsToday
            // 
            this.btnLogsToday.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnLogsToday.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogsToday.FlatAppearance.BorderSize = 0;
            this.btnLogsToday.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogsToday.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnLogsToday.ForeColor = System.Drawing.Color.White;
            this.btnLogsToday.Location = new System.Drawing.Point(705, 9);
            this.btnLogsToday.Name = "btnLogsToday";
            this.btnLogsToday.Size = new System.Drawing.Size(85, 30);
            this.btnLogsToday.TabIndex = 7;
            this.btnLogsToday.Text = "Hôm nay";
            this.btnLogsToday.UseVisualStyleBackColor = false;
            this.btnLogsToday.Click += new System.EventHandler(this.btnLogsToday_Click);
            // 
            // pnlKpi
            // 
            this.pnlKpi.BackColor = System.Drawing.Color.White;
            this.pnlKpi.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlKpi.Controls.Add(this.cardKpi4);
            this.pnlKpi.Controls.Add(this.cardKpi3);
            this.pnlKpi.Controls.Add(this.cardKpi2);
            this.pnlKpi.Controls.Add(this.cardKpi1);
            this.pnlKpi.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlKpi.Location = new System.Drawing.Point(3, 3);
            this.pnlKpi.Name = "pnlKpi";
            this.pnlKpi.Padding = new System.Windows.Forms.Padding(10);
            this.pnlKpi.Size = new System.Drawing.Size(1086, 84);
            this.pnlKpi.TabIndex = 2;
            // 
            // cardKpi1
            // 
            this.cardKpi1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(156)))), ((int)(((byte)(18)))));
            this.cardKpi1.Controls.Add(this.lblKpiT1);
            this.cardKpi1.Controls.Add(this.lblKpiWait);
            this.cardKpi1.Location = new System.Drawing.Point(15, 9);
            this.cardKpi1.Name = "cardKpi1";
            this.cardKpi1.Size = new System.Drawing.Size(220, 64);
            this.cardKpi1.TabIndex = 0;
            // 
            // lblKpiT1
            // 
            this.lblKpiT1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblKpiT1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.lblKpiT1.Location = new System.Drawing.Point(10, 6);
            this.lblKpiT1.Name = "lblKpiT1";
            this.lblKpiT1.Size = new System.Drawing.Size(200, 18);
            this.lblKpiT1.TabIndex = 0;
            this.lblKpiT1.Text = "⏳ Thời gian chờ TB";
            // 
            // lblKpiWait
            // 
            this.lblKpiWait.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblKpiWait.ForeColor = System.Drawing.Color.White;
            this.lblKpiWait.Location = new System.Drawing.Point(10, 26);
            this.lblKpiWait.Name = "lblKpiWait";
            this.lblKpiWait.Size = new System.Drawing.Size(200, 32);
            this.lblKpiWait.TabIndex = 1;
            this.lblKpiWait.Text = "0 phút";
            // 
            // cardKpi2
            // 
            this.cardKpi2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(84)))), ((int)(((byte)(0)))));
            this.cardKpi2.Controls.Add(this.lblKpiT2);
            this.cardKpi2.Controls.Add(this.lblKpiCook);
            this.cardKpi2.Location = new System.Drawing.Point(250, 9);
            this.cardKpi2.Name = "cardKpi2";
            this.cardKpi2.Size = new System.Drawing.Size(220, 64);
            this.cardKpi2.TabIndex = 1;
            // 
            // lblKpiT2
            // 
            this.lblKpiT2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblKpiT2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.lblKpiT2.Location = new System.Drawing.Point(10, 6);
            this.lblKpiT2.Name = "lblKpiT2";
            this.lblKpiT2.Size = new System.Drawing.Size(200, 18);
            this.lblKpiT2.TabIndex = 0;
            this.lblKpiT2.Text = "🍳 Thời gian nấu TB";
            // 
            // lblKpiCook
            // 
            this.lblKpiCook.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblKpiCook.ForeColor = System.Drawing.Color.White;
            this.lblKpiCook.Location = new System.Drawing.Point(10, 26);
            this.lblKpiCook.Name = "lblKpiCook";
            this.lblKpiCook.Size = new System.Drawing.Size(200, 32);
            this.lblKpiCook.TabIndex = 1;
            this.lblKpiCook.Text = "0 phút";
            // 
            // cardKpi3
            // 
            this.cardKpi3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.cardKpi3.Controls.Add(this.lblKpiT3);
            this.cardKpi3.Controls.Add(this.lblKpiDeliver);
            this.cardKpi3.Location = new System.Drawing.Point(485, 9);
            this.cardKpi3.Name = "cardKpi3";
            this.cardKpi3.Size = new System.Drawing.Size(220, 64);
            this.cardKpi3.TabIndex = 2;
            // 
            // lblKpiT3
            // 
            this.lblKpiT3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblKpiT3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.lblKpiT3.Location = new System.Drawing.Point(10, 6);
            this.lblKpiT3.Name = "lblKpiT3";
            this.lblKpiT3.Size = new System.Drawing.Size(200, 18);
            this.lblKpiT3.TabIndex = 0;
            this.lblKpiT3.Text = "🛎️ Thời gian bưng món TB";
            // 
            // lblKpiDeliver
            // 
            this.lblKpiDeliver.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblKpiDeliver.ForeColor = System.Drawing.Color.White;
            this.lblKpiDeliver.Location = new System.Drawing.Point(10, 26);
            this.lblKpiDeliver.Name = "lblKpiDeliver";
            this.lblKpiDeliver.Size = new System.Drawing.Size(200, 32);
            this.lblKpiDeliver.TabIndex = 1;
            this.lblKpiDeliver.Text = "0 phút";
            // 
            // cardKpi4
            // 
            this.cardKpi4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.cardKpi4.Controls.Add(this.lblKpiT4);
            this.cardKpi4.Controls.Add(this.lblKpiOverdue);
            this.cardKpi4.Location = new System.Drawing.Point(720, 9);
            this.cardKpi4.Name = "cardKpi4";
            this.cardKpi4.Size = new System.Drawing.Size(220, 64);
            this.cardKpi4.TabIndex = 3;
            // 
            // lblKpiT4
            // 
            this.lblKpiT4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblKpiT4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.lblKpiT4.Location = new System.Drawing.Point(10, 6);
            this.lblKpiT4.Name = "lblKpiT4";
            this.lblKpiT4.Size = new System.Drawing.Size(200, 18);
            this.lblKpiT4.TabIndex = 0;
            this.lblKpiT4.Text = "⚠️ Món trễ hạn (>20p)";
            // 
            // lblKpiOverdue
            // 
            this.lblKpiOverdue.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblKpiOverdue.ForeColor = System.Drawing.Color.White;
            this.lblKpiOverdue.Location = new System.Drawing.Point(10, 26);
            this.lblKpiOverdue.Name = "lblKpiOverdue";
            this.lblKpiOverdue.Size = new System.Drawing.Size(200, 32);
            this.lblKpiOverdue.TabIndex = 1;
            this.lblKpiOverdue.Text = "0 món";
            // 
            // FormKitchen
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(1100, 700);
            this.Controls.Add(this.tabControlMain);
            this.Controls.Add(this.pnlHeader);
            this.Name = "FormKitchen";
            this.Text = "Bếp & Theo Dõi Chế Biến";
            this.Load += new System.EventHandler(this.FormKitchen_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.tabControlMain.ResumeLayout(false);
            this.tabKds.ResumeLayout(false);
            this.tablePanel.ResumeLayout(false);
            this.colNew.ResumeLayout(false);
            this.pnlColHead1.ResumeLayout(false);
            this.colCooking.ResumeLayout(false);
            this.pnlColHead2.ResumeLayout(false);
            this.colReady.ResumeLayout(false);
            this.pnlColHead3.ResumeLayout(false);
            this.tabTiming.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTiming)).EndInit();
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            this.pnlKpi.ResumeLayout(false);
            this.cardKpi1.ResumeLayout(false);
            this.cardKpi2.ResumeLayout(false);
            this.cardKpi3.ResumeLayout(false);
            this.cardKpi4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblAuto;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabKds;
        private System.Windows.Forms.TableLayoutPanel tablePanel;
        private System.Windows.Forms.Panel colNew;
        private System.Windows.Forms.Panel pnlColHead1;
        private System.Windows.Forms.Label lblColTitle1;
        private System.Windows.Forms.Label lblCountNew;
        private System.Windows.Forms.FlowLayoutPanel pnlNew;
        private System.Windows.Forms.Panel colCooking;
        private System.Windows.Forms.Panel pnlColHead2;
        private System.Windows.Forms.Label lblColTitle2;
        private System.Windows.Forms.Label lblCountCooking;
        private System.Windows.Forms.FlowLayoutPanel pnlCooking;
        private System.Windows.Forms.Panel colReady;
        private System.Windows.Forms.Panel pnlColHead3;
        private System.Windows.Forms.Label lblColTitle3;
        private System.Windows.Forms.Label lblCountReady;
        private System.Windows.Forms.FlowLayoutPanel pnlReady;
        private System.Windows.Forms.TabPage tabTiming;
        private System.Windows.Forms.DataGridView dgvTiming;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOrderCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTable;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDish;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQuantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOrderTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWaitTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCookTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDeliverTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTotalTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSpeed;
        private System.Windows.Forms.Panel pnlFilter;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.Label lblFilterStatus;
        private System.Windows.Forms.ComboBox cboStatusFilter;
        private System.Windows.Forms.Button btnFilterLogs;
        private System.Windows.Forms.Button btnLogsToday;
        private System.Windows.Forms.Panel pnlKpi;
        private System.Windows.Forms.Panel cardKpi1;
        private System.Windows.Forms.Label lblKpiT1;
        private System.Windows.Forms.Label lblKpiWait;
        private System.Windows.Forms.Panel cardKpi2;
        private System.Windows.Forms.Label lblKpiT2;
        private System.Windows.Forms.Label lblKpiCook;
        private System.Windows.Forms.Panel cardKpi3;
        private System.Windows.Forms.Label lblKpiT3;
        private System.Windows.Forms.Label lblKpiDeliver;
        private System.Windows.Forms.Panel cardKpi4;
        private System.Windows.Forms.Label lblKpiT4;
        private System.Windows.Forms.Label lblKpiOverdue;
    }
}