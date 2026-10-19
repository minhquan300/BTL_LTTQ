using QuanLyNhaHang.Helpers;
using System.Windows.Forms.DataVisualization.Charting;

namespace QuanLyNhaHang.Forms
{
    partial class FormStatistics
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && components != null) components.Dispose(); base.Dispose(disposing); }

        private TabControl tabControlStats = null!;
        private TabPage tabRevenue = null!;
        private TabPage tabTopDishes = null!;
        private TabPage tabPayment = null!;
        private TabPage tabCookPerf = null!;

        private DataGridView dgvRevenue = null!;
        private Chart chartRevenue = null!;
        private Chart chartPayment = null!;
        private DataGridView dgvTopDishes = null!;
        private DataGridView dgvPayment = null!;
        private DataGridView dgvCookPerf = null!;

        private DateTimePicker dtpFrom = null!, dtpTo = null!;
        private Label lblRevenueToday = null!, lblSummary = null!, lblTotalRevenueCard = null!;
        private Button btnSearch = null!, btnToday = null!, btnThisMonth = null!, btnAllTime = null!;
        private Panel pnlHeader = null!, pnlCards = null!, pnlFilter = null!, pnlSummary = null!,
            pnlRevenueCard = null!, pnlTotalRevenueCard = null!;
        private Label lblHeaderTitle = null!, lblRevenueCardTitle = null!, lblTotalRevenueCardTitle = null!,
            lblFrom = null!, lblTo = null!;
        private SplitContainer splitRevenue = null!;
        private TableLayoutPanel paymentLayout = null!;
        private ChartArea revenueArea = null!, paymentArea = null!;

        private void InitializeComponent()
        {
            this.BackColor = UITheme.Light;
            this.Load += new System.EventHandler(FormStatistics_Load);

            // ── Header ──────────────────────────────────────────
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = Color.White, Name = "pnlHeader" };
            this.Controls.Add(pnlHeader);
            lblHeaderTitle = new Label { Text = "📊 Báo Cáo & Thống Kê Nhà Hàng", Font = UITheme.FontTitle, ForeColor = UITheme.PrimaryDark, AutoSize = true, Location = new Point(18, 14), Name = "lblHeaderTitle" };
            pnlHeader.Controls.Add(lblHeaderTitle);

            // ── KPI Summary Cards row ────────────────────────────
            pnlCards = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.White, Padding = new Padding(15, 10, 15, 10), Name = "pnlCards" };
            this.Controls.Add(pnlCards);

            pnlRevenueCard = new Panel { BackColor = UITheme.Primary, Size = new Size(240, 60), Location = new Point(18, 10), Padding = new Padding(8), Name = "pnlRevenueCard" };
            lblRevenueCardTitle = new Label { Text = "💰 Doanh thu hôm nay", Font = UITheme.FontSmall, ForeColor = Color.FromArgb(220, 255, 255, 255), AutoSize = false, Size = new Size(220, 16), Location = new Point(8, 4), Name = "lblRevenueCardTitle" };
            lblRevenueToday = new Label { Text = "...", Font = new Font("Segoe UI", 15f, FontStyle.Bold), ForeColor = Color.White, AutoSize = false, Size = new Size(220, 32), Location = new Point(8, 22), Name = "lblRevenueToday" };
            pnlRevenueCard.Controls.Add(lblRevenueCardTitle);
            pnlRevenueCard.Controls.Add(lblRevenueToday);
            pnlCards.Controls.Add(pnlRevenueCard);

            pnlTotalRevenueCard = new Panel { BackColor = UITheme.Success, Size = new Size(240, 60), Location = new Point(275, 10), Padding = new Padding(8), Name = "pnlTotalRevenueCard" };
            lblTotalRevenueCardTitle = new Label { Text = "📈 Tổng doanh thu lọc", Font = UITheme.FontSmall, ForeColor = Color.FromArgb(220, 255, 255, 255), AutoSize = false, Size = new Size(220, 16), Location = new Point(8, 4), Name = "lblTotalRevenueCardTitle" };
            lblTotalRevenueCard = new Label { Text = "...", Font = new Font("Segoe UI", 15f, FontStyle.Bold), ForeColor = Color.White, AutoSize = false, Size = new Size(220, 32), Location = new Point(8, 22), Name = "lblTotalRevenueCardValue" };
            pnlTotalRevenueCard.Controls.Add(lblTotalRevenueCardTitle);
            pnlTotalRevenueCard.Controls.Add(lblTotalRevenueCard);
            pnlCards.Controls.Add(pnlTotalRevenueCard);

            // ── Filter bar ───────────────────────────────────────
            pnlFilter = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = Color.White, Padding = new Padding(15, 8, 15, 8), Name = "pnlFilter" };
            this.Controls.Add(pnlFilter);

            lblFrom = new Label { Text = "Từ:", Font = UITheme.FontNormal, AutoSize = true, Location = new Point(15, 15), Name = "lblFrom" };
            pnlFilter.Controls.Add(lblFrom);
            dtpFrom = new DateTimePicker { Format = DateTimePickerFormat.Short, Size = new Size(115, 28), Location = new Point(45, 11), Font = UITheme.FontNormal, Value = DateTime.Today.AddDays(-30) };
            pnlFilter.Controls.Add(dtpFrom);

            lblTo = new Label { Text = "Đến:", Font = UITheme.FontNormal, AutoSize = true, Location = new Point(175, 15), Name = "lblTo" };
            pnlFilter.Controls.Add(lblTo);
            dtpTo = new DateTimePicker { Format = DateTimePickerFormat.Short, Size = new Size(115, 28), Location = new Point(215, 11), Font = UITheme.FontNormal, Value = DateTime.Today };
            pnlFilter.Controls.Add(dtpTo);

            btnSearch = new Button { Name = "btnSearch", Text = "🔍 Xem báo cáo", Size = new Size(135, 30), Location = new Point(350, 10) };
            btnSearch.Click += new System.EventHandler(btnSearch_Click);
            pnlFilter.Controls.Add(btnSearch);
            btnToday = new Button { Name = "btnToday", Text = "Hôm nay", Size = new Size(85, 30), Location = new Point(495, 10) };
            btnToday.Click += new System.EventHandler(btnToday_Click);
            pnlFilter.Controls.Add(btnToday);
            btnThisMonth = new Button { Name = "btnThisMonth", Text = "Tháng này", Size = new Size(95, 30), Location = new Point(590, 10) };
            btnThisMonth.Click += new System.EventHandler(btnThisMonth_Click);
            pnlFilter.Controls.Add(btnThisMonth);
            btnAllTime = new Button { Name = "btnAllTime", Text = "Tất cả", Size = new Size(80, 30), Location = new Point(695, 10) };
            btnAllTime.Click += new System.EventHandler(btnAllTime_Click);
            pnlFilter.Controls.Add(btnAllTime);

            // ── Summary footer bar ──────────────────────────────
            pnlSummary = new Panel { Dock = DockStyle.Bottom, Height = 38, BackColor = UITheme.PrimaryDark, Name = "pnlSummary" };
            lblSummary = new Label { Text = "", Font = new Font(UITheme.FontFamily, 9.5f, FontStyle.Bold), ForeColor = Color.White, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            pnlSummary.Controls.Add(lblSummary);
            this.Controls.Add(pnlSummary);

            // ── TabControl for multi-reports ────────────────────
            tabControlStats = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font(UITheme.FontFamily, 10f, FontStyle.Bold),
                Padding = new Point(14, 6)
            };
            this.Controls.Add(tabControlStats);

            // Tab 1: Doanh thu
            tabRevenue = new TabPage("📈 Doanh Thu Theo Ngày") { BackColor = UITheme.Light };
            splitRevenue = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 245,
                Panel1MinSize = 100,
                Panel2MinSize = 120,
                BackColor = UITheme.Light,
                BorderStyle = BorderStyle.FixedSingle
            };
            dgvRevenue = new DataGridView();
            dgvRevenue.AutoGenerateColumns = false;
            dgvRevenue.ReadOnly = true;
            dgvRevenue.AllowUserToAddRows = false;
            dgvRevenue.AllowUserToDeleteRows = false;
            dgvRevenue.MultiSelect = false;
            dgvRevenue.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRevenue.AlternatingRowsDefaultCellStyle.BackColor = UITheme.AlternateRow;
            dgvRevenue.DefaultCellStyle.SelectionBackColor = UITheme.Selection;
            dgvRevenue.DefaultCellStyle.SelectionForeColor = UITheme.TextPrimary;
            dgvRevenue.Dock = DockStyle.Fill;
            dgvRevenue.Columns.AddRange(
                new DataGridViewTextBoxColumn
                {
                    Name = "colRevenueDate",
                    HeaderText = "Ngày",
                    DataPropertyName = "Ngày",
                    FillWeight = 35,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "colRevenueOrders",
                    HeaderText = "Số đơn",
                    DataPropertyName = "Số đơn",
                    FillWeight = 25,
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, Format = "#,##0" }
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "colRevenueAmount",
                    HeaderText = "Doanh thu (đ)",
                    DataPropertyName = "Doanh thu (đ)",
                    FillWeight = 40,
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "#,##0" }
                });
            splitRevenue.Panel1.Controls.Add(dgvRevenue);

            chartRevenue = new Chart { Dock = DockStyle.Fill, BackColor = Color.White };
            revenueArea = new ChartArea("RevenueArea")
            {
                BackColor = Color.White
            };
            revenueArea.AxisX.Title = "Ngày";
            revenueArea.AxisX.LabelStyle.Format = "dd/MM";
            revenueArea.AxisX.LabelStyle.Angle = -45;
            revenueArea.AxisX.MajorGrid.Enabled = false;
            revenueArea.AxisX.IntervalType = DateTimeIntervalType.Days;
            revenueArea.AxisY.Title = "Doanh thu (đ)";
            revenueArea.AxisY.LabelStyle.Format = "#,##0";
            revenueArea.AxisY.MajorGrid.LineColor = UITheme.Border;
            chartRevenue.ChartAreas.Add(revenueArea);
            chartRevenue.Titles.Add(new Title("Doanh thu theo ngày", Docking.Top, new Font("Segoe UI", 11f, FontStyle.Bold), UITheme.TextPrimary));
            chartRevenue.Legends.Add(new Legend { Enabled = false });
            chartRevenue.Series.Add(new Series("Doanh thu")
            {
                ChartArea = "RevenueArea",
                ChartType = SeriesChartType.Column,
                XValueType = ChartValueType.Date,
                YValueType = ChartValueType.Double,
                Color = UITheme.Primary,
                BorderWidth = 2,
                ToolTip = "#VALX{dd/MM/yyyy}: #VALY{#,##0} đ"
            });
            splitRevenue.Panel2.Controls.Add(chartRevenue);
            tabRevenue.Controls.Add(splitRevenue);
            tabControlStats.TabPages.Add(tabRevenue);

            // Tab 2: Món bán chạy
            tabTopDishes = new TabPage("🏆 Top Món Ăn Bán Chạy") { BackColor = UITheme.Light };
            dgvTopDishes = new DataGridView();
            dgvTopDishes.Dock = DockStyle.Fill;
            tabTopDishes.Controls.Add(dgvTopDishes);
            tabControlStats.TabPages.Add(tabTopDishes);

            // Tab 3: Phương thức thanh toán
            tabPayment = new TabPage("💳 Phương Thức Thanh Toán") { BackColor = UITheme.Light };
            paymentLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = UITheme.Light,
                Padding = new Padding(6)
            };
            paymentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 56F));
            paymentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 44F));

            dgvPayment = new DataGridView();
            dgvPayment.AutoGenerateColumns = false;
            dgvPayment.ReadOnly = true;
            dgvPayment.AllowUserToAddRows = false;
            dgvPayment.AllowUserToDeleteRows = false;
            dgvPayment.MultiSelect = false;
            dgvPayment.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPayment.AlternatingRowsDefaultCellStyle.BackColor = UITheme.AlternateRow;
            dgvPayment.DefaultCellStyle.SelectionBackColor = UITheme.Selection;
            dgvPayment.DefaultCellStyle.SelectionForeColor = UITheme.TextPrimary;
            dgvPayment.Dock = DockStyle.Fill;
            dgvPayment.Columns.AddRange(
                new DataGridViewTextBoxColumn
                {
                    Name = "colPaymentMethod",
                    HeaderText = "Phương thức",
                    DataPropertyName = "Phương thức",
                    FillWeight = 30
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "colPaymentOrders",
                    HeaderText = "Số đơn",
                    DataPropertyName = "Số đơn",
                    FillWeight = 18,
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, Format = "#,##0" }
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "colPaymentAmount",
                    HeaderText = "Doanh thu (đ)",
                    DataPropertyName = "Doanh thu (đ)",
                    FillWeight = 30,
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "#,##0" }
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "colPaymentShare",
                    HeaderText = "Tỷ lệ %",
                    DataPropertyName = "Tỷ lệ %",
                    FillWeight = 22,
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "P2" }
                });
            paymentLayout.Controls.Add(dgvPayment, 0, 0);

            chartPayment = new Chart { Dock = DockStyle.Fill, BackColor = Color.White, Palette = ChartColorPalette.BrightPastel };
            paymentArea = new ChartArea("PaymentArea") { BackColor = Color.White };
            paymentArea.AxisX.Enabled = AxisEnabled.False;
            paymentArea.AxisY.Enabled = AxisEnabled.False;
            chartPayment.ChartAreas.Add(paymentArea);
            chartPayment.Titles.Add(new Title("Tỷ lệ doanh thu theo phương thức", Docking.Top, new Font("Segoe UI", 11f, FontStyle.Bold), UITheme.TextPrimary));
            chartPayment.Legends.Add(new Legend("PaymentLegend")
            {
                Docking = Docking.Right,
                Alignment = StringAlignment.Center,
                Font = UITheme.FontSmall
            });
            chartPayment.Series.Add(new Series("Tỷ lệ thanh toán")
            {
                ChartArea = "PaymentArea",
                ChartType = SeriesChartType.Pie,
                Legend = "PaymentLegend",
                IsValueShownAsLabel = true,
                Label = "#PERCENT{P0}",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ToolTip = "#VALX: #VALY{#,##0} đ (#PERCENT{P1})"
            });
            paymentLayout.Controls.Add(chartPayment, 1, 0);
            tabPayment.Controls.Add(paymentLayout);
            tabControlStats.TabPages.Add(tabPayment);

            // Tab 4: Thời gian chế biến theo món
            tabCookPerf = new TabPage("⏱️ Thời Gian Chế Biến Theo Món") { BackColor = UITheme.Light };
            dgvCookPerf = new DataGridView();
            dgvCookPerf.Dock = DockStyle.Fill;
            dgvCookPerf.AutoGenerateColumns = false;
            dgvCookPerf.ReadOnly = true;
            dgvCookPerf.AllowUserToAddRows = false;
            dgvCookPerf.AllowUserToDeleteRows = false;
            dgvCookPerf.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCookItemName",
                DataPropertyName = "Tên món",
                HeaderText = "Tên món",
                FillWeight = 200
            });
            dgvCookPerf.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCookCount",
                DataPropertyName = "Số lần chế biến",
                HeaderText = "Số lần chế biến",
                FillWeight = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "#,##0", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvCookPerf.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCookAverage",
                DataPropertyName = "Thời gian TB (phút)",
                HeaderText = "Thời gian TB (phút)",
                FillWeight = 140,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "0.0", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvCookPerf.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCookFastest",
                DataPropertyName = "Nhanh nhất (phút)",
                HeaderText = "Nhanh nhất (phút)",
                FillWeight = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "0.0", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvCookPerf.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCookSlowest",
                DataPropertyName = "Chậm nhất (phút)",
                HeaderText = "Chậm nhất (phút)",
                FillWeight = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "0.0", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            tabCookPerf.Controls.Add(dgvCookPerf);
            tabControlStats.TabPages.Add(tabCookPerf);

            tabControlStats.SelectedIndexChanged += new System.EventHandler(tabControlStats_SelectedIndexChanged);

            // Z-Order
            this.Controls.SetChildIndex(tabControlStats, 0);
            this.Controls.SetChildIndex(pnlSummary, 1);
            this.Controls.SetChildIndex(pnlFilter, 2);
            this.Controls.SetChildIndex(pnlCards, 3);
            this.Controls.SetChildIndex(pnlHeader, 4);
        }
    }
}
