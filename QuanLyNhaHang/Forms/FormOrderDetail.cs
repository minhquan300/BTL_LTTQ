using QuanLyNhaHang.Data;
using QuanLyNhaHang.Helpers;
using QuanLyNhaHang.Models;
using MenuItemModel = QuanLyNhaHang.Models.MenuItem;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
namespace QuanLyNhaHang.Forms
{
    /// <summary>Quản lý thực đơn (Category + MenuItem)</summary>
    public partial class FormOrderDetail : Form
    {
        private readonly MenuRepository _repo = new();
        private readonly System.Windows.Forms.Timer _searchDebounceTimer = new() { Interval = 300 };
        private List<Category> _categories = new();
        private List<MenuItemModel> _items = new();
        private int _selectedCatId = 0;
        private int _hoveredCategoryIndex = -1;
        private int? _selectedMenuItemId;
        private bool _refreshingMenuView;
        private bool _useCardView = true;
        private CategoryListEntry? _contextCategory;
        private readonly ContextMenuStrip _categoryContextMenu = new();
        private readonly ToolStripMenuItem _renameCategoryItem = new("Đổi tên");
        private readonly ToolStripMenuItem _toggleCategoryItem = new("Ẩn");
        private readonly ToolStripMenuItem _deleteCategoryItem = new("Xóa");

        public FormOrderDetail()
        {
            UITheme.ConfigureDpi(this);
            InitializeComponent();
            UITheme.EnableDoubleBuffering(this);
            _searchDebounceTimer.Tick += (_, _) =>
            {
                _searchDebounceTimer.Stop();
                RefreshMenuGrid();
            };
            _categoryContextMenu.Items.AddRange(new ToolStripItem[] { _renameCategoryItem, _toggleCategoryItem, new ToolStripSeparator(), _deleteCategoryItem });
            _renameCategoryItem.Click += async (_, _) => await RenameContextCategoryAsync();
            _toggleCategoryItem.Click += async (_, _) => await ToggleContextCategoryAsync();
            _deleteCategoryItem.Click += async (_, _) => await DeleteContextCategoryAsync();
            FormClosed += (_, _) =>
            {
                _searchDebounceTimer.Dispose();
                _categoryContextMenu.Dispose();
            };
        }

        private async void FormOrderDetail_Load(object sender, EventArgs e)
        {
            await LoadCategoriesAsync();
            await LoadMenuItemsAsync();
        }

        private async Task LoadCategoriesAsync(int selectedCategoryId = 0)
        {
            _categories = await _repo.GetAllCategoriesAsync();
            PopulateCategoryList(selectedCategoryId);
        }

        private void PopulateCategoryList(int selectedCategoryId)
        {
            lvCategories.BeginUpdate();
            lvCategories.Items.Clear();
            lvCategories.Items.Add(new CategoryListEntry(0, "Tất cả", _items.Count, true, true));
            var selectedIndex = 0;
            foreach (var c in _categories)
            {
                var count = _items.Count(m => m.CategoryID == c.CategoryID);
                lvCategories.Items.Add(new CategoryListEntry(c.CategoryID, c.CategoryName, count, c.IsActive, false));
                if (c.CategoryID == selectedCategoryId)
                    selectedIndex = lvCategories.Items.Count - 1;
            }
            lvCategories.EndUpdate();
            lvCategories.SelectedIndex = selectedIndex;
        }

        private async Task LoadMenuItemsAsync()
        {
            _items = await _repo.GetAllMenuItemsAsync();
            PopulateCategoryList(_selectedCatId);
            RefreshMenuGrid();
        }

        private void RefreshMenuGrid()
        {
            var filtered = _selectedCatId == 0
                ? _items
                : _items.Where(m => m.CategoryID == _selectedCatId).ToList();

            var q = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(q))
                filtered = filtered.Where(m => m.ItemName.Contains(q, StringComparison.CurrentCultureIgnoreCase)).ToList();

            _refreshingMenuView = true;
            try
            {
                dgvMenu.Rows.Clear();
                foreach (Control card in flowMenuCards.Controls.Cast<Control>().ToArray())
                    card.Dispose();
                flowMenuCards.Controls.Clear();
                foreach (var m in filtered)
                {
                    var idx = dgvMenu.Rows.Add(m.MenuItemID, m.ItemName, m.CategoryName, m.Price, m.IsAvailable ? "Đang bán" : "Đã ẩn", m.Description ?? "");
                    dgvMenu.Rows[idx].Tag = m;
                    dgvMenu.Rows[idx].Cells["colDesc"].ToolTipText = m.Description ?? string.Empty;
                    if (!m.IsAvailable)
                        dgvMenu.Rows[idx].DefaultCellStyle.ForeColor = Color.FromArgb(145, 150, 156);

                    flowMenuCards.Controls.Add(CreateMenuCard(m));
                }

                if (_selectedMenuItemId.HasValue && filtered.All(m => m.MenuItemID != _selectedMenuItemId.Value))
                    _selectedMenuItemId = null;
                if (_selectedMenuItemId.HasValue)
                {
                    foreach (DataGridViewRow row in dgvMenu.Rows)
                    {
                        if (row.Tag is MenuItemModel item && item.MenuItemID == _selectedMenuItemId.Value)
                        {
                            row.Selected = true;
                            break;
                        }
                    }
                }

                var hasItems = filtered.Count > 0;
                pnlEmpty.Visible = !hasItems;
                flowMenuCards.Visible = hasItems && _useCardView;
                dgvMenu.Visible = hasItems && !_useCardView;
                lblEmptyMessage.Text = string.IsNullOrWhiteSpace(q)
                    ? "Chưa có món nào trong danh mục này"
                    : "Không tìm thấy món phù hợp";
                LayoutEmptyState();
            }
            finally { _refreshingMenuView = false; }

            lblCount.Text = $"Tổng: {filtered.Count} món";
            UpdateCardSelectionStyles();
            UpdateMenuActionButtons();
        }

        private Panel CreateMenuCard(MenuItemModel item)
        {
            var card = new Panel
            {
                Size = new Size(220, 260),
                Margin = new Padding(10),
                BackColor = item.IsAvailable ? UITheme.Surface : UITheme.AlternateRow,
                Tag = item,
                Cursor = Cursors.Hand
            };
            card.Paint += (_, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var selected = _selectedMenuItemId == item.MenuItemID;
                using var pen = new Pen(selected ? UITheme.Primary : UITheme.Border, selected ? 2f : 1f);
                using var path = new System.Drawing.Drawing2D.GraphicsPath();
                const int radius = UITheme.CornerRadius;
                int diameter = radius * 2;
                int right = card.Width - diameter - 2;
                int bottom = card.Height - diameter - 2;
                path.AddArc(1, 1, diameter, diameter, 180, 90);
                path.AddArc(right, 1, diameter, diameter, 270, 90);
                path.AddArc(right, bottom, diameter, diameter, 0, 90);
                path.AddArc(1, bottom, diameter, diameter, 90, 90);
                path.CloseFigure();
                e.Graphics.DrawPath(pen, path);
            };
            var imagePanel = new Panel
            {
                Location = new Point(1, 1),
                Size = new Size(218, 98),
                BackColor = item.IsAvailable ? UITheme.Light : UITheme.AlternateRow,
                Tag = item
            };
            imagePanel.Disposed += (_, _) =>
            {
                var image = imagePanel.BackgroundImage;
                imagePanel.BackgroundImage = null;
                image?.Dispose();
            };
            var imagePath = ResolveMenuImagePath(item.ImagePath);
            if (imagePath != null)
            {
                try
                {
                    using var source = Image.FromFile(imagePath);
                    var bitmap = item.IsAvailable ? new Bitmap(source) : CreateFadedImage(source);
                    imagePanel.BackgroundImage = bitmap;
                    imagePanel.BackgroundImageLayout = ImageLayout.Zoom;
                }
                catch { AddDefaultMenuIcon(imagePanel, item); }
            }
            else
            {
                AddDefaultMenuIcon(imagePanel, item);
            }
            card.Controls.Add(imagePanel);

            var name = new Label
            {
                Text = item.ItemName,
                Location = new Point(12, 108),
                Size = new Size(196, 42),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = item.IsAvailable ? UITheme.TextPrimary : UITheme.TextMuted,
                AutoEllipsis = true,
                Tag = item
            };
            card.Controls.Add(name);

            var category = new Label
            {
                Text = item.CategoryName,
                Location = new Point(12, 151),
                Size = new Size(196, 18),
                Font = UITheme.FontSmall,
                ForeColor = UITheme.TextMuted,
                AutoEllipsis = true,
                Tag = item
            };
            card.Controls.Add(category);

            var price = new Label
            {
                Text = item.Price.ToString("#,##0", CultureInfo.GetCultureInfo("en-US")) + " đ",
                Location = new Point(12, 174),
                Size = new Size(196, 26),
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = item.IsAvailable ? Color.FromArgb(220, 96, 35) : Color.FromArgb(160, 163, 168),
                Tag = item
            };
            card.Controls.Add(price);

            var status = new Label
            {
                Text = item.IsAvailable ? "Đang bán" : "Đã ẩn",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(12, 220),
                Size = new Size(item.IsAvailable ? 76 : 60, 24),
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = item.IsAvailable ? UITheme.Success : UITheme.TextMuted,
                BackColor = item.IsAvailable ? UITheme.SuccessSoft : UITheme.AlternateRow,
                Tag = item
            };
            card.Controls.Add(status);

            AttachMenuCardEvents(card, item);
            return card;
        }

        private static void AddDefaultMenuIcon(Control parent, MenuItemModel item)
        {
            parent.Controls.Add(new Label
            {
                Text = "🍽️",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Emoji", 34f),
                ForeColor = item.IsAvailable ? Color.FromArgb(180, 190, 200) : Color.FromArgb(205, 208, 212),
                BackColor = Color.Transparent,
                Tag = item
            });
        }

        private static string? ResolveMenuImagePath(string? imagePath)
        {
            return MenuImageStorage.Resolve(imagePath);
        }

        private static Image CreateFadedImage(Image source)
        {
            var faded = new Bitmap(source.Width, source.Height);
            using var graphics = Graphics.FromImage(faded);
            using var attributes = new ImageAttributes();
            var gray = new ColorMatrix(new[]
            {
                new[] { .30f, .30f, .30f, 0f, 0f },
                new[] { .59f, .59f, .59f, 0f, 0f },
                new[] { .11f, .11f, .11f, 0f, 0f },
                new[] { 0f, 0f, 0f, .62f, 0f },
                new[] { 0f, 0f, 0f, 0f, 1f }
            });
            attributes.SetColorMatrix(gray);
            graphics.DrawImage(source, new Rectangle(0, 0, faded.Width, faded.Height), 0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attributes);
            return faded;
        }

        private void AttachMenuCardEvents(Control control, MenuItemModel item, Panel? card = null)
        {
            card ??= control as Panel;
            control.Click += (_, _) => SelectMenuItem(item);
            if (card != null && item.IsAvailable)
            {
                control.MouseEnter += (_, _) => { card.BackColor = UITheme.PrimarySoft; card.Invalidate(); };
                control.MouseLeave += (_, _) =>
                {
                    var pointer = card.PointToClient(Cursor.Position);
                    if (!card.ClientRectangle.Contains(pointer))
                    {
                        card.BackColor = UITheme.Surface;
                        card.Invalidate();
                    }
                };
            }
            control.DoubleClick += async (_, _) =>
            {
                SelectMenuItem(item);
                await OpenMenuItemEditAsync(item);
            };
            foreach (Control child in control.Controls)
                AttachMenuCardEvents(child, item, card);
        }

        private void SelectMenuItem(MenuItemModel item)
        {
            _selectedMenuItemId = item.MenuItemID;
            foreach (DataGridViewRow row in dgvMenu.Rows)
                row.Selected = row.Tag is MenuItemModel rowItem && rowItem.MenuItemID == item.MenuItemID;
            UpdateCardSelectionStyles();
            UpdateMenuActionButtons();
        }

        private void UpdateCardSelectionStyles()
        {
            foreach (Control control in flowMenuCards.Controls)
                control.Invalidate();
        }

        private void UpdateMenuActionButtons()
        {
            var hasSelection = _selectedMenuItemId.HasValue && _items.Any(m => m.MenuItemID == _selectedMenuItemId.Value);
            btnEditItem.Enabled = hasSelection;
            btnDeleteItem.Enabled = hasSelection;
        }

        private void LayoutEmptyState()
        {
            if (pnlEmpty == null || lblEmptyMessage == null || btnEmptyAdd == null) return;
            lblEmptyMessage.Left = Math.Max(0, (pnlEmpty.ClientSize.Width - lblEmptyMessage.Width) / 2);
            lblEmptyMessage.Top = Math.Max(0, (pnlEmpty.ClientSize.Height - 76) / 2);
            btnEmptyAdd.Left = (pnlEmpty.ClientSize.Width - btnEmptyAdd.Width) / 2;
            btnEmptyAdd.Top = lblEmptyMessage.Bottom + 12;
        }

        private void SetMenuView(bool useCards)
        {
            _useCardView = useCards;
            btnGridView.BackColor = useCards ? UITheme.PrimarySoft : UITheme.Surface;
            btnGridView.ForeColor = useCards ? UITheme.Primary : UITheme.TextMuted;
            btnTableView.BackColor = useCards ? UITheme.Surface : UITheme.PrimarySoft;
            btnTableView.ForeColor = useCards ? UITheme.TextMuted : UITheme.Primary;
            var hasItems = dgvMenu.Rows.Count > 0;
            flowMenuCards.Visible = hasItems && useCards;
            dgvMenu.Visible = hasItems && !useCards;
            pnlEmpty.Visible = !hasItems;
        }

        private void lvCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvCategories.SelectedItem is CategoryListEntry category)
            {
                _selectedCatId = category.CategoryId;
                RefreshMenuGrid();
            }
        }

        private void lvCategories_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= lvCategories.Items.Count || lvCategories.Items[e.Index] is not CategoryListEntry category)
                return;

            var isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            var isHovered = e.Index == _hoveredCategoryIndex;
            var bounds = e.Bounds;
            var background = isSelected
                ? UITheme.PrimarySoft
                : isHovered ? UITheme.AlternateRow : lvCategories.BackColor;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var brush = new SolidBrush(background))
                e.Graphics.FillRectangle(brush, bounds);

            if (isSelected)
            {
                using var indicator = new SolidBrush(UITheme.Primary);
                e.Graphics.FillRectangle(indicator, bounds.Left, bounds.Top + 5, 3, bounds.Height - 10);
            }

            var nameColor = category.IsActive ? UITheme.TextPrimary : UITheme.TextMuted;
            var textBounds = new Rectangle(bounds.Left + 16, bounds.Top, bounds.Width - 58, bounds.Height);
            TextRenderer.DrawText(e.Graphics, category.Name, UITheme.FontNormal, textBounds, nameColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

            var badge = new Rectangle(bounds.Right - 39, bounds.Top + 9, 28, 22);
            using (var badgeBrush = new SolidBrush(isSelected ? UITheme.Surface : UITheme.Border))
                e.Graphics.FillEllipse(badgeBrush, badge);
            TextRenderer.DrawText(e.Graphics, category.ItemCount.ToString(), UITheme.FontSmall, badge, UITheme.TextMuted,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }

        private void lvCategories_MouseMove(object? sender, MouseEventArgs e)
        {
            var index = lvCategories.IndexFromPoint(e.Location);
            if (index == _hoveredCategoryIndex) return;
            var oldIndex = _hoveredCategoryIndex;
            _hoveredCategoryIndex = index;
            if (oldIndex >= 0) lvCategories.Invalidate(lvCategories.GetItemRectangle(oldIndex));
            if (index >= 0) lvCategories.Invalidate(lvCategories.GetItemRectangle(index));
        }

        private void lvCategories_MouseLeave(object? sender, EventArgs e)
        {
            if (_hoveredCategoryIndex < 0) return;
            var oldIndex = _hoveredCategoryIndex;
            _hoveredCategoryIndex = -1;
            lvCategories.Invalidate(lvCategories.GetItemRectangle(oldIndex));
        }

        private void lvCategories_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            var index = lvCategories.IndexFromPoint(e.Location);
            if (index < 0 || lvCategories.Items[index] is not CategoryListEntry category || category.IsAll)
                return;

            lvCategories.SelectedIndex = index;
            _contextCategory = category;
            _toggleCategoryItem.Text = category.IsActive ? "Ẩn" : "Hiện";
            _deleteCategoryItem.Enabled = category.ItemCount == 0;
            _deleteCategoryItem.ToolTipText = category.ItemCount == 0
                ? "Xóa danh mục"
                : "Chỉ xóa được danh mục chưa có món";
            _categoryContextMenu.Show(lvCategories, e.Location);
        }

        private async Task RenameContextCategoryAsync()
        {
            if (_contextCategory == null) return;
            var category = _categories.FirstOrDefault(c => c.CategoryID == _contextCategory.CategoryId);
            if (category == null) return;

            var name = ValidationHelper.ShowInputDialog("Nhập tên danh mục mới:", "Đổi tên danh mục", category.CategoryName);
            if (string.IsNullOrWhiteSpace(name) || name.Trim() == category.CategoryName) return;
            category.CategoryName = name.Trim();
            await _repo.UpdateCategoryAsync(category);
            await LoadCategoriesAsync(category.CategoryID);
            await LoadMenuItemsAsync();
        }

        private async Task ToggleContextCategoryAsync()
        {
            if (_contextCategory == null) return;
            var category = _categories.FirstOrDefault(c => c.CategoryID == _contextCategory.CategoryId);
            if (category == null) return;

            category.IsActive = !category.IsActive;
            var selectedId = !category.IsActive && _selectedCatId == category.CategoryID ? 0 : _selectedCatId;
            await _repo.UpdateCategoryAsync(category);
            await LoadCategoriesAsync(selectedId);
        }

        private async Task DeleteContextCategoryAsync()
        {
            if (_contextCategory == null) return;
            var category = _categories.FirstOrDefault(c => c.CategoryID == _contextCategory.CategoryId);
            if (category == null) return;
            if (_items.Any(m => m.CategoryID == category.CategoryID))
            {
                ValidationHelper.ShowError("Không thể xóa danh mục đang có món. Hãy chuyển món sang danh mục khác trước.");
                return;
            }
            if (!ValidationHelper.Confirm($"Xóa danh mục '{category.CategoryName}'?")) return;

            var deleted = await _repo.DeleteCategoryAsync(category.CategoryID);
            if (!deleted)
            {
                ValidationHelper.ShowError("Danh mục này đang được sử dụng nên không thể xóa.");
                return;
            }

            var selectedId = _selectedCatId == category.CategoryID ? 0 : _selectedCatId;
            await LoadCategoriesAsync(selectedId);
        }

        private async void btnAddItem_Click(object sender, EventArgs e)
        {
            using var dlg = new FormMenuItemEdit(null, _categories);
            if (dlg.ShowDialog() == DialogResult.OK && dlg.ResultItem != null)
            {
                await _repo.CreateMenuItemAsync(dlg.ResultItem);
                _selectedCatId = dlg.ResultItem.CategoryID;
                await LoadMenuItemsAsync();
                ValidationHelper.ShowSuccess("Thêm món thành công!");
            }
        }

        private async void btnEditItem_Click(object sender, EventArgs e)
        {
            var m = GetSelectedMenuItem();
            if (m == null) return;
            await OpenMenuItemEditAsync(m);
        }

        private async Task OpenMenuItemEditAsync(MenuItemModel m)
        {
            using var dlg = new FormMenuItemEdit(m, _categories);
            if (dlg.ShowDialog() == DialogResult.OK && dlg.ResultItem != null)
            {
                await _repo.UpdateMenuItemAsync(dlg.ResultItem);
                _selectedCatId = dlg.ResultItem.CategoryID;
                await LoadMenuItemsAsync();
                ValidationHelper.ShowSuccess("Cập nhật món thành công!");
            }
        }

        private async void btnDeleteItem_Click(object sender, EventArgs e)
        {
            var m = GetSelectedMenuItem();
            if (m == null) return;
            if (m.IsAvailable && !ValidationHelper.Confirm($"Ẩn món '{m.ItemName}' khỏi thực đơn?")) return;
            await _repo.SetMenuItemAvailabilityAsync(m.MenuItemID, !m.IsAvailable);
            await LoadMenuItemsAsync();
            ValidationHelper.ShowSuccess(m.IsAvailable ? "Đã ẩn món!" : "Đã hiện món!");
        }

        private async void btnAddCat_Click(object sender, EventArgs e)
        {
            var name = ValidationHelper.ShowInputDialog("Nhập tên danh mục mới:", "Thêm danh mục món ăn");
            if (string.IsNullOrWhiteSpace(name)) return;
            await _repo.CreateCategoryAsync(new Category { CategoryName = name.Trim(), IsActive = true });
            await LoadCategoriesAsync();
            ValidationHelper.ShowSuccess("Thêm danh mục thành công!");
        }

        private async void btnImportImages_Click(object? sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Chọn thư mục chứa ảnh món ăn. Tên ảnh nên gần giống tên món để ghép tự động.",
                ShowNewFolderButton = false
            };
            if (dialog.ShowDialog(this) != DialogResult.OK) return;

            var sourceFiles = Directory.EnumerateFiles(dialog.SelectedPath, "*", SearchOption.TopDirectoryOnly)
                .Where(MenuImageStorage.IsSupportedImage)
                .ToList();
            if (sourceFiles.Count == 0)
            {
                MessageBox.Show(this, "Thư mục không có ảnh JPG, JPEG, JFIF, PNG, BMP hoặc GIF.",
                    "Không tìm thấy ảnh", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var sourceByItem = new Dictionary<int, string>();
            var usedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in _items)
            {
                string? existingImage = MenuImageStorage.Resolve(item.ImagePath);
                if (existingImage == null) continue;

                bool alreadyPortable = !Path.IsPathRooted(item.ImagePath) &&
                    item.ImagePath!.StartsWith("Images" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
                if (!alreadyPortable)
                    sourceByItem[item.MenuItemID] = existingImage;
                usedFiles.Add(Path.GetFullPath(existingImage));
            }

            var pendingItems = _items.Where(item => !sourceByItem.ContainsKey(item.MenuItemID) &&
                !(item.ImagePath?.StartsWith("Images" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) == true &&
                  MenuImageStorage.Resolve(item.ImagePath) != null)).ToList();

            foreach (var item in pendingItems)
            {
                var candidates = sourceFiles
                    .Where(file => !usedFiles.Contains(Path.GetFullPath(file)))
                    .Select(file => new { File = file, Score = GetImageNameMatchScore(item.ItemName, Path.GetFileNameWithoutExtension(file)) })
                    .Where(candidate => candidate.Score >= 0.75)
                    .OrderByDescending(candidate => candidate.Score)
                    .ToList();

                if (candidates.Count == 0) continue;
                if (candidates.Count > 1 && candidates[0].Score - candidates[1].Score < 0.08) continue;

                sourceByItem[item.MenuItemID] = candidates[0].File;
                usedFiles.Add(Path.GetFullPath(candidates[0].File));
            }

            if (sourceByItem.Count == 0)
            {
                MessageBox.Show(this, "Không tìm thấy ảnh nào ghép được với món ăn hoặc các đường dẫn ảnh cũ không còn truy cập được.",
                    "Chưa có ảnh để nhập", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var skippedItems = _items.Where(item => !sourceByItem.ContainsKey(item.MenuItemID) &&
                (string.IsNullOrWhiteSpace(item.ImagePath) || MenuImageStorage.Resolve(item.ImagePath) == null)).ToList();
            string skippedPreview = skippedItems.Count == 0 ? string.Empty :
                "\n\nKhông ghép được an toàn (sẽ bỏ qua): " + string.Join(", ", skippedItems.Take(8).Select(item => item.ItemName)) +
                (skippedItems.Count > 8 ? $" và {skippedItems.Count - 8} món khác." : ".");
            var answer = MessageBox.Show(this,
                $"Tìm thấy ảnh cho {sourceByItem.Count} món. Ảnh sẽ được chép vào thư mục Images cạnh ứng dụng và đường dẫn trong database sẽ chuyển thành đường dẫn tương đối.{skippedPreview}\n\nTiếp tục?",
                "Nhập ảnh hàng loạt", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (answer != DialogResult.Yes) return;

            btnImportImages.Enabled = false;
            int imported = 0;
            var failures = new List<string>();
            try
            {
                foreach (var item in _items.Where(item => sourceByItem.ContainsKey(item.MenuItemID)))
                {
                    try
                    {
                        string relativePath = MenuImageStorage.CopyIntoApplicationImages(
                            sourceByItem[item.MenuItemID], item.MenuItemID);
                        await _repo.UpdateMenuItemImagePathAsync(item.MenuItemID, relativePath);
                        imported++;
                    }
                    catch (Exception ex)
                    {
                        failures.Add($"{item.ItemName}: {ex.Message}");
                    }
                }

                await LoadMenuItemsAsync();
                string result = $"Đã cập nhật đường dẫn ảnh cho {imported} món.";
                if (skippedItems.Count > 0) result += $"\nBỏ qua {skippedItems.Count} món không ghép được chắc chắn.";
                if (failures.Count > 0) result += $"\nKhông cập nhật được {failures.Count} món: " +
                    string.Join("; ", failures.Take(5)) + (failures.Count > 5 ? "; ..." : string.Empty);
                MessageBox.Show(this, result, "Hoàn tất nhập ảnh", MessageBoxButtons.OK,
                    failures.Count > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
            }
            finally
            {
                btnImportImages.Enabled = true;
            }
        }

        private static double GetImageNameMatchScore(string menuItemName, string fileName)
        {
            string normalizedItem = MenuImageStorage.NormalizeForMatching(menuItemName);
            string normalizedFile = MenuImageStorage.NormalizeForMatching(fileName);
            if (normalizedItem.Length == 0 || normalizedFile.Length == 0) return 0;
            if (normalizedItem == normalizedFile) return 1;

            var itemWords = normalizedItem.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToHashSet();
            var fileWords = normalizedFile.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToHashSet();
            int commonWords = itemWords.Intersect(fileWords).Count();
            if (commonWords < 2) return 0;
            double wordScore = (double)commonWords / Math.Max(itemWords.Count, fileWords.Count);
            bool prefixMatch = normalizedItem.StartsWith(normalizedFile + " ", StringComparison.Ordinal) ||
                normalizedFile.StartsWith(normalizedItem + " ", StringComparison.Ordinal);
            return prefixMatch ? Math.Max(wordScore, 0.88) : wordScore;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            _searchDebounceTimer.Stop();
            _searchDebounceTimer.Start();
        }

        private void dgvMenu_SelectionChanged(object? sender, EventArgs e)
        {
            if (_refreshingMenuView) return;
            if (dgvMenu.SelectedRows.Count > 0 && dgvMenu.SelectedRows[0].Tag is MenuItemModel item)
                _selectedMenuItemId = item.MenuItemID;
            else if (!_useCardView)
                _selectedMenuItemId = null;
            UpdateCardSelectionStyles();
            UpdateMenuActionButtons();
        }

        private void dgvMenu_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvMenu.Rows[e.RowIndex].Tag is not MenuItemModel item) return;
            SelectMenuItem(item);
            _ = OpenMenuItemEditAsync(item);
        }

        private void dgvMenu_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            var descriptionColumn = dgvMenu.Columns["colDesc"];
            if (e.RowIndex < 0 || descriptionColumn == null || e.ColumnIndex != descriptionColumn.Index ||
                dgvMenu.Rows[e.RowIndex].Tag is not MenuItemModel item || string.IsNullOrEmpty(item.Description))
                return;

            var font = e.CellStyle.Font ?? dgvMenu.Font;
            var availableWidth = Math.Max(0, descriptionColumn.Width - 14);
            var fullText = item.Description;
            if (TextRenderer.MeasureText(fullText, font, Size.Empty, TextFormatFlags.SingleLine | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix).Width <= availableWidth)
                return;

            const string ellipsis = "...";
            var low = 0;
            var high = fullText.Length;
            while (low < high)
            {
                var mid = (low + high + 1) / 2;
                var candidate = fullText[..mid] + ellipsis;
                var measured = TextRenderer.MeasureText(candidate, font, Size.Empty, TextFormatFlags.SingleLine | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix).Width;
                if (measured <= availableWidth) low = mid;
                else high = mid - 1;
            }

            e.Value = fullText[..low] + ellipsis;
            e.FormattingApplied = true;
        }

        private MenuItemModel? GetSelectedMenuItem() => _selectedMenuItemId.HasValue
            ? _items.FirstOrDefault(m => m.MenuItemID == _selectedMenuItemId.Value)
            : null;

        private sealed record CategoryListEntry(int CategoryId, string Name, int ItemCount, bool IsActive, bool IsAll);
    }
}
