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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            pnlHeader = new Panel();
            lblHeaderTitle = new Label();
            pnlCards = new Panel();
            pnlRevenueCard = new Panel();
            lblRevenueCardTitle = new Label();
            lblRevenueToday = new Label();
            pnlTotalRevenueCard = new Panel();
            lblTotalRevenueCardTitle = new Label();
            lblTotalRevenueCard = new Label();
            pnlFilter = new Panel();
            lblFrom = new Label();
            dtpFrom = new DateTimePicker();
            lblTo = new Label();
            dtpTo = new DateTimePicker();
            btnSearch = new Button();
            btnToday = new Button();
            btnThisMonth = new Button();
            btnAllTime = new Button();
            pnlSummary = new Panel();
            lblSummary = new Label();
            tabControlStats = new TabControl();
            tabRevenue = new TabPage();
            splitRevenue = new SplitContainer();
            dgvRevenue = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            tabTopDishes = new TabPage();
            dgvTopDishes = new DataGridView();
            tabPayment = new TabPage();
            paymentLayout = new TableLayoutPanel();
            dgvPayment = new DataGridView();
            dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn6 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn7 = new DataGridViewTextBoxColumn();
            tabCookPerf = new TabPage();
            dgvCookPerf = new DataGridView();
            dataGridViewTextBoxColumn8 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn9 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn10 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn11 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn12 = new DataGridViewTextBoxColumn();
            pnlHeader.SuspendLayout();
            pnlCards.SuspendLayout();
            pnlRevenueCard.SuspendLayout();
            pnlTotalRevenueCard.SuspendLayout();
            pnlFilter.SuspendLayout();
            pnlSummary.SuspendLayout();
            tabControlStats.SuspendLayout();
            tabRevenue.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitRevenue).BeginInit();
            splitRevenue.Panel1.SuspendLayout();
            splitRevenue.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRevenue).BeginInit();
            tabTopDishes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTopDishes).BeginInit();
            tabPayment.SuspendLayout();
            paymentLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPayment).BeginInit();
            tabCookPerf.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCookPerf).BeginInit();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.Controls.Add(lblHeaderTitle);
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(200, 100);
            pnlHeader.TabIndex = 0;
            // 
            // lblHeaderTitle
            // 
            lblHeaderTitle.Location = new Point(0, 0);
            lblHeaderTitle.Name = "lblHeaderTitle";
            lblHeaderTitle.Size = new Size(100, 23);
            lblHeaderTitle.TabIndex = 0;
            // 
            // pnlCards
            // 
            pnlCards.Controls.Add(pnlRevenueCard);
            pnlCards.Controls.Add(pnlTotalRevenueCard);
            pnlCards.Location = new Point(0, 0);
            pnlCards.Name = "pnlCards";
            pnlCards.Size = new Size(200, 100);
            pnlCards.TabIndex = 1;
            // 
            // pnlRevenueCard
            // 
            pnlRevenueCard.Controls.Add(lblRevenueCardTitle);
            pnlRevenueCard.Controls.Add(lblRevenueToday);
            pnlRevenueCard.Location = new Point(0, 0);
            pnlRevenueCard.Name = "pnlRevenueCard";
            pnlRevenueCard.Size = new Size(200, 100);
            pnlRevenueCard.TabIndex = 0;
            // 
            // lblRevenueCardTitle
            // 
            lblRevenueCardTitle.Location = new Point(0, 0);
            lblRevenueCardTitle.Name = "lblRevenueCardTitle";
            lblRevenueCardTitle.Size = new Size(100, 23);
            lblRevenueCardTitle.TabIndex = 0;
            // 
            // lblRevenueToday
            // 
            lblRevenueToday.Location = new Point(0, 0);
            lblRevenueToday.Name = "lblRevenueToday";
            lblRevenueToday.Size = new Size(100, 23);
            lblRevenueToday.TabIndex = 1;
            // 
            // pnlTotalRevenueCard
            // 
            pnlTotalRevenueCard.Controls.Add(lblTotalRevenueCardTitle);
            pnlTotalRevenueCard.Controls.Add(lblTotalRevenueCard);
            pnlTotalRevenueCard.Location = new Point(0, 0);
            pnlTotalRevenueCard.Name = "pnlTotalRevenueCard";
            pnlTotalRevenueCard.Size = new Size(200, 100);
            pnlTotalRevenueCard.TabIndex = 1;
            // 
            // lblTotalRevenueCardTitle
            // 
            lblTotalRevenueCardTitle.Location = new Point(0, 0);
            lblTotalRevenueCardTitle.Name = "lblTotalRevenueCardTitle";
            lblTotalRevenueCardTitle.Size = new Size(100, 23);
            lblTotalRevenueCardTitle.TabIndex = 0;
            // 
            // lblTotalRevenueCard
            // 
            lblTotalRevenueCard.Location = new Point(0, 0);
            lblTotalRevenueCard.Name = "lblTotalRevenueCard";
            lblTotalRevenueCard.Size = new Size(100, 23);
            lblTotalRevenueCard.TabIndex = 1;
            // 
            // pnlFilter
            // 
            pnlFilter.Controls.Add(lblFrom);
            pnlFilter.Controls.Add(dtpFrom);
            pnlFilter.Controls.Add(lblTo);
            pnlFilter.Controls.Add(dtpTo);
            pnlFilter.Controls.Add(btnSearch);
            pnlFilter.Controls.Add(btnToday);
            pnlFilter.Controls.Add(btnThisMonth);
            pnlFilter.Controls.Add(btnAllTime);
            pnlFilter.Location = new Point(0, 0);
            pnlFilter.Name = "pnlFilter";
            pnlFilter.Size = new Size(200, 100);
            pnlFilter.TabIndex = 2;
            // 
            // lblFrom
            // 
            lblFrom.Location = new Point(0, 0);
            lblFrom.Name = "lblFrom";
            lblFrom.Size = new Size(100, 23);
            lblFrom.TabIndex = 0;
            // 
            // dtpFrom
            // 
            dtpFrom.Location = new Point(0, 0);
            dtpFrom.Name = "dtpFrom";
            dtpFrom.Size = new Size(200, 23);
            dtpFrom.TabIndex = 1;
            // 
            // lblTo
            // 
            lblTo.Location = new Point(0, 0);
            lblTo.Name = "lblTo";
            lblTo.Size = new Size(100, 23);
            lblTo.TabIndex = 2;
            // 
            // dtpTo
            // 
            dtpTo.Location = new Point(0, 0);
            dtpTo.Name = "dtpTo";
            dtpTo.Size = new Size(200, 23);
            dtpTo.TabIndex = 3;
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(0, 0);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(75, 23);
            btnSearch.TabIndex = 4;
            btnSearch.Click += btnSearch_Click;
            // 
            // btnToday
            // 
            btnToday.Location = new Point(0, 0);
            btnToday.Name = "btnToday";
            btnToday.Size = new Size(75, 23);
            btnToday.TabIndex = 5;
            btnToday.Click += btnToday_Click;
            // 
            // btnThisMonth
            // 
            btnThisMonth.Location = new Point(0, 0);
            btnThisMonth.Name = "btnThisMonth";
            btnThisMonth.Size = new Size(75, 23);
            btnThisMonth.TabIndex = 6;
            btnThisMonth.Click += btnThisMonth_Click;
            // 
            // btnAllTime
            // 
            btnAllTime.Location = new Point(0, 0);
            btnAllTime.Name = "btnAllTime";
            btnAllTime.Size = new Size(75, 23);
            btnAllTime.TabIndex = 7;
            btnAllTime.Click += btnAllTime_Click;
            // 
            // pnlSummary
            // 
            pnlSummary.Controls.Add(lblSummary);
            pnlSummary.Location = new Point(0, 0);
            pnlSummary.Name = "pnlSummary";
            pnlSummary.Size = new Size(200, 100);
            pnlSummary.TabIndex = 3;
            // 
            // lblSummary
            // 
            lblSummary.Location = new Point(0, 0);
            lblSummary.Name = "lblSummary";
            lblSummary.Size = new Size(100, 23);
            lblSummary.TabIndex = 0;
            // 
            // tabControlStats
            // 
            tabControlStats.Controls.Add(tabRevenue);
            tabControlStats.Controls.Add(tabTopDishes);
            tabControlStats.Controls.Add(tabPayment);
            tabControlStats.Controls.Add(tabCookPerf);
            tabControlStats.Location = new Point(0, 0);
            tabControlStats.Name = "tabControlStats";
            tabControlStats.SelectedIndex = 0;
            tabControlStats.Size = new Size(746, 249);
            tabControlStats.TabIndex = 4;
            tabControlStats.SelectedIndexChanged += tabControlStats_SelectedIndexChanged;
            // 
            // tabRevenue
            // 
            tabRevenue.Controls.Add(splitRevenue);
            tabRevenue.Location = new Point(4, 24);
            tabRevenue.Name = "tabRevenue";
            tabRevenue.Size = new Size(738, 221);
            tabRevenue.TabIndex = 0;
            tabRevenue.Text = "📈 Doanh Thu Theo Ngày";
            // 
            // splitRevenue
            // 
            splitRevenue.Location = new Point(0, 0);
            splitRevenue.Name = "splitRevenue";
            // 
            // splitRevenue.Panel1
            // 
            splitRevenue.Panel1.Controls.Add(dgvRevenue);
            splitRevenue.Size = new Size(150, 100);
            splitRevenue.TabIndex = 0;
            // 
            // dgvRevenue
            // 
            dgvRevenue.AllowUserToAddRows = false;
            dgvRevenue.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(249, 250, 251);
            dgvRevenue.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgvRevenue.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2, dataGridViewTextBoxColumn3 });
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Window;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dataGridViewCellStyle2.SelectionForeColor = Color.FromArgb(31, 41, 55);
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            dgvRevenue.DefaultCellStyle = dataGridViewCellStyle2;
            dgvRevenue.Dock = DockStyle.Fill;
            dgvRevenue.Location = new Point(0, 0);
            dgvRevenue.MultiSelect = false;
            dgvRevenue.Name = "dgvRevenue";
            dgvRevenue.ReadOnly = true;
            dgvRevenue.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRevenue.Size = new Size(50, 100);
            dgvRevenue.TabIndex = 0;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // tabTopDishes
            // 
            tabTopDishes.Controls.Add(dgvTopDishes);
            tabTopDishes.Location = new Point(4, 24);
            tabTopDishes.Name = "tabTopDishes";
            tabTopDishes.Size = new Size(192, 72);
            tabTopDishes.TabIndex = 1;
            tabTopDishes.Text = "🏆 Top Món Ăn Bán Chạy";
            // 
            // dgvTopDishes
            // 
            dgvTopDishes.Dock = DockStyle.Fill;
            dgvTopDishes.Location = new Point(0, 0);
            dgvTopDishes.Name = "dgvTopDishes";
            dgvTopDishes.Size = new Size(192, 72);
            dgvTopDishes.TabIndex = 0;
            // 
            // tabPayment
            // 
            tabPayment.Controls.Add(paymentLayout);
            tabPayment.Location = new Point(4, 24);
            tabPayment.Name = "tabPayment";
            tabPayment.Size = new Size(192, 72);
            tabPayment.TabIndex = 2;
            tabPayment.Text = "💳 Phương Thức Thanh Toán";
            // 
            // paymentLayout
            // 
            paymentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 56F));
            paymentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 44F));
            paymentLayout.Controls.Add(dgvPayment, 0, 0);
            paymentLayout.Location = new Point(0, 0);
            paymentLayout.Name = "paymentLayout";
            paymentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            paymentLayout.Size = new Size(200, 100);
            paymentLayout.TabIndex = 0;
            // 
            // dgvPayment
            // 
            dgvPayment.AllowUserToAddRows = false;
            dgvPayment.AllowUserToDeleteRows = false;
            dataGridViewCellStyle3.BackColor = Color.FromArgb(249, 250, 251);
            dgvPayment.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            dgvPayment.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn4, dataGridViewTextBoxColumn5, dataGridViewTextBoxColumn6, dataGridViewTextBoxColumn7 });
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = SystemColors.Window;
            dataGridViewCellStyle4.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle4.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dataGridViewCellStyle4.SelectionForeColor = Color.FromArgb(31, 41, 55);
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.False;
            dgvPayment.DefaultCellStyle = dataGridViewCellStyle4;
            dgvPayment.Dock = DockStyle.Fill;
            dgvPayment.Location = new Point(3, 3);
            dgvPayment.MultiSelect = false;
            dgvPayment.Name = "dgvPayment";
            dgvPayment.ReadOnly = true;
            dgvPayment.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPayment.Size = new Size(194, 94);
            dgvPayment.TabIndex = 0;
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn5
            // 
            dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn6
            // 
            dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            dataGridViewTextBoxColumn6.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn7
            // 
            dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            dataGridViewTextBoxColumn7.ReadOnly = true;
            // 
            // tabCookPerf
            // 
            tabCookPerf.Controls.Add(dgvCookPerf);
            tabCookPerf.Location = new Point(4, 24);
            tabCookPerf.Name = "tabCookPerf";
            tabCookPerf.Size = new Size(192, 72);
            tabCookPerf.TabIndex = 3;
            tabCookPerf.Text = "⏱️ Thời Gian Chế Biến Theo Món";
            // 
            // dgvCookPerf
            // 
            dgvCookPerf.AllowUserToAddRows = false;
            dgvCookPerf.AllowUserToDeleteRows = false;
            dgvCookPerf.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn8, dataGridViewTextBoxColumn9, dataGridViewTextBoxColumn10, dataGridViewTextBoxColumn11, dataGridViewTextBoxColumn12 });
            dgvCookPerf.Dock = DockStyle.Fill;
            dgvCookPerf.Location = new Point(0, 0);
            dgvCookPerf.Name = "dgvCookPerf";
            dgvCookPerf.ReadOnly = true;
            dgvCookPerf.Size = new Size(192, 72);
            dgvCookPerf.TabIndex = 0;
            // 
            // dataGridViewTextBoxColumn8
            // 
            dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            dataGridViewTextBoxColumn8.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn9
            // 
            dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            dataGridViewTextBoxColumn9.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn10
            // 
            dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            dataGridViewTextBoxColumn10.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn11
            // 
            dataGridViewTextBoxColumn11.Name = "dataGridViewTextBoxColumn11";
            dataGridViewTextBoxColumn11.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn12
            // 
            dataGridViewTextBoxColumn12.Name = "dataGridViewTextBoxColumn12";
            dataGridViewTextBoxColumn12.ReadOnly = true;
            // 
            // FormStatistics
            // 
            BackColor = Color.White;
            ClientSize = new Size(746, 365);
            Controls.Add(tabControlStats);
            Controls.Add(pnlSummary);
            Controls.Add(pnlFilter);
            Controls.Add(pnlCards);
            Controls.Add(pnlHeader);
            Name = "FormStatistics";
            Load += FormStatistics_Load;
            pnlHeader.ResumeLayout(false);
            pnlCards.ResumeLayout(false);
            pnlRevenueCard.ResumeLayout(false);
            pnlTotalRevenueCard.ResumeLayout(false);
            pnlFilter.ResumeLayout(false);
            pnlSummary.ResumeLayout(false);
            tabControlStats.ResumeLayout(false);
            tabRevenue.ResumeLayout(false);
            splitRevenue.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitRevenue).EndInit();
            splitRevenue.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvRevenue).EndInit();
            tabTopDishes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvTopDishes).EndInit();
            tabPayment.ResumeLayout(false);
            paymentLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvPayment).EndInit();
            tabCookPerf.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvCookPerf).EndInit();
            ResumeLayout(false);
        }

        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn11;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn12;
    }
}
