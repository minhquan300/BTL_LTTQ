using QuanLyNhaHang.Helpers;

namespace QuanLyNhaHang.Forms
{
    partial class FormOrderDetail
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && components != null) components.Dispose(); base.Dispose(disposing); }

        private ListBox lvCategories = null!;
        private DataGridView dgvMenu = null!;
        private FlowLayoutPanel flowMenuCards = null!;
        private Panel pnlMenuContent = null!, pnlEmpty = null!;
        private TextBox txtSearch = null!;
        private Label lblCount = null!;
        private Label lblEmptyMessage = null!;
        private Button btnAddItem = null!, btnEditItem = null!, btnDeleteItem = null!, btnAddCat = null!, btnImportImages = null!, btnGridView = null!, btnTableView = null!, btnEmptyAdd = null!;

        private void InitializeComponent()
        {
            this.BackColor = UITheme.Light;
            this.Load += FormOrderDetail_Load;

            // ── Toolbar ─────────────────────────────────────────
            var pnlBar = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = Color.White, Padding = new Padding(16, 0, 16, 0) };
            pnlBar.Paint += (s, e) => { using var p = new Pen(UITheme.Border); e.Graphics.DrawLine(p, 0, pnlBar.Height - 1, pnlBar.Width, pnlBar.Height - 1); };
            this.Controls.Add(pnlBar);

            txtSearch = new TextBox { PlaceholderText = "Tìm món theo tên...", Font = UITheme.FontNormal, Size = new Size(300, 32), Location = new Point(16, 12), BorderStyle = BorderStyle.FixedSingle, Anchor = AnchorStyles.Left | AnchorStyles.Top };
            txtSearch.TextChanged += txtSearch_TextChanged;
            pnlBar.Controls.Add(txtSearch);

            lblCount = new Label { Font = UITheme.FontNormal, ForeColor = UITheme.TextMuted, AutoSize = true, Location = new Point(328, 18), Anchor = AnchorStyles.Left | AnchorStyles.Top };
            pnlBar.Controls.Add(lblCount);

            var pnlActions = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                Width = 630,
                Height = 56,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 10, 0, 0),
                Margin = Padding.Empty
            };
            pnlBar.Controls.Add(pnlActions);

            Button MakeToolbarButton(string text, int width, Color backColor, Color foreColor, Color hoverColor, bool outlined = false)
            {
                var button = new Button
                {
                    Text = text,
                    Size = new Size(width, 36),
                    Margin = new Padding(0, 0, 8, 0),
                    Font = UITheme.FontNormal,
                    BackColor = backColor,
                    ForeColor = foreColor,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    UseVisualStyleBackColor = false
                };
                button.FlatAppearance.BorderSize = outlined ? 1 : 0;
                button.FlatAppearance.BorderColor = UITheme.Border;
                UITheme.ApplyHoverEffect(button, backColor, hoverColor);
                UITheme.ApplyRoundedCorners(button);
                pnlActions.Controls.Add(button);
                return button;
            }

            btnAddItem = MakeToolbarButton("+ Thêm món", 112, UITheme.Success, Color.White, ControlPaint.Dark(UITheme.Success, 0.08f));
            btnAddItem.Click += btnAddItem_Click;
            btnEditItem = MakeToolbarButton("Sửa món", 94, UITheme.PrimarySoft, UITheme.Primary, UITheme.Selection);
            btnEditItem.Click += btnEditItem_Click;
            btnDeleteItem = MakeToolbarButton("Ẩn/Hiện món", 120, UITheme.WarningSoft, Color.FromArgb(146, 90, 0), Color.FromArgb(253, 230, 138));
            btnDeleteItem.Click += btnDeleteItem_Click;
            btnAddCat = MakeToolbarButton("Thêm danh mục", 136, UITheme.Surface, UITheme.TextPrimary, UITheme.AlternateRow, outlined: true);
            btnAddCat.Click += btnAddCat_Click;
            btnImportImages = MakeToolbarButton("Nhập ảnh hàng loạt", 128, UITheme.PrimarySoft, UITheme.Primary, UITheme.Selection, outlined: true);
            btnImportImages.Click += btnImportImages_Click;

            // ── Split ───────────────────────────────────────────
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                FixedPanel = FixedPanel.Panel1,
                IsSplitterFixed = true,
                Panel1MinSize = 0,
                Panel2MinSize = 0
            };
            void SetSafeMenuSplitterDistance(object sender, EventArgs e)
            {
                var availableWidth = split.ClientSize.Width - split.SplitterWidth;
                if (availableWidth <= 0) return;
                var targetDistance = Math.Min(220, availableWidth);
                if (split.SplitterDistance != targetDistance)
                    split.SplitterDistance = targetDistance;
            }
            split.SizeChanged += SetSafeMenuSplitterDistance;
            split.HandleCreated += SetSafeMenuSplitterDistance;
            this.Controls.Add(split);

            // Left: categories
            split.Panel1.BackColor = UITheme.Light;
            var lblCatTitle = new Label { Text = "DANH MỤC", Font = new Font(UITheme.FontFamily, 9f, FontStyle.Bold), ForeColor = UITheme.TextMuted, Dock = DockStyle.Top, Height = 36, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(16, 0, 0, 0) };
            split.Panel1.Controls.Add(lblCatTitle);

            lvCategories = new ListBox
            {
                Dock = DockStyle.Fill,
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 40,
                IntegralHeight = false,
                BackColor = UITheme.Light,
                Font = UITheme.FontNormal,
                BorderStyle = BorderStyle.None,
                SelectionMode = SelectionMode.One,
                HorizontalScrollbar = false
            };
            lvCategories.DrawItem += lvCategories_DrawItem;
            lvCategories.SelectedIndexChanged += lvCategories_SelectedIndexChanged;
            lvCategories.MouseMove += lvCategories_MouseMove;
            lvCategories.MouseLeave += lvCategories_MouseLeave;
            lvCategories.MouseDown += lvCategories_MouseDown;
            split.Panel1.Controls.Add(lvCategories);

            // Right: menu items
            split.Panel2.BackColor = UITheme.Light;
            var pnlViewMode = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = Color.White, Padding = new Padding(12, 6, 12, 6) };
            btnGridView = new Button
            {
                Text = "Dạng lưới (thẻ)", Size = new Size(132, 34), Location = new Point(12, 6),
                Font = UITheme.FontNormal, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
                BackColor = UITheme.PrimarySoft, ForeColor = UITheme.Primary, UseVisualStyleBackColor = false
            };
            btnGridView.FlatAppearance.BorderSize = 0;
            UITheme.ApplyRoundedCorners(btnGridView, UITheme.ButtonCornerRadius);
            btnGridView.Click += (_, _) => SetMenuView(true);
            btnTableView = new Button
            {
                Text = "Dạng bảng", Size = new Size(108, 34), Location = new Point(150, 6),
                Font = UITheme.FontNormal, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
                BackColor = Color.White, ForeColor = UITheme.TextMuted, UseVisualStyleBackColor = false
            };
            btnTableView.FlatAppearance.BorderSize = 0;
            UITheme.ApplyRoundedCorners(btnTableView, UITheme.ButtonCornerRadius);
            btnTableView.Click += (_, _) => SetMenuView(false);
            pnlViewMode.Controls.Add(btnGridView);
            pnlViewMode.Controls.Add(btnTableView);
            split.Panel2.Controls.Add(pnlViewMode);

            pnlMenuContent = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.Light };
            split.Panel2.Controls.Add(pnlMenuContent);

            flowMenuCards = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(12),
                BackColor = UITheme.Light
            };
            pnlMenuContent.Controls.Add(flowMenuCards);

            dgvMenu = UITheme.CreateDataGrid();
            dgvMenu.Dock = DockStyle.Fill;
            dgvMenu.Visible = false;
            dgvMenu.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMenu.ReadOnly = true;
            dgvMenu.AllowUserToAddRows = false;
            dgvMenu.AllowUserToDeleteRows = false;
            dgvMenu.RowHeadersVisible = false;
            dgvMenu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMenu.MultiSelect = false;
            dgvMenu.AllowUserToResizeRows = false;
            dgvMenu.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgvMenu.RowTemplate.Height = 40;
            dgvMenu.ColumnHeadersHeight = 40;
            dgvMenu.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgvMenu.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvMenu.AlternatingRowsDefaultCellStyle.BackColor = UITheme.AlternateRow;
            dgvMenu.SelectionChanged += dgvMenu_SelectionChanged;
            dgvMenu.CellDoubleClick += dgvMenu_CellDoubleClick;
            dgvMenu.CellFormatting += dgvMenu_CellFormatting;
            dgvMenu.Columns.AddRange(
                new DataGridViewTextBoxColumn { HeaderText = "ID", Name = "colID", Width = 60, AutoSizeMode = DataGridViewAutoSizeColumnMode.None, FillWeight = 7 },
                new DataGridViewTextBoxColumn { HeaderText = "Tên món", Name = "colName", FillWeight = 27 },
                new DataGridViewTextBoxColumn { HeaderText = "Danh mục", Name = "colCat", FillWeight = 18 },
                new DataGridViewTextBoxColumn { HeaderText = "Giá (đ)", Name = "colPrice", FillWeight = 14, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "#,##0" } },
                new DataGridViewTextBoxColumn { HeaderText = "Trạng thái", Name = "colAvail", FillWeight = 16 },
                new DataGridViewTextBoxColumn { HeaderText = "Mô tả", Name = "colDesc", FillWeight = 25 }
            );
            pnlMenuContent.Controls.Add(dgvMenu);

            pnlEmpty = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.Light, Visible = false };
            lblEmptyMessage = new Label { Text = "Chưa có món nào trong danh mục này", Font = UITheme.FontHeader, ForeColor = UITheme.TextMuted, AutoSize = true, TextAlign = ContentAlignment.MiddleCenter };
            btnEmptyAdd = UITheme.CreateButton("+ Thêm món", UITheme.Success);
            btnEmptyAdd.Size = new Size(132, 38);
            btnEmptyAdd.Click += btnAddItem_Click;
            pnlEmpty.Controls.Add(lblEmptyMessage);
            pnlEmpty.Controls.Add(btnEmptyAdd);
            pnlEmpty.Resize += (_, _) => LayoutEmptyState();
            pnlMenuContent.Controls.Add(pnlEmpty);

            split.Panel2.Controls.SetChildIndex(pnlViewMode, 0);

            this.Controls.SetChildIndex(split, 0);
            this.Controls.SetChildIndex(pnlBar, 1);
        }
    }

    // Helper extension for inline control registration
    internal static class ControlExt
    {
        public static T Also<T>(this T self, Action<T> action) { action(self); return self; }
    }
}
