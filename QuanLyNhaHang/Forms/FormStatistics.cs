using QuanLyNhaHang.Helpers;
using QuanLyNhaHang.Service;
using System.Data;

namespace QuanLyNhaHang.Forms
{
    public partial class FormStatistics : Form
    {
        private const decimal CookingWarningThresholdMinutes = 20m;
        private readonly StatisticsService _statSvc = new();
        private readonly ServiceTimeService _timeSvc = new();

        public FormStatistics()
        {
            UITheme.ConfigureDpi(this);
            InitializeComponent();
            ConfigureVisualStyles();
            UITheme.EnableDoubleBuffering(this);
        }

        private void ConfigureVisualStyles()
        {
            pnlHeader.Paint += new PaintEventHandler(pnlHeader_Paint);
            pnlCards.Paint += new PaintEventHandler(pnlCards_Paint);
            pnlFilter.Paint += new PaintEventHandler(pnlFilter_Paint);

            StyleDataGrid(dgvRevenue);
            StyleDataGrid(dgvTopDishes);
            StyleDataGrid(dgvPayment);
            StyleDataGrid(dgvCookPerf);

            StyleButton(btnSearch, UITheme.Primary);
            StyleButton(btnToday, UITheme.Info);
            StyleButton(btnThisMonth, UITheme.Warning);
            StyleButton(btnAllTime, UITheme.Dark);

            UITheme.ApplyRoundedCorners(pnlRevenueCard, UITheme.CornerRadius);
            UITheme.ApplyHoverEffect(pnlRevenueCard, UITheme.Primary, ControlPaint.Light(UITheme.Primary, 0.05f));
            UITheme.ApplyRoundedCorners(pnlTotalRevenueCard, UITheme.CornerRadius);
            UITheme.ApplyHoverEffect(pnlTotalRevenueCard, UITheme.Success, ControlPaint.Light(UITheme.Success, 0.05f));
        }

        private static void StyleButton(Button button, Color backColor)
        {
            button.BackColor = backColor;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = UITheme.FontNormal;
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;
            UITheme.SetButtonInteraction(button, backColor);
            UITheme.ApplyRoundedCorners(button);
        }

        private static void StyleDataGrid(DataGridView grid)
        {
            grid.BackgroundColor = UITheme.Surface;
            grid.BorderStyle = BorderStyle.None;
            grid.RowHeadersVisible = false;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.ReadOnly = true;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.Font = UITheme.FontNormal;
            grid.GridColor = UITheme.Border;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersHeight = 38;
            grid.RowTemplate.Height = 34;
            grid.ColumnHeadersDefaultCellStyle.BackColor = UITheme.Primary;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font(UITheme.FontFamily, 10f, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = UITheme.Primary;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.DefaultCellStyle.SelectionBackColor = UITheme.Selection;
            grid.DefaultCellStyle.SelectionForeColor = UITheme.TextPrimary;
            grid.AlternatingRowsDefaultCellStyle.BackColor = UITheme.AlternateRow;
        }

        private void pnlHeader_Paint(object? sender, PaintEventArgs e)
        {
            using var pen = new Pen(UITheme.Border);
            e.Graphics.DrawLine(pen, 0, 55, pnlHeader.Width, 55);
        }

        private void pnlCards_Paint(object? sender, PaintEventArgs e)
        {
            using var pen = new Pen(UITheme.Border);
            e.Graphics.DrawLine(pen, 0, 79, pnlCards.Width, 79);
        }

        private void pnlFilter_Paint(object? sender, PaintEventArgs e)
        {
            using var pen = new Pen(UITheme.Border);
            e.Graphics.DrawLine(pen, 0, 51, pnlFilter.Width, 51);
        }

        private void tabControlStats_SelectedIndexChanged(object? sender, EventArgs e) =>
            btnSearch_Click(sender, e);

        private async void FormStatistics_Load(object sender, EventArgs e)
        {
            await LoadTodayRevenueAsync();
            await LoadCurrentReportAsync();
        }

        private async Task LoadTodayRevenueAsync()
        {
            try
            {
                var today = await _statSvc.GetRevenueTodayAsync();
                lblRevenueToday.Text = today.ToString("N0") + " đ";
            }
            catch
            {
                lblRevenueToday.Text = "0 đ";
            }
        }

        private async void btnSearch_Click(object? sender, EventArgs e)
        {
            await LoadCurrentReportAsync();
        }

        private async Task LoadCurrentReportAsync()
        {
            try
            {
                var from = dtpFrom.Value.Date;
                var to = dtpTo.Value.Date;
                if (from > to)
                {
                    ValidationHelper.ShowError("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc!");
                    return;
                }

                if (tabControlStats.SelectedTab == tabRevenue)
                {
                    var dt = await _statSvc.GetRevenueReportAsync(from, to);
                    dt.DefaultView.Sort = "[Ngày] ASC";
                    dgvRevenue.DataSource = dt.DefaultView;
                    UpdateRevenueChart(dt);

                    decimal totalRev = 0;
                    int totalOrders = 0;
                    foreach (DataRow r in dt.Rows)
                    {
                        if (r[1] != DBNull.Value) totalOrders += Convert.ToInt32(r[1]);
                        if (r[2] != DBNull.Value) totalRev += Convert.ToDecimal(r[2]);
                    }
                    lblTotalRevenueCard.Text = totalRev.ToString("N0") + " đ";
                    lblSummary.Text = $"📊 Tổng cộng: {totalOrders} đơn hàng | Doanh thu: {totalRev:N0} đ";
                }
                else if (tabControlStats.SelectedTab == tabTopDishes)
                {
                    var dt = await _statSvc.GetTopSellingItemsAsync(15, from, to);
                    dgvTopDishes.DataSource = dt;

                    long totalDishes = 0;
                    foreach (DataRow r in dt.Rows)
                    {
                        if (r[2] != DBNull.Value) totalDishes += Convert.ToInt64(r[2]);
                    }
                    lblSummary.Text = $"🏆 Đã phục vụ tổng cộng {totalDishes} phần món ăn trong khoảng thời gian đã chọn";
                }
                else if (tabControlStats.SelectedTab == tabPayment)
                {
                    var dt = await _statSvc.GetPaymentMethodStatsAsync(from, to);
                    decimal sumAmount = 0;
                    foreach (DataRow r in dt.Rows)
                    {
                        if (r["Doanh thu (đ)"] != DBNull.Value)
                            sumAmount += Convert.ToDecimal(r["Doanh thu (đ)"]);
                    }

                    dt.Columns.Add("Tỷ lệ %", typeof(decimal));
                    foreach (DataRow r in dt.Rows)
                    {
                        var amount = Convert.ToDecimal(r["Doanh thu (đ)"]);
                        r["Tỷ lệ %"] = sumAmount == 0 ? 0m : amount / sumAmount;
                    }

                    dgvPayment.DataSource = dt;
                    UpdatePaymentChart(dt);
                    lblSummary.Text = $"💳 Tổng tiền thu qua các kênh: {sumAmount:N0} đ";
                }
                else if (tabControlStats.SelectedTab == tabCookPerf)
                {
                    var dt = await _timeSvc.GetAverageCookingTimeByMenuItemAsync(from, to);
                    dgvCookPerf.DataSource = dt;

                    var overThresholdCount = 0;
                    foreach (DataGridViewRow row in dgvCookPerf.Rows)
                    {
                        if (row.IsNewRow || row.Cells["colCookAverage"].Value == null || row.Cells["colCookAverage"].Value == DBNull.Value)
                            continue;

                        row.DefaultCellStyle.BackColor = Color.Empty;
                        row.DefaultCellStyle.ForeColor = Color.Empty;
                        if (Convert.ToDecimal(row.Cells["colCookAverage"].Value) > CookingWarningThresholdMinutes)
                        {
                            row.DefaultCellStyle.BackColor = UITheme.DangerSoft;
                            row.DefaultCellStyle.ForeColor = UITheme.Danger;
                            overThresholdCount++;
                        }
                    }

                    lblSummary.Text = $"⏱️ {dt.Rows.Count} món có dữ liệu chế biến | {overThresholdCount} món vượt ngưỡng {CookingWarningThresholdMinutes:0} phút (tô đỏ)";
                }
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError("Lỗi tải báo cáo thống kê: " + ex.Message);
            }
        }

        private void UpdateRevenueChart(DataTable revenueByDay)
        {
            var series = chartRevenue.Series["Doanh thu"];
            series.Points.Clear();

            foreach (DataRow row in revenueByDay.Rows)
            {
                if (row["Ngày"] == DBNull.Value || row["Doanh thu (đ)"] == DBNull.Value)
                    continue;

                var date = Convert.ToDateTime(row["Ngày"]);
                var revenue = Convert.ToDouble(row["Doanh thu (đ)"]);
                int pointIndex = series.Points.AddXY(date.ToOADate(), revenue);
                series.Points[pointIndex].ToolTip = $"{date:dd/MM/yyyy}: {revenue:#,##0} đ";
            }

            chartRevenue.ChartAreas["RevenueArea"].AxisX.Interval = Math.Max(1, Math.Ceiling(revenueByDay.Rows.Count / 12d));
        }

        private void UpdatePaymentChart(DataTable paymentStats)
        {
            var series = chartPayment.Series["Tỷ lệ thanh toán"];
            series.Points.Clear();

            foreach (DataRow row in paymentStats.Rows)
            {
                var method = Convert.ToString(row["Phương thức"]) ?? string.Empty;
                var amount = Convert.ToDouble(row["Doanh thu (đ)"]);
                if (amount <= 0)
                    continue;

                int pointIndex = series.Points.AddXY(method, amount);
                series.Points[pointIndex].LegendText = method;
                series.Points[pointIndex].ToolTip = $"{method}: {amount:#,##0} đ ({Convert.ToDecimal(row["Tỷ lệ %"]):P1})";
            }
        }

        private void btnToday_Click(object sender, EventArgs e)
        {
            dtpFrom.Value = DateTime.Today;
            dtpTo.Value = DateTime.Today;
            btnSearch_Click(sender, e);
        }

        private void btnThisMonth_Click(object sender, EventArgs e)
        {
            dtpFrom.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpTo.Value = DateTime.Today;
            btnSearch_Click(sender, e);
        }

        private void btnAllTime_Click(object sender, EventArgs e)
        {
            dtpFrom.Value = DateTime.Today.AddYears(-2);
            dtpTo.Value = DateTime.Today;
            btnSearch_Click(sender, e);
        }
    }
}
