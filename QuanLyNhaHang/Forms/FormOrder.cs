using QuanLyNhaHang.Data;
using QuanLyNhaHang.Helpers;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Service;
using MenuItemModel = QuanLyNhaHang.Models.MenuItem;
using System.Globalization;
using System.Text;

namespace QuanLyNhaHang.Forms
{
    public partial class FormOrder : Form
    {
        private Table? _table;
        private Order? _currentOrder;
        private List<MenuItemModel> _menuItems = new();
        private List<Category> _categories = new();
        private List<Table> _allTables = new();
        private readonly OrderService _orderSvc = new();
        private readonly MenuRepository _menuRepo = new();
        private readonly TableService _tableSvc = new();
        private readonly System.Windows.Forms.Timer _searchDebounceTimer = new() { Interval = 250 };
        private readonly System.Windows.Forms.Timer _orderRefreshTimer = new() { Interval = 5000 };
        private readonly SemaphoreSlim _dbWriteGate = new(1, 1);
        private readonly SemaphoreSlim _imageLoadGate = new(4);
        private readonly Dictionary<string, Image> _menuImageCache = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Task<Image?>> _pendingImageLoads = new(StringComparer.OrdinalIgnoreCase);
        private static readonly string[] QuickNoteOptions = { "Ít cay", "Không cay", "Không hành", "Ít đá", "Không đường", "Mang về" };
        private FlowLayoutPanel flowCategories = null!;
        private FlowLayoutPanel flowCart = null!;
        private Label lblMenuEmpty = null!;
        private Panel menuHost = null!;
        private OrderDetail? _selectedCartDetail;
        private Button btnClearCart = null!;
        private Label lblSubtotalValue = null!;
        private Label lblDiscountValue = null!;
        private NumericUpDown numCartDiscount = null!;
        private ComboBox cboCartDiscountMode = null!;
        private Button btnSendKitchen = null!;
        private bool _isSavingOrder;
        private bool _isRefreshingOrder;
        private bool _isModalDialogOpen;

        public FormOrder(Table? table = null)
        {
            _table = table;
            UITheme.ConfigureDpi(this);
            InitializeComponent();
            UITheme.EnableDoubleBuffering(this);
            split.SizeChanged += split_SizeChanged;
            pnlHeader.SizeChanged += pnlHeader_SizeChanged;
            ConfigureMenuArea();
            ConfigureCartArea();
            _searchDebounceTimer.Tick += SearchDebounceTimer_Tick;
            _orderRefreshTimer.Tick += OrderRefreshTimer_Tick;
            KeyPreview = true;
            KeyDown += FormOrder_KeyDown;
            FormClosed += FormOrder_FormClosed;
        }

        private void ConfigureCartArea()
        {
            var cartLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Color.White,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            cartLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            cartLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            cartLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            cartLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 218));

            var header = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(12, 0, 10, 0)
            };
            lblOrderTitle.Parent?.Controls.Remove(lblOrderTitle);
            lblItemCount.Parent?.Controls.Remove(lblItemCount);
            lblOrderTitle.Dock = DockStyle.None;
            lblOrderTitle.AutoSize = false;
            lblOrderTitle.Location = new Point(12, 5);
            lblOrderTitle.Size = new Size(205, 38);
            lblOrderTitle.Padding = Padding.Empty;
            lblOrderTitle.Text = "Đơn hàng";
            lblOrderTitle.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
            lblOrderTitle.AutoEllipsis = true;

            lblItemCount.AutoSize = false;
            lblItemCount.TextAlign = ContentAlignment.MiddleCenter;
            lblItemCount.BackColor = UITheme.PrimarySoft;
            lblItemCount.ForeColor = UITheme.Primary;
            lblItemCount.Font = new Font(UITheme.FontFamily, 9f, FontStyle.Bold);
            lblItemCount.Location = new Point(220, 12);
            lblItemCount.Size = new Size(74, 24);
            lblItemCount.Text = "0 món";
            ApplyRoundedRegion(lblItemCount, 12);

            btnClearCart = new Button
            {
                Text = "Xóa tất cả",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = UITheme.Danger,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                Size = new Size(82, 28),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            btnClearCart.FlatAppearance.BorderSize = 0;
            btnClearCart.Click += btnClearCart_Click;
            header.Controls.Add(lblOrderTitle);
            header.Controls.Add(lblItemCount);
            header.Controls.Add(btnClearCart);
            header.Resize += (_, _) => btnClearCart.Location = new Point(header.ClientSize.Width - btnClearCart.Width - 10, 10);

            var cartHost = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            flowCart = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = false,
                FlowDirection = FlowDirection.TopDown,
                BackColor = Color.White,
                Padding = new Padding(8, 4, 8, 8),
                Margin = new Padding(0)
            };
            typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(flowCart, true);
            flowCart.SizeChanged += (_, _) => ResizeCartRows();

            var empty = new Label
            {
                Dock = DockStyle.Fill,
                Text = "🛒\r\nChưa có món nào. Chọn món bên trái để bắt đầu",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = UITheme.FontNormal,
                ForeColor = UITheme.TextMuted,
                Visible = false
            };
            cartHost.Controls.Add(flowCart);
            cartHost.Controls.Add(empty);
            empty.BringToFront();
            flowCart.Tag = empty;

            btnDecrease.Visible = false;
            btnIncrease.Visible = false;
            btnRemoveItem.Visible = false;
            pnlBottom.Dock = DockStyle.Fill;
            ConfigureOrderFooter();
            cartLayout.Controls.Add(header, 0, 0);
            cartLayout.Controls.Add(cartHost, 0, 1);
            cartLayout.Controls.Add(pnlBottom, 0, 2);
            split.Panel2.Controls.Add(cartLayout);
            cartLayout.BringToFront();
            dgvOrder.Visible = false;
            pnlOrderStats.Visible = false;
        }

        private void ConfigureOrderFooter()
        {
            pnlBottom.Padding = new Padding(12, 8, 12, 8);
            pnlBottom.BorderStyle = BorderStyle.None;
            pnlBottom.BackColor = Color.White;
            pnlBottom.Controls.Add(new Label { Text = "Tạm tính", AutoSize = true, Font = UITheme.FontNormal,
                ForeColor = UITheme.TextMuted, Location = new Point(12, 8) });
            lblSubtotalValue = new Label { Text = "0 đ", AutoSize = true, Font = UITheme.FontNormal,
                ForeColor = UITheme.TextPrimary, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            pnlBottom.Controls.Add(lblSubtotalValue);

            pnlBottom.Controls.Add(new Label { Text = "Giảm giá", AutoSize = true, Font = UITheme.FontNormal,
                ForeColor = UITheme.TextMuted, Location = new Point(12, 36) });
            cboCartDiscountMode = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList,
                Font = UITheme.FontSmall, Width = 62, Height = 26, Location = new Point(105, 31) };
            cboCartDiscountMode.Items.AddRange(new object[] { "%", "đ" });
            cboCartDiscountMode.SelectedIndex = 0;
            numCartDiscount = new NumericUpDown { Minimum = 0, Maximum = 100, DecimalPlaces = 2,
                ThousandsSeparator = true, Font = UITheme.FontSmall, Width = 105, Height = 26,
                Location = new Point(172, 31) };
            lblDiscountValue = new Label { Text = "− 0 đ", AutoSize = true, Font = UITheme.FontNormal,
                ForeColor = UITheme.Danger, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            pnlBottom.Controls.AddRange(new Control[] { cboCartDiscountMode, numCartDiscount, lblDiscountValue });
            cboCartDiscountMode.SelectedIndexChanged += (_, _) =>
            {
                numCartDiscount.Maximum = cboCartDiscountMode.SelectedIndex == 0 ? 100 : 100_000_000;
                numCartDiscount.DecimalPlaces = cboCartDiscountMode.SelectedIndex == 0 ? 2 : 0;
                numCartDiscount.Value = 0;
                RefreshOrderSummary();
            };
            numCartDiscount.ValueChanged += (_, _) => RefreshOrderSummary();

            lblTotal.Text = "TỔNG TIỀN: 0 đ";
            lblTotal.Font = new Font("Segoe UI", 14f, FontStyle.Bold);
            lblTotal.ForeColor = UITheme.TextPrimary;
            lblTotal.Location = new Point(12, 63);
            lblTotal.Size = new Size(300, 30);
            lblTotal.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            btnSendKitchen = UITheme.CreateButton("Gửi bếp (F5)", UITheme.Primary);
            btnSendKitchen.Size = new Size(150, 38);
            btnSendKitchen.Location = new Point(12, 105);
            btnSendKitchen.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnSendKitchen.Click += btnSendKitchen_Click;
            // This button is created at runtime, so it must be added to the footer
            // before SetChildIndex is used below.
            pnlBottom.Controls.Add(btnSendKitchen);
            btnCheckout.Text = "Thanh toán (F9)";
            btnCheckout.BackColor = UITheme.Success;
            btnCheckout.Location = new Point(170, 105);
            btnCheckout.Size = new Size(160, 38);
            btnCheckout.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancelOrder.Text = "Hủy đơn";
            btnCancelOrder.BackColor = Color.White;
            btnCancelOrder.ForeColor = UITheme.Danger;
            btnCancelOrder.FlatStyle = FlatStyle.Flat;
            btnCancelOrder.FlatAppearance.BorderColor = UITheme.Danger;
            btnCancelOrder.FlatAppearance.BorderSize = 1;
            btnCancelOrder.Location = new Point(12, 157);
            btnCancelOrder.Size = new Size(98, 32);
            btnCancelOrder.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            pnlBottom.Resize += (_, _) => LayoutOrderFooter();
            LayoutOrderFooter();
            pnlBottom.Controls.SetChildIndex(btnCancelOrder, 0);
            pnlBottom.Controls.SetChildIndex(btnCheckout, 0);
            pnlBottom.Controls.SetChildIndex(btnSendKitchen, 0);
        }

        private void LayoutOrderFooter()
        {
            if (lblSubtotalValue == null) return;
            lblSubtotalValue.Location = new Point(Math.Max(160, pnlBottom.ClientSize.Width - 120), 8);
            lblDiscountValue.Location = new Point(Math.Max(160, pnlBottom.ClientSize.Width - 120), 36);
            btnCheckout.Location = new Point(Math.Max(170, pnlBottom.ClientSize.Width - btnCheckout.Width - 12), 105);
            btnSendKitchen.Width = Math.Max(125, btnCheckout.Left - btnSendKitchen.Left - 8);
        }

        private void ConfigureMenuArea()
        {
            var leftLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = UITheme.Light,
                Padding = new Padding(8, 6, 8, 0)
            };
            leftLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            leftLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
            leftLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            leftLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
            leftLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var searchPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10, 0, 8, 0),
                Margin = new Padding(0, 0, 0, 6)
            };
            var searchIcon = new Label
            {
                Text = "🔍",
                Dock = DockStyle.Left,
                Width = 28,
                Font = new Font("Segoe UI Emoji", 12f, FontStyle.Regular),
                ForeColor = UITheme.TextMuted,
                TextAlign = ContentAlignment.MiddleCenter
            };
            txtMenuSearch.Dock = DockStyle.Fill;
            txtMenuSearch.BorderStyle = BorderStyle.None;
            txtMenuSearch.Font = new Font("Segoe UI", 11f);
            txtMenuSearch.PlaceholderText = "Tìm món (F2)...";
            txtMenuSearch.Margin = new Padding(0);
            searchPanel.Controls.Add(txtMenuSearch);
            searchPanel.Controls.Add(searchIcon);

            flowCategories = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = UITheme.Light,
                Margin = new Padding(0),
                Padding = new Padding(0, 3, 0, 3)
            };

            var notePanel = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.Light };
            lblNoteTitle.Dock = DockStyle.Top;
            lblNoteTitle.Height = 20;
            lblNoteTitle.Padding = new Padding(2, 1, 0, 0);
            txtItemNote.Dock = DockStyle.Fill;
            txtItemNote.Margin = new Padding(0);
            notePanel.Controls.Add(txtItemNote);
            notePanel.Controls.Add(lblNoteTitle);

            menuHost = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.Light };
            flowMenu.Dock = DockStyle.Fill;
            flowMenu.WrapContents = true;
            flowMenu.FlowDirection = FlowDirection.LeftToRight;
            flowMenu.AutoScroll = true;
            flowMenu.Margin = new Padding(0);
            typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(flowMenu, true);
            lblMenuEmpty = new Label
            {
                Dock = DockStyle.Fill,
                Text = "Không tìm thấy món phù hợp",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 11f),
                ForeColor = UITheme.TextMuted,
                Visible = false
            };
            menuHost.Controls.Add(flowMenu);
            menuHost.Controls.Add(lblMenuEmpty);
            lblMenuEmpty.BringToFront();

            leftLayout.Controls.Add(searchPanel, 0, 0);
            leftLayout.Controls.Add(flowCategories, 0, 1);
            leftLayout.Controls.Add(notePanel, 0, 2);
            leftLayout.Controls.Add(menuHost, 0, 3);

            tabCategories.Visible = false;
            split.Panel1.Controls.Add(leftLayout);
            leftLayout.BringToFront();
        }

        private void split_SizeChanged(object? sender, EventArgs e)
        {
            // Keep both sides usable when this form is hosted in the main window.
            int minDistance = split.Panel1MinSize;
            int maxDistance = split.ClientSize.Width - split.SplitterWidth - split.Panel2MinSize;
            if (maxDistance < minDistance) return;

            int desiredDistance = (int)(split.ClientSize.Width * 0.65);
            int distance = Math.Clamp(desiredDistance, minDistance, maxDistance);
            if (split.SplitterDistance != distance)
                split.SplitterDistance = distance;
        }

        private void pnlHeader_SizeChanged(object? sender, EventArgs e)
        {
            int width = pnlHeader.ClientSize.Width;
            int height = pnlHeader.ClientSize.Height;
            btnBack.Location = new Point(15, Math.Max(0, (height - btnBack.Height) / 2));

            cboTableSelect.Width = Math.Min(220, Math.Max(150, width / 5));
            cboTableSelect.Location = new Point(Math.Max(btnBack.Right + 24, width - cboTableSelect.Width - 15),
                Math.Max(0, (height - cboTableSelect.Height) / 2));

            int left = btnBack.Right + 16;
            int right = cboTableSelect.Left - 16;
            int available = Math.Max(0, right - left);
            lblTableName.Width = Math.Min(310, Math.Max(180, available - lblTitle.Width - 12));
            int groupWidth = lblTitle.Width + 12 + lblTableName.Width;
            int groupLeft = left + Math.Max(0, (available - groupWidth) / 2);
            lblTitle.Location = new Point(groupLeft, Math.Max(0, (height - lblTitle.Height) / 2));
            lblTableName.Location = new Point(lblTitle.Right + 12, Math.Max(0, (height - lblTableName.Height) / 2));
        }

        private async void FormOrder_Load(object sender, EventArgs e)
        {
            await LoadTablesDropdownAsync();
            await LoadMenuAsync();

            if (_table != null)
            {
                SelectTableInDropdown(_table.TableID);
                await SwitchToTableAsync(_table);
            }
            else if (_allTables.Count > 0)
            {
                cboTableSelect.SelectedIndex = 0;
            }
            _orderRefreshTimer.Start();
        }

        private async void OrderRefreshTimer_Tick(object? sender, EventArgs e)
        {
            if (_isRefreshingOrder || _isSavingOrder || _isModalDialogOpen || _currentOrder == null || IsDisposed) return;
            int orderId = _currentOrder.OrderID;
            _isRefreshingOrder = true;
            try
            {
                var latestOrder = await _orderSvc.GetOrderAsync(orderId);
                if (latestOrder != null && _currentOrder?.OrderID == orderId && !_isModalDialogOpen)
                {
                    _currentOrder = latestOrder;
                    RefreshOrderSummary();
                }
            }
            catch
            {
                // A transient refresh failure should not interrupt order entry.
            }
            finally
            {
                _isRefreshingOrder = false;
            }
        }

        private async Task<bool> RunDbWriteAsync(Func<Task> write)
        {
            await _dbWriteGate.WaitAsync();
            bool previousWaitCursor = UseWaitCursor;
            Cursor previousCursor = Cursor;
            _isSavingOrder = true;
            UseWaitCursor = true;
            Cursor = Cursors.WaitCursor;
            try
            {
                await write();
                return true;
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError("Không thể lưu thay đổi: " + ex.Message);
                return false;
            }
            finally
            {
                Cursor = previousCursor;
                UseWaitCursor = previousWaitCursor;
                _isSavingOrder = false;
                _dbWriteGate.Release();
            }
        }

        private async Task<T> RunDbWriteAsync<T>(Func<Task<T>> write)
        {
            await _dbWriteGate.WaitAsync();
            bool previousWaitCursor = UseWaitCursor;
            Cursor previousCursor = Cursor;
            _isSavingOrder = true;
            UseWaitCursor = true;
            Cursor = Cursors.WaitCursor;
            try
            {
                return await write();
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError("Không thể lưu thay đổi: " + ex.Message);
                return default!;
            }
            finally
            {
                Cursor = previousCursor;
                UseWaitCursor = previousWaitCursor;
                _isSavingOrder = false;
                _dbWriteGate.Release();
            }
        }

        private async Task LoadTablesDropdownAsync()
        {
            _allTables = await _tableSvc.GetAllTablesAsync();
            cboTableSelect.Items.Clear();
            foreach (var t in _allTables)
            {
                cboTableSelect.Items.Add(new TableDropdownItem(t));
            }
        }

        private void SelectTableInDropdown(int tableId)
        {
            for (int i = 0; i < cboTableSelect.Items.Count; i++)
            {
                if (cboTableSelect.Items[i] is TableDropdownItem item && item.Table.TableID == tableId)
                {
                    cboTableSelect.SelectedIndex = i;
                    break;
                }
            }
        }

        private async void cboTableSelect_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cboTableSelect.SelectedItem is TableDropdownItem item)
            {
                _table = item.Table;
                await SwitchToTableAsync(_table);
            }
        }

        private void cboTableSelect_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= cboTableSelect.Items.Count) return;

            e.DrawBackground();
            if (cboTableSelect.Items[e.Index] is TableDropdownItem item)
            {
                string text = $"{item.Table.TableName}  •  {item.Table.StatusDisplay}";
                using var brush = new SolidBrush(item.Table.StatusColor);
                using var format = new StringFormat { LineAlignment = StringAlignment.Center };
                e.Graphics.DrawString(text, e.Font ?? cboTableSelect.Font, brush, e.Bounds, format);
            }
            e.DrawFocusRectangle();
        }

        private async Task SwitchToTableAsync(Table table)
        {
            _currentOrder = await RunDbWriteAsync(() => _orderSvc.GetOrCreateOrderForTableAsync(table.TableID));
            if (_currentOrder != null)
            {
                // Creating an order updates the table status in the existing repository logic.
                // Keep the already-loaded table object in sync for the header and dropdown display.
                table.TableStatus = "Dang phuc vu";
                cboTableSelect.Invalidate();
            }
            UpdateTableHeader();
            RefreshOrderSummary();
        }

        private void UpdateTableHeader()
        {
            if (_table == null)
            {
                lblTableName.Text = "Chọn bàn • Trống • Số khách: 0";
                lblTableName.ForeColor = UITheme.Success;
                lblTableName.BackColor = Color.FromArgb(236, 253, 245);
                return;
            }

            lblTableName.Text = $"{_table.TableName} • {_table.StatusDisplay} • Số khách tối đa: {_table.Capacity}";
            lblTableName.ForeColor = _table.StatusColor;
            lblTableName.BackColor = Color.FromArgb(
                (255 + _table.StatusColor.R) / 2,
                (255 + _table.StatusColor.G) / 2,
                (255 + _table.StatusColor.B) / 2);
            pnlHeader_SizeChanged(pnlHeader, EventArgs.Empty);
        }

        private async Task LoadMenuAsync()
        {
            _categories = await _menuRepo.GetCategoriesAsync();
            _menuItems = await _menuRepo.GetAvailableMenuItemsAsync();

            // Keep the existing tab control as the category-selection state and event source.
            tabCategories.TabPages.Clear();
            var allPage = new TabPage("Tất cả") { Tag = 0 };
            tabCategories.TabPages.Add(allPage);

            foreach (var cat in _categories)
            {
                var page = new TabPage(cat.CategoryName) { Tag = cat.CategoryID };
                tabCategories.TabPages.Add(page);
            }

            tabCategories.SelectedIndex = 0;
            RenderCategoryPills();
            RenderMenuItems(0);
        }

        private void tabCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            var catId = (int)(tabCategories.SelectedTab?.Tag ?? 0);
            UpdateCategoryPillStyles();
            RenderMenuItems(catId);
        }

        private void RenderCategoryPills()
        {
            flowCategories.Controls.Clear();
            var options = new List<(int Id, string Name, int Count)> { (0, "Tất cả", _menuItems.Count) };
            options.AddRange(_categories.Where(c => c.IsActive)
                .Select(c => (c.CategoryID, c.CategoryName, _menuItems.Count(m => m.CategoryID == c.CategoryID))));

            foreach (var option in options)
            {
                var button = new Button
                {
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    Height = 34,
                    MinimumSize = new Size(78, 34),
                    Margin = new Padding(0, 0, 8, 0),
                    Padding = new Padding(12, 0, 12, 0),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                    Text = $"{option.Name}  {option.Count}",
                    Tag = option.Id,
                    Cursor = Cursors.Hand
                };
                button.FlatAppearance.BorderSize = 0;
                button.Click += (_, _) =>
                {
                    int categoryId = (int)button.Tag!;
                    int index = tabCategories.TabPages.Cast<TabPage>().ToList()
                        .FindIndex(page => (int)(page.Tag ?? 0) == categoryId);
                    if (index >= 0 && tabCategories.SelectedIndex != index)
                        tabCategories.SelectedIndex = index;
                    else
                        RenderMenuItems(categoryId);
                };
                button.MouseEnter += (_, _) =>
                {
                    if ((int)button.Tag! != (int)(tabCategories.SelectedTab?.Tag ?? 0))
                        button.BackColor = Color.FromArgb(209, 213, 219);
                };
                button.MouseLeave += (_, _) => UpdateCategoryPillStyles();
                flowCategories.Controls.Add(button);
                button.SizeChanged += (_, _) => ApplyRoundedRegion(button, 17);
                ApplyRoundedRegion(button, 17);
            }
            UpdateCategoryPillStyles();
        }

        private void UpdateCategoryPillStyles()
        {
            int selectedId = (int)(tabCategories.SelectedTab?.Tag ?? 0);
            foreach (Control control in flowCategories.Controls)
            {
                if (control is not Button button) continue;
                bool selected = (int)button.Tag! == selectedId;
                button.BackColor = selected ? UITheme.Primary : UITheme.Border;
                button.ForeColor = selected ? Color.White : Color.FromArgb(55, 65, 81);
            }
        }

        private void RenderMenuItems(int categoryId)
        {
            var items = categoryId == 0
                ? _menuItems
                : _menuItems.Where(m => m.CategoryID == categoryId).ToList();

            var q = NormalizeSearchText(txtMenuSearch.Text);
            if (!string.IsNullOrEmpty(q))
                items = items.Where(m => NormalizeSearchText(m.ItemName).Contains(q)).ToList();

            foreach (Control oldCard in flowMenu.Controls.Cast<Control>().ToList())
            {
                flowMenu.Controls.Remove(oldCard);
                oldCard.Dispose();
            }

            foreach (var item in items)
            {
                var card = CreateMenuCard(item);
                flowMenu.Controls.Add(card);
            }

            bool hasItems = items.Count > 0;
            flowMenu.Visible = hasItems;
            lblMenuEmpty.Visible = !hasItems;
            lblMenuEmpty.Text = string.IsNullOrWhiteSpace(txtMenuSearch.Text)
                ? "Chưa có món nào trong danh mục này"
                : "Không tìm thấy món phù hợp";
            UpdateCartBadges();
        }

        private Panel CreateMenuCard(MenuItemModel item)
        {
            var card = new Panel
            {
                Size = new Size(200, 240),
                BackColor = item.IsAvailable ? UITheme.Surface : UITheme.AlternateRow,
                Cursor = item.IsAvailable ? Cursors.Hand : Cursors.Default,
                Margin = new Padding(6),
                Tag = item
            };
            card.Paint += (s, e) =>
            {
                var quantity = GetCartQuantity(item.MenuItemID);
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using var pen = new Pen(quantity > 0 ? UITheme.Primary : UITheme.Border, quantity > 0 ? 2f : 1f);
                using var path = new System.Drawing.Drawing2D.GraphicsPath();
                const int radius = UITheme.CornerRadius;
                int diameter = radius * 2;
                path.AddArc(0, 0, diameter, diameter, 180, 90);
                path.AddArc(card.Width - diameter - 1, 0, diameter, diameter, 270, 90);
                path.AddArc(card.Width - diameter - 1, card.Height - diameter - 1, diameter, diameter, 0, 90);
                path.AddArc(0, card.Height - diameter - 1, diameter, diameter, 90, 90);
                path.CloseFigure();
                e.Graphics.DrawPath(pen, path);
            };

            var imageBox = new MenuItemPictureBox
            {
                Location = new Point(8, 8),
                Size = new Size(184, 150),
                BackColor = Color.FromArgb(243, 244, 246),
                SizeMode = PictureBoxSizeMode.Zoom,
                PlaceholderText = GetCategoryIcon(item.CategoryName),
                Tag = item
            };
            imageBox.Resize += (_, _) => ApplyRoundedRegion(imageBox, 12);
            ApplyRoundedRegion(imageBox, 12);
            card.Controls.Add(imageBox);
            LoadMenuImageAsync(imageBox, item);

            var lblName = new Label
            {
                Text = item.ItemName,
                Font = new Font(UITheme.FontFamily, 10f, FontStyle.Bold),
                ForeColor = item.IsAvailable ? UITheme.TextPrimary : UITheme.TextMuted,
                AutoSize = false,
                Size = new Size(180, 38),
                Location = new Point(10, 163),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true,
                Tag = item
            };
            card.Controls.Add(lblName);

            var lblPrice = new Label
            {
                Text = item.Price.ToString("#,##0", CultureInfo.GetCultureInfo("en-US")) + " đ",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = item.IsAvailable ? Color.FromArgb(220, 96, 35) : Color.FromArgb(160, 163, 168),
                AutoSize = false,
                Size = new Size(132, 26),
                Location = new Point(10, 205),
                Tag = item
            };
            card.Controls.Add(lblPrice);

            var lblOutOfStock = new Label
            {
                Text = "Hết món",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(12, 12),
                Size = new Size(70, 24),
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = UITheme.TextMuted,
                Visible = !item.IsAvailable,
                Tag = item
            };
            card.Controls.Add(lblOutOfStock);

            var lblQuantity = new Label
            {
                Name = "lblQuantityBadge",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(158, 12),
                Size = new Size(30, 30),
                Font = new Font(UITheme.FontFamily, 9f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = UITheme.Primary,
                Visible = false,
                Tag = item
            };
            lblQuantity.Region = CreateCircularRegion(lblQuantity.Size);
            card.Controls.Add(lblQuantity);

            var btnAdd = new Button
            {
                Text = "+",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                BackColor = item.IsAvailable ? UITheme.Primary : Color.FromArgb(156, 163, 175),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(36, 36),
                Location = new Point(154, 196),
                Cursor = item.IsAvailable ? Cursors.Hand : Cursors.Default,
                Tag = item,
                Enabled = item.IsAvailable
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Region = CreateCircularRegion(btnAdd.Size);
            btnAdd.Click += BtnAddItem_Click;
            btnAdd.MouseEnter += (_, _) => { if (btnAdd.Enabled) btnAdd.BackColor = Color.FromArgb(29, 78, 216); };
            btnAdd.MouseLeave += (_, _) => btnAdd.BackColor = item.IsAvailable ? UITheme.Primary : Color.FromArgb(156, 163, 175);
            card.Controls.Add(btnAdd);

            card.Click += (s, e) => BtnAddItem_Click(btnAdd, e);
            foreach (Control c in card.Controls)
                if (c != btnAdd) c.Click += (s, e) => BtnAddItem_Click(btnAdd, e);
            card.MouseEnter += (_, _) => { if (item.IsAvailable) { card.BackColor = UITheme.PrimarySoft; card.Invalidate(); } };
            card.MouseLeave += (_, _) => { card.BackColor = item.IsAvailable ? UITheme.Surface : UITheme.AlternateRow; card.Invalidate(); };

            return card;
        }

        private async void BtnAddItem_Click(object? sender, EventArgs e)
        {
            if (_table == null)
            {
                ValidationHelper.ShowError("Vui lòng chọn bàn trước khi gọi món!");
                return;
            }
            if (_currentOrder == null)
            {
                _currentOrder = await _orderSvc.GetOrCreateOrderForTableAsync(_table.TableID);
            }

            if (sender is Button btn && btn.Tag is MenuItemModel item && _currentOrder != null)
            {
                if (!item.IsAvailable) return;
                string? note = string.IsNullOrWhiteSpace(txtItemNote.Text) ? null : txtItemNote.Text.Trim();
                int orderId = _currentOrder.OrderID;
                await RunDbWriteAsync(() => _orderSvc.AddItemAsync(orderId, item, 1, note));
                _currentOrder = await _orderSvc.GetOrderAsync(_currentOrder.OrderID);
                RefreshOrderSummary();
                txtItemNote.Clear();
            }
        }

        private void RefreshOrderSummary()
        {
            int? selectedDetailId = _selectedCartDetail?.OrderDetailID;
            dgvOrder.Rows.Clear();
            if (_currentOrder == null)
            {
                lblTotal.Text = "TỔNG TIỀN: 0 đ";
                lblSubtotalValue.Text = "0 đ";
                lblDiscountValue.Text = "− 0 đ";
                lblItemCount.Text = "0 món";
                lblOrderTitle.Text = _table == null ? "Đơn hàng" : $"Đơn của {_table.TableName}";
                RenderCartRows(new List<OrderDetail>());
                btnSendKitchen.Enabled = false;
                btnCheckout.Enabled = false;
                btnCancelOrder.Enabled = false;
                UpdateCartBadges();
                return;
            }

            var activeDetails = _currentOrder.Details
                .Where(d => d.ItemStatus != "Da huy")
                .ToList();

            foreach (var d in activeDetails)
            {
                var idx = dgvOrder.Rows.Add(
                    d.ItemName + (string.IsNullOrEmpty(d.Note) ? "" : $" ({d.Note})"),
                    d.Quantity,
                    d.UnitPrice.ToString("N0"),
                    d.SubTotal.ToString("N0"),
                    d.StatusDisplay,
                    d.OrderDetailID
                );
                dgvOrder.Rows[idx].Tag = d;
                dgvOrder.Rows[idx].DefaultCellStyle.BackColor = d.ItemStatus == "Moi"
                    ? Color.FromArgb(240, 248, 255) : Color.White;
            }

            decimal subtotal = activeDetails.Sum(d => d.SubTotal);
            decimal discount = cboCartDiscountMode.SelectedIndex == 1
                ? Math.Min(subtotal, numCartDiscount.Value)
                : Math.Round(subtotal * numCartDiscount.Value / 100, 0);
            decimal total = subtotal - discount;
            lblSubtotalValue.Text = $"{subtotal:N0} đ";
            lblDiscountValue.Text = $"− {discount:N0} đ";
            lblTotal.Text = $"TỔNG TIỀN: {total:N0} đ";
            int itemCount = activeDetails.Sum(d => d.Quantity);
            lblItemCount.Text = itemCount > 99 ? "99+ món" : $"{itemCount} món";
            lblOrderTitle.Text = _table == null ? "Đơn hàng" : $"Đơn của {_table.TableName}";
            RenderCartRows(activeDetails);
            if (selectedDetailId.HasValue)
            {
                _selectedCartDetail = activeDetails.FirstOrDefault(d => d.OrderDetailID == selectedDetailId.Value);
                foreach (Control row in flowCart.Controls)
                    if (row is Panel cartRow) cartRow.Invalidate();
            }
            btnSendKitchen.Enabled = activeDetails.Any(d => d.ItemStatus == "Moi");
            btnCheckout.Enabled = activeDetails.Count > 0;
            btnCancelOrder.Enabled = true;
            UpdateCartBadges();
        }

        private void RenderCartRows(IReadOnlyList<OrderDetail> details)
        {
            _selectedCartDetail = null;
            foreach (Control row in flowCart.Controls.Cast<Control>().ToList())
            {
                flowCart.Controls.Remove(row);
                row.Dispose();
            }

            var empty = flowCart.Tag as Control;
            bool hasDetails = details.Count > 0;
            flowCart.Visible = hasDetails;
            if (empty != null) empty.Visible = !hasDetails;
            foreach (var detail in details)
                flowCart.Controls.Add(CreateCartRow(detail));
            ResizeCartRows();
            btnClearCart.Enabled = details.Any(d => d.ItemStatus is "Moi" or "Cho bep");
            btnClearCart.ForeColor = btnClearCart.Enabled ? UITheme.Danger : Color.FromArgb(156, 163, 175);
        }

        private Panel CreateCartRow(OrderDetail detail)
        {
            bool hasNote = !string.IsNullOrWhiteSpace(detail.Note);
            int rowHeight = hasNote ? 92 : 72;
            var row = new Panel
            {
                Height = rowHeight,
                Width = Math.Max(280, flowCart.ClientSize.Width - 30),
                Margin = new Padding(2, 3, 2, 5),
                Padding = new Padding(8, 4, 8, 4),
                BackColor = Color.White,
                Tag = detail,
                Cursor = Cursors.Hand
            };
            row.Paint += (_, e) =>
            {
                bool selected = _selectedCartDetail?.OrderDetailID == detail.OrderDetailID;
                using var pen = new Pen(selected ? UITheme.Primary : UITheme.Border, selected ? 2 : 1);
                e.Graphics.DrawRectangle(pen, 0, 0, row.Width - 1, row.Height - 1);
            };

            var name = new Label
            {
                Text = detail.ItemName,
                Location = new Point(9, 5),
                Size = new Size(Math.Max(120, row.Width - 165), 22),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = UITheme.TextPrimary,
                AutoEllipsis = true,
                Tag = detail
            };
            var subtotal = new Label
            {
                Text = $"{detail.SubTotal:N0} đ",
                Location = new Point(row.Width - 132, 5),
                Size = new Size(120, 22),
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = UITheme.TextPrimary,
                Tag = detail
            };
            var unitPrice = new Label
            {
                Text = $"{detail.UnitPrice:N0} đ / món",
                Location = new Point(9, 31),
                Size = new Size(112, 22),
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = UITheme.TextMuted,
                Tag = detail
            };
            var minus = CreateCartActionButton("−", 8, 28, Color.FromArgb(243, 244, 246));
            var quantity = new Label
            {
                Text = detail.Quantity.ToString(),
                Location = new Point(0, 0),
                Size = new Size(28, 26),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(UITheme.FontFamily, 9f, FontStyle.Bold),
                ForeColor = UITheme.TextPrimary,
                Tag = detail
            };
            var plus = CreateCartActionButton("+", 28, 28, Color.FromArgb(243, 244, 246));
            bool preCooking = detail.ItemStatus is "Moi" or "Cho bep";
            bool cancellable = detail.ItemStatus != "Da huy";
            minus.Enabled = preCooking || cancellable;
            plus.Enabled = preCooking;
            minus.Location = new Point(Math.Max(125, row.Width - 178), 29);
            quantity.Location = new Point(minus.Right, 29);
            plus.Location = new Point(quantity.Right, 29);
            minus.Click += (_, e) => RunCartAction(detail, (_, args) => btnDecrease_Click(this, args));
            plus.Click += (_, e) => RunCartAction(detail, (_, args) => btnIncrease_Click(this, args));

            var editNote = CreateCartActionButton("✎", 28, 28, UITheme.PrimarySoft);
            editNote.ForeColor = UITheme.Primary;
            editNote.Location = new Point(plus.Right + 5, 29);
            editNote.Click += async (_, _) => await EditCartNoteAsync(detail);
            var remove = CreateCartActionButton("🗑", 28, 28, Color.FromArgb(254, 242, 242));
            remove.ForeColor = UITheme.Danger;
            remove.Location = new Point(editNote.Right + 5, 29);
            remove.Enabled = cancellable;
            remove.Click += (_, e) => RunCartAction(detail, (_, args) => btnRemoveItem_Click(this, args));

            var status = CreateStatusChip(detail.ItemStatus, detail);
            status.Location = new Point(9, 50);
            ApplyRoundedRegion(status, 9);

            row.Controls.AddRange(new Control[] { name, subtotal, unitPrice, minus, quantity, plus, editNote, remove, status });
            if (hasNote)
            {
                var note = new Label
                {
                    Text = detail.Note,
                    Location = new Point(9, 69),
                    Size = new Size(Math.Max(160, row.Width - 20), 18),
                    Font = new Font("Segoe UI", 8.5f, FontStyle.Italic),
                    ForeColor = Color.FromArgb(217, 119, 6),
                    AutoEllipsis = true,
                    Tag = detail
                };
                row.Controls.Add(note);
            }

            row.Click += async (_, _) =>
            {
                SelectCartDetail(detail);
                await EditCartNoteAsync(detail);
            };
            foreach (Control control in row.Controls)
            {
                if (control is Button) continue;
                control.Click += async (_, _) =>
                {
                    SelectCartDetail(detail);
                    await EditCartNoteAsync(detail);
                };
            }
            return row;
        }

        private void SelectCartDetail(OrderDetail detail)
        {
            _selectedCartDetail = detail;
            foreach (Control control in flowCart.Controls)
                if (control is Panel row) row.Invalidate();
        }

        private static Button CreateCartActionButton(string text, int width, int height, Color background)
        {
            var button = new Button
            {
                Text = text,
                Size = new Size(width, height),
                FlatStyle = FlatStyle.Flat,
                BackColor = background,
                ForeColor = Color.FromArgb(55, 65, 81),
                Font = new Font(UITheme.FontFamily, 9f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = Padding.Empty
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private static Label CreateStatusChip(string itemStatus, OrderDetail detail)
        {
            var (text, background, foreground) = itemStatus switch
            {
                "Cho bep" => ("Chờ bếp", Color.FromArgb(254, 243, 199), Color.FromArgb(180, 83, 9)),
                "Dang che bien" => ("Đang nấu", Color.FromArgb(219, 234, 254), Color.FromArgb(29, 78, 216)),
                "San sang" => ("Đã xong", Color.FromArgb(220, 252, 231), Color.FromArgb(21, 128, 61)),
                "Da phuc vu" => ("Đã phục vụ", Color.FromArgb(243, 244, 246), Color.FromArgb(75, 85, 99)),
                _ => ("Mới", Color.FromArgb(243, 244, 246), Color.FromArgb(75, 85, 99))
            };
            return new Label
            {
                Text = text,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(text.Length > 8 ? 78 : 62, 19),
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                BackColor = background,
                ForeColor = foreground,
                Tag = detail
            };
        }

        private void ResizeCartRows()
        {
            if (flowCart == null || flowCart.IsDisposed) return;
            int width = Math.Max(280, flowCart.ClientSize.Width - 30);
            foreach (Control control in flowCart.Controls)
            {
                if (control is not Panel row) continue;
                row.Width = width;
                if (row.Tag is not OrderDetail detail) continue;
                var name = row.Controls.OfType<Label>().FirstOrDefault(label => label.Text == detail.ItemName);
                var total = row.Controls.OfType<Label>().FirstOrDefault(label => label.Text == $"{detail.SubTotal:N0} đ");
                if (name != null) name.Width = Math.Max(120, row.Width - 165);
                if (total != null) total.Location = new Point(row.Width - 132, 5);
                var buttons = row.Controls.OfType<Button>().ToList();
                if (buttons.Count < 4) continue;
                var minus = buttons[0];
                minus.Location = new Point(Math.Max(125, row.Width - 178), 29);
                var qty = row.Controls.OfType<Label>().FirstOrDefault(label => label.Tag == detail && int.TryParse(label.Text, out _));
                if (qty != null) qty.Location = new Point(minus.Right, 29);
                var plus = buttons[1];
                plus.Location = new Point(minus.Right + 28, 29);
                buttons[2].Location = new Point(plus.Right + 5, 29);
                buttons[3].Location = new Point(buttons[2].Right + 5, 29);
            }
        }

        private void RunCartAction(OrderDetail detail, EventHandler handler)
        {
            _selectedCartDetail = detail;
            handler(this, EventArgs.Empty);
        }

        private async Task EditCartNoteAsync(OrderDetail detail)
        {
            string[] existingParts = (detail.Note ?? string.Empty)
                .Split(" • ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var selectedOptions = existingParts
                .Where(part => QuickNoteOptions.Contains(part, StringComparer.OrdinalIgnoreCase))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            string customNote = string.Join(" • ", existingParts
                .Where(part => !QuickNoteOptions.Contains(part, StringComparer.OrdinalIgnoreCase)));

            using var dialog = new Form
            {
                Text = $"Ghi chú món: {detail.ItemName}",
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MinimizeBox = false,
                MaximizeBox = false,
                ShowInTaskbar = false,
                ClientSize = new Size(410, 306),
                Font = new Font("Segoe UI", 9.5f),
                BackColor = Color.White
            };
            var label = new Label
            {
                Text = "Ghi chú riêng cho món này",
                Location = new Point(20, 15),
                Size = new Size(360, 24),
                Font = new Font(UITheme.FontFamily, 10f, FontStyle.Bold),
                ForeColor = UITheme.TextPrimary
            };
            var quickLabel = new Label
            {
                Text = "Ghi chú nhanh (có thể chọn nhiều)",
                Location = new Point(20, 42),
                Size = new Size(360, 18),
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = UITheme.TextMuted
            };
            var chipPanel = new FlowLayoutPanel
            {
                Location = new Point(20, 63),
                Size = new Size(370, 76),
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.White,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            var chipButtons = new Dictionary<string, Button>();
            foreach (string option in QuickNoteOptions)
            {
                bool isSelected = selectedOptions.Contains(option);
                var chip = new Button
                {
                    Text = option,
                    Size = new Size(112, 32),
                    Margin = new Padding(0, 0, 6, 6),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                    Cursor = Cursors.Hand,
                    Tag = isSelected
                };
                chip.FlatAppearance.BorderSize = 1;
                chip.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
                ApplyQuickNoteChipStyle(chip, isSelected);
                chip.Click += (_, _) =>
                {
                    bool selected = !(chip.Tag as bool? ?? false);
                    chip.Tag = selected;
                    ApplyQuickNoteChipStyle(chip, selected);
                };
                chipButtons[option] = chip;
                chipPanel.Controls.Add(chip);
            }
            var customLabel = new Label
            {
                Text = "Ghi chú thêm",
                Location = new Point(20, 143),
                Size = new Size(360, 18),
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = UITheme.TextMuted
            };
            var noteInput = new TextBox
            {
                Text = customNote,
                Location = new Point(20, 164),
                Size = new Size(370, 82),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                MaxLength = 255,
                Font = UITheme.FontNormal,
                PlaceholderText = "Ví dụ: ít cay, không hành"
            };
            var cancel = new Button
            {
                Text = "Hủy",
                DialogResult = DialogResult.Cancel,
                Location = new Point(220, 258),
                Size = new Size(78, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(243, 244, 246),
                ForeColor = Color.FromArgb(55, 65, 81)
            };
            var save = new Button
            {
                Text = "Lưu",
                Location = new Point(310, 258),
                Size = new Size(80, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = UITheme.Primary,
                ForeColor = Color.White
            };
            cancel.FlatAppearance.BorderSize = 0;
            save.FlatAppearance.BorderSize = 0;
            save.Click += (_, _) =>
            {
                var parts = chipButtons
                    .Where(pair => pair.Value.Tag as bool? == true)
                    .Select(pair => pair.Key)
                    .ToList();
                if (!string.IsNullOrWhiteSpace(noteInput.Text))
                    parts.Add(noteInput.Text.Trim());
                string combinedNote = string.Join(" • ", parts);
                if (combinedNote.Length > 255)
                {
                    ValidationHelper.ShowError("Ghi chú tối đa 255 ký tự.");
                    return;
                }
                dialog.Tag = combinedNote;
                dialog.DialogResult = DialogResult.OK;
                dialog.Close();
            };
            dialog.Controls.AddRange(new Control[] { label, quickLabel, chipPanel, customLabel, noteInput, cancel, save });
            dialog.CancelButton = cancel;
            dialog.AcceptButton = save;

            _isModalDialogOpen = true;
            try
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
            }
            finally
            {
                _isModalDialogOpen = false;
            }
            await RunDbWriteAsync(() => _orderSvc.UpdateDetailNoteAsync(detail.OrderDetailID, dialog.Tag as string));
            if (_currentOrder != null)
                _currentOrder = await _orderSvc.GetOrderAsync(_currentOrder.OrderID);
            RefreshOrderSummary();
        }

        private static void ApplyQuickNoteChipStyle(Button chip, bool selected)
        {
            chip.BackColor = selected ? Color.FromArgb(219, 234, 254) : UITheme.AlternateRow;
            chip.ForeColor = selected ? Color.FromArgb(29, 78, 216) : Color.FromArgb(55, 65, 81);
            chip.FlatAppearance.BorderColor = selected ? Color.FromArgb(96, 165, 250) : Color.FromArgb(209, 213, 219);
            ApplyRoundedRegion(chip, 14);
        }

        private async void btnClearCart_Click(object? sender, EventArgs e)
        {
            if (_currentOrder == null) return;
            var active = _currentOrder.Details.Where(d => d.ItemStatus != "Da huy").ToList();
            var removable = active.Where(d => d.ItemStatus is "Moi" or "Cho bep").ToList();
            if (removable.Count == 0)
            {
                ValidationHelper.ShowError("Không có món ở trạng thái Mới hoặc Chờ bếp để xóa.");
                return;
            }

            string warning = removable.Count == active.Count
                ? $"Xóa tất cả {removable.Count} món ở trạng thái Mới/Chờ bếp khỏi đơn?"
                : $"Xóa {removable.Count} món ở trạng thái Mới/Chờ bếp? {active.Count - removable.Count} món đã chế biến sẽ được giữ lại.";
            if (!ValidationHelper.Confirm(warning)) return;

            await RunDbWriteAsync(async () =>
            {
                foreach (var detail in removable)
                    await _orderSvc.RemoveItemAsync(detail.OrderDetailID);
            });
            _currentOrder = await _orderSvc.GetOrderAsync(_currentOrder.OrderID);
            RefreshOrderSummary();
        }

        private OrderDetail? GetSelectedCartDetail() => _selectedCartDetail ??
            (dgvOrder.SelectedRows.Count > 0 ? dgvOrder.SelectedRows[0].Tag as OrderDetail : null);

        private int GetCartQuantity(int menuItemId) => _currentOrder?.Details
            .Where(d => d.MenuItemID == menuItemId && d.ItemStatus != "Da huy")
            .Sum(d => d.Quantity) ?? 0;

        private void UpdateCartBadges()
        {
            if (flowMenu.IsDisposed) return;
            foreach (Control control in flowMenu.Controls)
            {
                if (control is not Panel card || card.Tag is not MenuItemModel item) continue;
                var badge = card.Controls.Find("lblQuantityBadge", false).FirstOrDefault();
                if (badge == null) continue;
                int quantity = GetCartQuantity(item.MenuItemID);
                badge.Text = quantity > 99 ? "99+" : quantity.ToString();
                badge.Visible = quantity > 0;
                card.Invalidate();
            }
        }

        private static string NormalizeSearchText(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            string normalized = value.Trim().ToLowerInvariant().Replace('đ', 'd').Replace('Đ', 'd')
                .Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder(normalized.Length);
            foreach (char character in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
                    builder.Append(character);
            }
            return builder.ToString().Normalize(NormalizationForm.FormC);
        }

        private static string GetCategoryIcon(string? categoryName)
        {
            string category = NormalizeSearchText(categoryName);
            if (category.Contains("uong") || category.Contains("nuoc")) return "🥤";
            if (category.Contains("hai san") || category.Contains("ca") || category.Contains("tom")) return "🐟";
            if (category.Contains("com") || category.Contains("gao")) return "🍚";
            if (category.Contains("lau")) return "🍲";
            if (category.Contains("trang mieng") || category.Contains("banh")) return "🍰";
            return "🍽️";
        }

        private static void ApplyRoundedRegion(Control control, int radius)
        {
            if (control.Width <= 0 || control.Height <= 0) return;
            using var path = new System.Drawing.Drawing2D.GraphicsPath();
            int diameter = Math.Min(radius * 2, Math.Min(control.Width, control.Height));
            var bounds = new Rectangle(0, 0, control.Width, control.Height);
            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            var previous = control.Region;
            control.Region = new Region(path);
            previous?.Dispose();
        }

        private static Region CreateCircularRegion(Size size)
        {
            using var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, size.Width, size.Height);
            return new Region(path);
        }

        private static string? ResolveMenuImagePath(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath)) return null;
            string candidate = Path.IsPathRooted(imagePath)
                ? imagePath
                : Path.Combine(AppContext.BaseDirectory, imagePath);
            return File.Exists(candidate) ? candidate : null;
        }

        private async void LoadMenuImageAsync(MenuItemPictureBox picture, MenuItemModel item)
        {
            string? path = ResolveMenuImagePath(item.ImagePath);
            if (path == null || picture.IsDisposed) return;

            if (!_menuImageCache.TryGetValue(path, out var image))
            {
                if (!_pendingImageLoads.TryGetValue(path, out var pending))
                {
                    pending = LoadImageFromDiskAsync(path);
                    _pendingImageLoads[path] = pending;
                }

                image = await pending;
                _pendingImageLoads.Remove(path);
                if (image == null) return;
                _menuImageCache[path] = image;
            }

            if (!picture.IsDisposed)
            {
                picture.Image = image;
                picture.Invalidate();
            }
        }

        private async Task<Image?> LoadImageFromDiskAsync(string path)
        {
            await _imageLoadGate.WaitAsync();
            try
            {
                return await Task.Run<Image?>(() =>
                {
                    try
                    {
                        using var source = Image.FromFile(path);
                        double scale = Math.Min(1d, Math.Min(420d / source.Width, 340d / source.Height));
                        int width = Math.Max(1, (int)Math.Round(source.Width * scale));
                        int height = Math.Max(1, (int)Math.Round(source.Height * scale));
                        var thumbnail = new Bitmap(width, height);
                        using (var graphics = Graphics.FromImage(thumbnail))
                        {
                            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                            graphics.DrawImage(source, new Rectangle(0, 0, width, height));
                        }
                        return thumbnail;
                    }
                    catch
                    {
                        return null;
                    }
                });
            }
            finally
            {
                _imageLoadGate.Release();
            }
        }

        private void SearchDebounceTimer_Tick(object? sender, EventArgs e)
        {
            _searchDebounceTimer.Stop();
            RenderMenuItems((int)(tabCategories.SelectedTab?.Tag ?? 0));
        }

        private void FormOrder_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                txtMenuSearch.Focus();
                txtMenuSearch.SelectAll();
                ConsumeShortcut(e);
            }
            else if (e.KeyCode == Keys.F5 && btnSendKitchen.Enabled)
            {
                btnSendKitchen_Click(btnSendKitchen, EventArgs.Empty);
                ConsumeShortcut(e);
            }
            else if (e.KeyCode == Keys.F9 && btnCheckout.Enabled)
            {
                btnCheckout_Click(btnCheckout, EventArgs.Empty);
                ConsumeShortcut(e);
            }
            else if (e.KeyCode == Keys.Escape && txtMenuSearch.TextLength > 0)
            {
                txtMenuSearch.Clear();
                ConsumeShortcut(e);
            }
            else if (!IsTextEntryFocused() && e.KeyCode == Keys.Delete && GetSelectedCartDetail() != null)
            {
                btnRemoveItem_Click(this, EventArgs.Empty);
                ConsumeShortcut(e);
            }
            else if (!IsTextEntryFocused() && IsIncreaseShortcut(e) && GetSelectedCartDetail() != null)
            {
                btnIncrease_Click(this, EventArgs.Empty);
                ConsumeShortcut(e);
            }
            else if (!IsTextEntryFocused() && IsDecreaseShortcut(e) && GetSelectedCartDetail() != null)
            {
                btnDecrease_Click(this, EventArgs.Empty);
                ConsumeShortcut(e);
            }
        }

        private static void ConsumeShortcut(KeyEventArgs e)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private static bool IsIncreaseShortcut(KeyEventArgs e) =>
            e.KeyCode is Keys.Add or Keys.Oemplus;

        private static bool IsDecreaseShortcut(KeyEventArgs e) =>
            e.KeyCode is Keys.Subtract or Keys.OemMinus;

        private bool IsTextEntryFocused()
        {
            Control focused = this;
            while (true)
            {
                var child = focused.Controls.Cast<Control>().FirstOrDefault(control => control.ContainsFocus);
                if (child == null) break;
                focused = child;
            }
            return focused is TextBoxBase or NumericUpDown or MaskedTextBox ||
                   focused is ComboBox combo && combo.DropDownStyle != ComboBoxStyle.DropDownList;
        }

        private void FormOrder_FormClosed(object? sender, FormClosedEventArgs e)
        {
            _searchDebounceTimer.Stop();
            _orderRefreshTimer.Stop();
            _orderRefreshTimer.Dispose();
            foreach (var image in _menuImageCache.Values) image.Dispose();
            _menuImageCache.Clear();
        }

        private sealed class MenuItemPictureBox : PictureBox
        {
            private static readonly Font PlaceholderFont = new Font("Segoe UI Emoji", 38f, FontStyle.Regular);
            [System.ComponentModel.Browsable(false)]
            [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
            public string PlaceholderText { get; set; } = "🍽️";

            protected override void OnPaint(PaintEventArgs pe)
            {
                base.OnPaint(pe);
                if (Image != null) return;
                TextRenderer.DrawText(pe.Graphics, PlaceholderText, PlaceholderFont, ClientRectangle,
                    Color.FromArgb(156, 163, 175),
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            }
        }

        private async void btnRemoveItem_Click(object sender, EventArgs e)
        {
            if (GetSelectedCartDetail() is OrderDetail d)
            {
                if (!ValidationHelper.Confirm($"Hủy món '{d.ItemName}'?")) return;
                bool? managerOverride = await RequestItemEditPermissionAsync(d);
                if (!managerOverride.HasValue) return;
                await RunDbWriteAsync(() => _orderSvc.RemoveItemAsync(d.OrderDetailID, managerOverride.Value));
                _currentOrder = await _orderSvc.GetOrderAsync(_currentOrder!.OrderID);
                RefreshOrderSummary();
            }
        }

        private async void btnIncrease_Click(object sender, EventArgs e)
        {
            if (GetSelectedCartDetail() is OrderDetail d && d.ItemStatus is "Moi" or "Cho bep")
            {
                await RunDbWriteAsync(() => _orderSvc.UpdateQuantityAsync(d.OrderDetailID, d.Quantity + 1));
                _currentOrder = await _orderSvc.GetOrderAsync(_currentOrder!.OrderID);
                RefreshOrderSummary();
            }
        }

        private async void btnDecrease_Click(object sender, EventArgs e)
        {
            if (GetSelectedCartDetail() is OrderDetail d && d.ItemStatus is "Moi" or "Cho bep" or "Dang che bien" or "San sang" or "Da phuc vu")
            {
                bool? managerOverride = await RequestItemEditPermissionAsync(d);
                if (!managerOverride.HasValue) return;
                if (d.Quantity <= 1)
                {
                    if (ValidationHelper.Confirm($"Hủy món '{d.ItemName}' khỏi đơn?"))
                        await RunDbWriteAsync(() => _orderSvc.RemoveItemAsync(d.OrderDetailID, managerOverride.Value));
                    else
                        return;
                }
                else
                {
                    await RunDbWriteAsync(() => _orderSvc.UpdateQuantityAsync(d.OrderDetailID, d.Quantity - 1, managerOverride.Value));
                }
                _currentOrder = await _orderSvc.GetOrderAsync(_currentOrder!.OrderID);
                RefreshOrderSummary();
            }
        }

        private async Task<bool?> RequestItemEditPermissionAsync(OrderDetail detail)
        {
            if (detail.ItemStatus is "Moi" or "Cho bep") return false;
            if (detail.ItemStatus is not ("Dang che bien" or "San sang" or "Da phuc vu"))
            {
                ValidationHelper.ShowError("Không thể thay đổi món đã hủy.");
                return null;
            }

            if (!await ShowManagerPasswordDialogAsync($"Điều chỉnh món \"{detail.ItemName}\" đã được bếp nhận"))
            {
                ValidationHelper.ShowError("Cần xác thực quản lý để hủy hoặc giảm số lượng món đã được bếp nhận.");
                return null;
            }
            return true;
        }

        private async void btnCheckout_Click(object sender, EventArgs e)
        {
            if (_currentOrder != null)
                _currentOrder = await _orderSvc.GetOrderAsync(_currentOrder.OrderID);
            if (_currentOrder == null || !_currentOrder.Details.Any(d => d.ItemStatus != "Da huy"))
            {
                ValidationHelper.ShowError("Không có món nào trong đơn hàng để thanh toán!");
                return;
            }

            if (_currentOrder.Details.Any(d => d.ItemStatus == "Moi"))
            {
                _isModalDialogOpen = true;
                bool sendFirst;
                try
                {
                    sendFirst = MessageBox.Show("Đơn còn món chưa gửi bếp. Gửi bếp ngay rồi thanh toán?", "Chưa gửi hết món",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
                }
                finally { _isModalDialogOpen = false; }
                if (!sendFirst || !await SendNewItemsAndRefreshAsync()) return;
            }

            using var dlg = new FormPayment(_currentOrder, numCartDiscount.Value, cboCartDiscountMode.SelectedIndex == 1);
            _isModalDialogOpen = true;
            DialogResult paymentResult;
            try { paymentResult = dlg.ShowDialog(this); }
            finally { _isModalDialogOpen = false; }
            if (paymentResult == DialogResult.OK)
            {
                if (dlg.PaymentId <= 0) return;
                if (_table != null)
                {
                    _table.TableStatus = "Trong";
                    cboTableSelect.Invalidate();
                    UpdateTableHeader();
                }

                _currentOrder = null;
                dgvOrder.Rows.Clear();
                lblTotal.Text = "TỔNG TIỀN: 0 đ";
                lblItemCount.Text = "Số món: 0";

                // Return to table view
                btnBack_Click(sender, e);
            }
        }

        private async void btnCancelOrder_Click(object sender, EventArgs e)
        {
            if (_currentOrder == null || _table == null) return;
            string? reason;
            bool confirmed;
            _isModalDialogOpen = true;
            try
            {
                confirmed = ValidationHelper.Confirm($"Hủy toàn bộ đơn hàng của {_table.TableName}?");
                if (!confirmed) return;
                reason = ShowCancelReasonDialog();
            }
            finally { _isModalDialogOpen = false; }
            if (reason == null) return;

            if (_currentOrder.Details.Any(d => d.ItemStatus == "Dang che bien"))
            {
                int sentItemCount = _currentOrder.Details.Count(d => d.ItemStatus != "Moi" && d.ItemStatus != "Da huy");
                if (!await ShowManagerPasswordDialogAsync($"Hủy đơn {_table.TableName} (đã có {sentItemCount} món gửi bếp)"))
                {
                    ValidationHelper.ShowError("Cần mật khẩu quản lý hợp lệ để hủy đơn có món đang chế biến.");
                    return;
                }
            }

            bool canceled = await RunDbWriteAsync(() => _orderSvc.CancelOrderAsync(_currentOrder.OrderID, _table.TableID, reason));
            if (!canceled) return;
            ValidationHelper.ShowSuccess("Đã hủy đơn hàng.");
            _currentOrder = null;
            dgvOrder.Rows.Clear();
            lblTotal.Text = "TỔNG TIỀN: 0 đ";
            lblItemCount.Text = "Số món: 0";

            btnBack_Click(sender, e);
        }

        private async void btnSendKitchen_Click(object? sender, EventArgs e)
        {
            await SendNewItemsAndRefreshAsync();
        }

        private async Task<bool> SendNewItemsAndRefreshAsync()
        {
            if (_currentOrder == null) return false;
            int orderId = _currentOrder.OrderID;
            if (!_currentOrder.Details.Any(d => d.ItemStatus == "Moi")) return true;
            bool sent = await RunDbWriteAsync(async () =>
            {
                await _orderSvc.SendNewItemsToKitchenAsync(orderId);
                if (_table != null)
                    await _tableSvc.UpdateStatusAsync(_table.TableID, "Dang phuc vu");
            });
            if (!sent) return false;
            _currentOrder = await _orderSvc.GetOrderAsync(orderId);
            if (_currentOrder == null) return false;
            if (_table != null && _currentOrder.Details.Any(d => d.ItemStatus == "Cho bep"))
            {
                _table.TableStatus = "Dang phuc vu";
                UpdateTableHeader();
                cboTableSelect.Invalidate();
            }
            RefreshOrderSummary();
            return !_currentOrder.Details.Any(d => d.ItemStatus == "Moi");
        }

        private string? ShowCancelReasonDialog()
        {
            using var form = new Form
            {
                Text = "Lý do hủy đơn", Width = 410, Height = 205,
                FormBorderStyle = FormBorderStyle.FixedDialog, StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false, MinimizeBox = false, BackColor = Color.White
            };
            var label = new Label { Text = "Nhập lý do hủy đơn (bắt buộc):", AutoSize = true,
                Location = new Point(18, 16), Font = UITheme.FontNormal, ForeColor = UITheme.TextPrimary };
            var input = new TextBox { Location = new Point(18, 44), Size = new Size(355, 52), Multiline = true,
                Font = UITheme.FontNormal, MaxLength = 250 };
            var error = new Label { Text = "", AutoSize = true, Location = new Point(18, 102),
                ForeColor = UITheme.Danger, Font = UITheme.FontSmall };
            var ok = UITheme.CreateButton("Xác nhận hủy", UITheme.Danger);
            ok.Size = new Size(125, 34); ok.Location = new Point(122, 128);
            ok.Click += (_, _) =>
            {
                if (string.IsNullOrWhiteSpace(input.Text)) { error.Text = "Vui lòng nhập lý do."; return; }
                form.DialogResult = DialogResult.OK;
            };
            var cancel = UITheme.CreateButton("Quay lại", UITheme.TextMuted);
            cancel.Size = new Size(100, 34); cancel.Location = new Point(258, 128);
            cancel.Click += (_, _) => form.DialogResult = DialogResult.Cancel;
            form.Controls.AddRange(new Control[] { label, input, error, ok, cancel });
            form.AcceptButton = ok; form.CancelButton = cancel;
            return form.ShowDialog(this) == DialogResult.OK ? input.Text.Trim() : null;
        }

        private Task<bool> ShowManagerPasswordDialogAsync(string? moTaThaoTac = null)
        {
            using var form = new FormXacNhanQuanLy
            {
                MoTaThaoTac = moTaThaoTac ?? "Thao tác này cần Quản lý xác nhận."
            };
            _isModalDialogOpen = true;
            DialogResult result;
            try { result = form.ShowDialog(this); }
            finally { _isModalDialogOpen = false; }
            return Task.FromResult(result == DialogResult.OK && form.MaQuanLyDuyet.HasValue);
        }

        private void txtMenuSearch_TextChanged(object sender, EventArgs e)
        {
            _searchDebounceTimer.Stop();
            _searchDebounceTimer.Start();
        }

        private void btnBack_Click(object? sender, EventArgs e)
        {
            if (Parent?.FindForm() is FormMain main)
            {
                main.NavigateTo(new FormTable());
                return;
            }

            if (this.Parent is Panel pnl)
            {
                var tblForm = new FormTable();
                tblForm.TopLevel = false;
                tblForm.FormBorderStyle = FormBorderStyle.None;
                tblForm.Dock = DockStyle.Fill;
                pnl.Controls.Clear();
                pnl.Controls.Add(tblForm);
                tblForm.Show();
            }
        }

        private record TableDropdownItem(Table Table)
        {
            public override string ToString() => $"{Table.TableName} ({Table.StatusDisplay})";
        }
    }
}
