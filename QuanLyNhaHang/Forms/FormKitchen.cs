using QuanLyNhaHang.Helpers;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Service;

namespace QuanLyNhaHang.Forms
{
    public partial class FormKitchen : Form
    {
        private readonly KitchenService _kitchenSvc = new();
        private readonly ServiceTimeService _timeSvc = new();
        private System.Windows.Forms.Timer _refreshTimer = null!;
        private List<OrderDetail> _items = new();

        public FormKitchen()
        {
            InitializeComponent();
        }

        private async void FormKitchen_Load(object sender, EventArgs e)
        {
            await LoadKitchenBoardAsync();
            await LoadServiceTimeTrackingAsync();

            _refreshTimer = new System.Windows.Forms.Timer { Interval = 5000 };
            _refreshTimer.Tick += async (s, ev) =>
            {
                if (tabControlMain.SelectedTab == tabKds)
                    await LoadKitchenBoardAsync();
            };
            _refreshTimer.Start();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _refreshTimer?.Stop();
            _refreshTimer?.Dispose();
            base.OnFormClosed(e);
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab == tabKds)
                await LoadKitchenBoardAsync();
            else
                await LoadServiceTimeTrackingAsync();
        }

        // ==================== TAB 1: KDS BẾP ====================
        private async Task LoadKitchenBoardAsync()
        {
            try
            {
                _items = await _kitchenSvc.GetPendingItemsAsync();
                RenderKitchenCards();
                UpdateCounters();
            }
            catch (Exception ex)
            {
                if (!_refreshTimer.Enabled)
                    ValidationHelper.ShowError("Lỗi tải dữ liệu bếp: " + ex.Message);
            }
        }

        private void RenderKitchenCards()
        {
            pnlNew.Controls.Clear();
            pnlCooking.Controls.Clear();
            pnlReady.Controls.Clear();

            foreach (var item in _items)
            {
                var card = CreateKitchenCard(item);
                if (item.ItemStatus == "Cho bep")
                    pnlNew.Controls.Add(card);
                else if (item.ItemStatus == "Dang che bien")
                    pnlCooking.Controls.Add(card);
                else if (item.ItemStatus == "San sang")
                    pnlReady.Controls.Add(card);
            }
        }

        private Panel CreateKitchenCard(OrderDetail item)
        {
            var isCooking = item.ItemStatus == "Dang che bien";
            var isReady = item.ItemStatus == "San sang";
            var isUrgent = item.WaitTimeMinutes > 15 || item.CookingTimeMinutes > 15;
            var hasNote = !string.IsNullOrWhiteSpace(item.Note);
            int actionY = hasNote ? 174 : 152;

            var card = new Panel
            {
                Size = new Size(320, hasNote ? 224 : 200),
                BackColor = Color.White,
                Margin = new Padding(6),
                Tag = item
            };
            card.Paint += (s, e) =>
            {
                using var pen = new Pen(isUrgent ? UITheme.Danger : item.StatusColor, 2);
                e.Graphics.DrawRectangle(pen, 1, 1, card.Width - 3, card.Height - 3);
            };

            // Badge Header
            var pnlBadge = new Panel { BackColor = item.StatusColor, Size = new Size(320, 30), Location = new Point(0, 0) };
            var lblBadge = new Label
            {
                Text = $"{item.StatusDisplay} • {(string.IsNullOrEmpty(item.TableName) ? "Mang về" : item.TableName)}",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlBadge.Controls.Add(lblBadge);

            // Order ID & Time
            var lblOrderInfo = new Label
            {
                Text = $"Đơn #{item.OrderID} • Gọi lúc: {item.OrderedAt:HH:mm}",
                Font = UITheme.FontSmall,
                ForeColor = UITheme.TextMuted,
                AutoSize = true,
                Location = new Point(12, 38)
            };

            // Item Name & Quantity
            var lblName = new Label
            {
                Text = item.ItemName,
                Font = new Font("Segoe UI", 11.5f, FontStyle.Bold),
                ForeColor = UITheme.TextPrimary,
                AutoSize = false,
                Size = new Size(240, 42),
                Location = new Point(12, 58),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblQty = new Label
            {
                Text = $"x{item.Quantity}",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = UITheme.Primary,
                AutoSize = true,
                Location = new Point(265, 62)
            };

            // Timer display
            string timeText;
            if (isReady)
            {
                timeText = $"⏱ Đã nấu: {(int)item.CookingTimeMinutes}p | 🛎 Chờ bưng: {(int)item.DeliveryTimeMinutes}p";
            }
            else if (isCooking)
            {
                timeText = $"⏱ Đang nấu: {(int)item.CookingTimeMinutes}p {(int)((item.CookingTimeMinutes % 1) * 60)}s";
            }
            else
            {
                timeText = $"⏰ Đã chờ: {(int)item.WaitTimeMinutes}p {(int)((item.WaitTimeMinutes % 1) * 60)}s";
            }

            var lblTimer = new Label
            {
                Text = timeText,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = isUrgent ? UITheme.Danger : UITheme.TextMuted,
                AutoSize = true,
                Location = new Point(12, 105)
            };

            // Note (if any)
            if (hasNote)
            {
                var lblNote = new Label
                {
                    Text = $"📝 Ghi chú: {item.Note}",
                    Font = UITheme.FontSmall,
                    ForeColor = Color.FromArgb(211, 84, 0),
                    BackColor = Color.FromArgb(255, 247, 237),
                    AutoSize = false,
                    Size = new Size(296, 40),
                    Location = new Point(12, 126),
                    Padding = new Padding(4, 2, 4, 2),
                    AutoEllipsis = true
                };
                card.Controls.Add(lblNote);
            }

            // Action Button
            Button? btnAction = null;
            if (item.ItemStatus == "Cho bep")
            {
                btnAction = UITheme.CreateButton("▶ Bắt đầu chế biến", Color.FromArgb(230, 126, 34));
                btnAction.Size = new Size(296, 34); btnAction.Location = new Point(12, actionY);
                btnAction.Click += async (s, e) =>
                {
                    btnAction.Enabled = false;
                    await RunKitchenWriteAsync(() => _kitchenSvc.StartCookingAsync(item.OrderDetailID));
                    await LoadKitchenBoardAsync();
                };
            }
            else if (isCooking)
            {
                btnAction = UITheme.CreateButton("✅ Hoàn thành nấu", Color.FromArgb(39, 174, 96));
                btnAction.Size = new Size(296, 34); btnAction.Location = new Point(12, actionY);
                btnAction.Click += async (s, e) =>
                {
                    btnAction.Enabled = false;
                    await RunKitchenWriteAsync(() => _kitchenSvc.FinishCookingAsync(item.OrderDetailID));
                    await LoadKitchenBoardAsync();
                };
            }
            else if (isReady)
            {
                btnAction = UITheme.CreateButton("🛎 Đã phục vụ ra bàn", UITheme.Primary);
                btnAction.Size = new Size(296, 34); btnAction.Location = new Point(12, actionY);
                btnAction.Click += async (s, e) =>
                {
                    btnAction.Enabled = false;
                    await RunKitchenWriteAsync(() => _kitchenSvc.ServeItemAsync(item.OrderDetailID));
                    await LoadKitchenBoardAsync();
                };
            }

            card.Controls.AddRange(new Control[] { pnlBadge, lblOrderInfo, lblName, lblQty, lblTimer });
            if (btnAction != null) card.Controls.Add(btnAction);

            return card;
        }

        private async Task RunKitchenWriteAsync(Func<Task> write)
        {
            bool previousWaitCursor = UseWaitCursor;
            Cursor previousCursor = Cursor;
            UseWaitCursor = true;
            Cursor = Cursors.WaitCursor;
            try
            {
                await write();
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError("Không thể cập nhật trạng thái món: " + ex.Message);
            }
            finally
            {
                Cursor = previousCursor;
                UseWaitCursor = previousWaitCursor;
            }
        }

        private void UpdateCounters()
        {
            int newCount = _items.Count(i => i.ItemStatus == "Cho bep");
            int cookCount = _items.Count(i => i.ItemStatus == "Dang che bien");
            int readyCount = _items.Count(i => i.ItemStatus == "San sang");

            lblCountNew.Text = $"{newCount} món";
            lblCountCooking.Text = $"{cookCount} món";
            lblCountReady.Text = $"{readyCount} món";
        }

        // ==================== TAB 2: THEO DÕI THỜI GIAN ====================
        private async void btnFilterLogs_Click(object sender, EventArgs e)
        {
            await LoadServiceTimeTrackingAsync();
        }

        private async void btnLogsToday_Click(object sender, EventArgs e)
        {
            dtpFrom.Value = DateTime.Today;
            dtpTo.Value = DateTime.Today;
            await LoadServiceTimeTrackingAsync();
        }

        private async Task LoadServiceTimeTrackingAsync()
        {
            try
            {
                var from = dtpFrom.Value.Date;
                var to = dtpTo.Value.Date;
                var status = cboStatusFilter.SelectedItem?.ToString();

                // Load KPIs
                var kpis = await _timeSvc.GetKPIsAsync(from, to);
                lblKpiWait.Text = $"{kpis.AvgWaitMinutes:F1} phút";
                lblKpiCook.Text = $"{kpis.AvgCookingMinutes:F1} phút";
                lblKpiDeliver.Text = $"{kpis.AvgDeliveryMinutes:F1} phút";
                lblKpiOverdue.Text = $"{kpis.OverdueCount} món";

                // Load Grid
                var logs = await _timeSvc.GetServiceTimeLogsAsync(from, to, status);
                dgvTiming.Rows.Clear();

                foreach (var item in logs)
                {
                    var idx = dgvTiming.Rows.Add(
                        $"#{item.OrderID}",
                        string.IsNullOrEmpty(item.TableName) ? "---" : item.TableName,
                        item.ItemName,
                        item.Quantity,
                        item.StatusDisplay,
                        item.OrderedAt.ToString("HH:mm:ss"),
                        $"{item.WaitTimeMinutes:F1}",
                        item.CookingStartedAt.HasValue ? $"{item.CookingTimeMinutes:F1}" : "--",
                        item.CookingCompletedAt.HasValue ? $"{item.DeliveryTimeMinutes:F1}" : "--",
                        $"{item.TotalServiceMinutes:F1}",
                        item.ServiceSpeedRating
                    );

                    var row = dgvTiming.Rows[idx];
                    row.Cells[10].Style.ForeColor = item.ServiceSpeedColor;
                    row.Cells[10].Style.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                }
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError("Lỗi tải nhật ký theo dõi thời gian: " + ex.Message);
            }
        }
    }
}
