using QuanLyNhaHang.Helpers;
using QuanLyNhaHang.Service;
using System.Data;
using System.Windows.Forms.DataVisualization.Charting;

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
            ConfigureCharts();
            ConfigureLayout();
            ConfigureVisualStyles();
            UITheme.EnableDoubleBuffering(this);
        }

        private void ConfigureLayout()
        {
            BackColor = UITheme.Light;

            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 56;
            pnlHeader.BackColor = Color.White;
            lblHeaderTitle.Text = "📊 Báo Cáo & Thống Kê Nhà Hàng";
            lblHeaderTitle.Font = UITheme.FontTitle;
            lblHeaderTitle.ForeColor = UITheme.PrimaryDark;
            lblHeaderTitle.AutoSize = true;
            lblHeaderTitle.Location = new Point(18, 14);

            pnlCards.Dock = DockStyle.Top;
            pnlCards.Height = 80;
            pnlCards.BackColor = Color.White;
            pnlCards.Padding = new Padding(15, 10, 15, 10);
            pnlRevenueCard.BackColor = UITheme.Primary;
            pnlRevenueCard.SetBounds(18, 10, 240, 60);
            pnlRevenueCard.Padding = new Padding(8);
            lblRevenueCardTitle.Text = "💰 Doanh thu hôm nay";
            lblRevenueCardTitle.SetBounds(8, 4, 220, 16);
            lblRevenueCardTitle.Font = UITheme.FontSmall;
            lblRevenueCardTitle.ForeColor = Color.FromArgb(220, 255, 255, 255);
            lblRevenueToday.SetBounds(8, 22, 220, 32);
            lblRevenueToday.Font = new Font("Segoe UI", 15f, FontStyle.Bold);
            lblRevenueToday.ForeColor = Color.White;
            pnlTotalRevenueCard.BackColor = UITheme.Success;
            pnlTotalRevenueCard.SetBounds(275, 10, 240, 60);
            pnlTotalRevenueCard.Padding = new Padding(8);
            lblTotalRevenueCardTitle.Text = "📈 Tổng doanh thu lọc";
            lblTotalRevenueCardTitle.SetBounds(8, 4, 220, 16);
            lblTotalRevenueCardTitle.Font = UITheme.FontSmall;
            lblTotalRevenueCardTitle.ForeColor = Color.FromArgb(220, 255, 255, 255);
            lblTotalRevenueCard.SetBounds(8, 22, 220, 32);
            lblTotalRevenueCard.Font = new Font("Segoe UI", 15f, FontStyle.Bold);
            lblTotalRevenueCard.ForeColor = Color.White;

            pnlFilter.Dock = DockStyle.Top;
            pnlFilter.Height = 52;
            pnlFilter.BackColor = Color.White;
            pnlFilter.Padding = new Padding(15, 8, 15, 8);
            lblFrom.Text = "Từ:";
            lblFrom.Font = UITheme.FontNormal;
            lblFrom.AutoSize = true;
            lblFrom.Location = new Point(15, 15);
            dtpFrom.Format = DateTimePickerFormat.Short;
            dtpFrom.SetBounds(45, 11, 115, 28);
            dtpFrom.Font = UITheme.FontNormal;
            if (dtpFrom.Value.Date == DateTime.Today)
                dtpFrom.Value = DateTime.Today.AddDays(-30);
            lblTo.Text = "Đến:";
            lblTo.Font = UITheme.FontNormal;
            lblTo.AutoSize = true;
            lblTo.Location = new Point(175, 15);
            dtpTo.Format = DateTimePickerFormat.Short;
            dtpTo.SetBounds(215, 11, 115, 28);
            dtpTo.Font = UITheme.FontNormal;
            btnSearch.Text = "🔍 Xem báo cáo";
            btnSearch.SetBounds(350, 10, 135, 30);
            btnToday.Text = "Hôm nay";
            btnToday.SetBounds(495, 10, 85, 30);
            btnThisMonth.Text = "Tháng này";
            btnThisMonth.SetBounds(590, 10, 95, 30);
            btnAllTime.Text = "Tất cả";
            btnAllTime.SetBounds(695, 10, 80, 30);

            pnlSummary.Dock = DockStyle.Bottom;
            pnlSummary.Height = 38;
            pnlSummary.BackColor = UITheme.PrimaryDark;
            lblSummary.Dock = DockStyle.Fill;
            lblSummary.Font = new Font(UITheme.FontFamily, 9.5f, FontStyle.Bold);
            lblSummary.ForeColor = Color.White;
            lblSummary.TextAlign = ContentAlignment.MiddleCenter;

            tabControlStats.Dock = DockStyle.Fill;
            tabControlStats.Font = new Font(UITheme.FontFamily, 10f, FontStyle.Bold);
            tabControlStats.Padding = new Point(14, 6);
            tabRevenue.BackColor = UITheme.Light;
            tabTopDishes.BackColor = UITheme.Light;
            tabPayment.BackColor = UITheme.Light;
            tabCookPerf.BackColor = UITheme.Light;

            // Dock order: report tabs fill the center, with summary/filter/cards/header around them.
            Controls.SetChildIndex(tabControlStats, 0);
            Controls.SetChildIndex(pnlSummary, 1);
            Controls.SetChildIndex(pnlFilter, 2);
            Controls.SetChildIndex(pnlCards, 3);
            Controls.SetChildIndex(pnlHeader, 4);

            splitRevenue.Dock = DockStyle.Fill;
            splitRevenue.BackColor = UITheme.Light;
            splitRevenue.BorderStyle = BorderStyle.FixedSingle;
            splitRevenue.SizeChanged += (_, _) => LayoutRevenueSplit();
            dgvRevenue.Dock = DockStyle.Fill;
            chartRevenue.Dock = DockStyle.Fill;
            dgvTopDishes.Dock = DockStyle.Fill;
            dgvCookPerf.Dock = DockStyle.Fill;
            paymentLayout.Dock = DockStyle.Fill;

            ConfigureReportColumns();
            LayoutRevenueSplit();
        }

        private void LayoutRevenueSplit()
        {
            int width = splitRevenue.ClientSize.Width;
            int height = splitRevenue.ClientSize.Height;
            if (width >= 1000)
            {
                splitRevenue.Orientation = Orientation.Vertical;
                splitRevenue.Panel1MinSize = 320;
                splitRevenue.Panel2MinSize = 380;
                if (width >= splitRevenue.Panel1MinSize + splitRevenue.Panel2MinSize + splitRevenue.SplitterWidth)
                {
                    int maxDistance = width - splitRevenue.Panel2MinSize - splitRevenue.SplitterWidth;
                    splitRevenue.SplitterDistance = Math.Clamp((int)(width * 0.42), splitRevenue.Panel1MinSize, maxDistance);
                }
            }
            else
            {
                splitRevenue.Orientation = Orientation.Horizontal;
                splitRevenue.Panel1MinSize = 140;
                splitRevenue.Panel2MinSize = 180;
                if (height >= splitRevenue.Panel1MinSize + splitRevenue.Panel2MinSize + splitRevenue.SplitterWidth)
                {
                    int maxDistance = height - splitRevenue.Panel2MinSize - splitRevenue.SplitterWidth;
                    splitRevenue.SplitterDistance = Math.Clamp((int)(height * 0.42), splitRevenue.Panel1MinSize, maxDistance);
                }
            }
        }

        private void ConfigureReportColumns()
        {
            dgvRevenue.AutoGenerateColumns = false;
            dgvRevenue.Columns.Clear();
            dgvRevenue.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colRevenueDate", HeaderText = "Ngày", DataPropertyName = "Ngày", FillWeight = 35,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });
            dgvRevenue.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colRevenueOrders", HeaderText = "Số đơn", DataPropertyName = "Số đơn", FillWeight = 25,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, Format = "#,##0" }
            });
            dgvRevenue.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colRevenueAmount", HeaderText = "Doanh thu (đ)", DataPropertyName = "Doanh thu (đ)", FillWeight = 40,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "#,##0" }
            });

            dgvTopDishes.AutoGenerateColumns = true;
            dgvTopDishes.Columns.Clear();

            dgvPayment.AutoGenerateColumns = false;
            dgvPayment.Columns.Clear();
            dgvPayment.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPaymentMethod", HeaderText = "Phương thức", DataPropertyName = "Phương thức", FillWeight = 30
            });
            dgvPayment.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPaymentOrders", HeaderText = "Số đơn", DataPropertyName = "Số đơn", FillWeight = 18,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, Format = "#,##0" }
            });
            dgvPayment.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPaymentAmount", HeaderText = "Doanh thu (đ)", DataPropertyName = "Doanh thu (đ)", FillWeight = 30,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "#,##0" }
            });
            dgvPayment.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPaymentShare", HeaderText = "Tỷ lệ %", DataPropertyName = "Tỷ lệ %", FillWeight = 22,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "P2" }
            });

            dgvCookPerf.AutoGenerateColumns = false;
            dgvCookPerf.Columns.Clear();
            dgvCookPerf.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCookItemName", HeaderText = "Tên món", DataPropertyName = "Tên món", FillWeight = 200
            });
            dgvCookPerf.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCookCount", HeaderText = "Số lần chế biến", DataPropertyName = "Số lần chế biến", FillWeight = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "#,##0", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvCookPerf.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCookAverage", HeaderText = "Thời gian TB (phút)", DataPropertyName = "Thời gian TB (phút)", FillWeight = 140,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "0.0", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvCookPerf.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCookFastest", HeaderText = "Nhanh nhất (phút)", DataPropertyName = "Nhanh nhất (phút)", FillWeight = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "0.0", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvCookPerf.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCookSlowest", HeaderText = "Chậm nhất (phút)", DataPropertyName = "Chậm nhất (phút)", FillWeight = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "0.0", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
        }

        private void ConfigureCharts()
        {
            // The current designer layout does not instantiate these chart controls.
            // Create them here so the report handlers always have initialized charts.
            chartRevenue = new Chart
            {
                Name = "chartRevenue",
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
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
            chartRevenue.Titles.Add(new Title(
                "Doanh thu theo ngày", Docking.Top,
                new Font("Segoe UI", 11f, FontStyle.Bold), UITheme.TextPrimary));
            chartRevenue.Legends.Add(new Legend { Enabled = false });
            chartRevenue.Series.Add(new Series("Doanh thu")
            {
                ChartArea = "RevenueArea",
                ChartType = SeriesChartType.Column,
                XValueType = ChartValueType.Date,
                Color = UITheme.Primary,
                BorderWidth = 2,
                ToolTip = "#VALX{dd/MM/yyyy}: #VALY{#,##0} đ"
            });
            splitRevenue.Panel2.Controls.Add(chartRevenue);

            chartPayment = new Chart
            {
                Name = "chartPayment",
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Palette = ChartColorPalette.BrightPastel
            };
            paymentArea = new ChartArea("PaymentArea")
            {
                BackColor = Color.White
            };
            paymentArea.AxisX.Enabled = AxisEnabled.False;
            paymentArea.AxisY.Enabled = AxisEnabled.False;
            chartPayment.ChartAreas.Add(paymentArea);
            chartPayment.Titles.Add(new Title(
                "Tỷ lệ doanh thu theo phương thức", Docking.Top,
                new Font("Segoe UI", 11f, FontStyle.Bold), UITheme.TextPrimary));
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
                XValueMember = "Phương thức",
                YValueMembers = "Doanh thu (đ)",
                Label = "#PERCENT{P0}",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ToolTip = "#VALX: #VALY{#,##0} đ (#PERCENT{P1})"
            });

            paymentLayout.ColumnCount = 2;
            paymentLayout.RowCount = 1;
            paymentLayout.ColumnStyles.Clear();
            paymentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 56f));
            paymentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 44f));
            paymentLayout.RowStyles.Clear();
            paymentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            paymentLayout.Controls.Add(chartPayment, 1, 0);
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
