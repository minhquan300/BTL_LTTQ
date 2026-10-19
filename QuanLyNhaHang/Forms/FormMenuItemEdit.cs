using QuanLyNhaHang.Helpers;
using QuanLyNhaHang.Models;
using MenuItemModel = QuanLyNhaHang.Models.MenuItem;

namespace QuanLyNhaHang.Forms
{
    public class FormMenuItemEdit : Form
    {
        public MenuItemModel? ResultItem { get; private set; }
        private readonly MenuItemModel? _existing;
        private readonly List<Category> _categories;
        private string? _selectedImagePath;

        private TextBox txtName = null!, txtDesc = null!;
        private NumericUpDown numPrice = null!;
        private ComboBox cboCategory = null!;
        private CheckBox chkAvailable = null!;
        private PictureBox picPreview = null!;
        private Label lblImagePlaceholder = null!, lblImageName = null!;
        private Label lblNameError = null!, lblCategoryError = null!, lblPriceError = null!;

        public FormMenuItemEdit(MenuItemModel? existing, List<Category> categories)
        {
            _existing = existing;
            _categories = categories;
            UITheme.ConfigureDpi(this);
            BuildUI();
            UITheme.EnableDoubleBuffering(this);
            if (existing != null) FillData(existing);
        }

        private void BuildUI()
        {
            Text = _existing == null ? "Thêm món" : "Sửa món";
            ClientSize = new Size(480, 550);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            BackColor = Color.White;
            Font = UITheme.FontNormal;

            var header = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = UITheme.Primary };
            header.Controls.Add(new Label
            {
                Text = _existing == null ? "+  Thêm món mới" : "Sửa thông tin món",
                Font = UITheme.FontHeader,
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            });
            Controls.Add(header);

            AddFieldLabel("Tên món *", 28, 77);
            txtName = new TextBox { Location = new Point(158, 72), Size = new Size(294, 30), BorderStyle = BorderStyle.FixedSingle, Font = UITheme.FontNormal };
            txtName.TextChanged += (_, _) => { if (!string.IsNullOrWhiteSpace(txtName.Text)) SetFieldError(lblNameError, string.Empty); };
            Controls.Add(txtName);
            lblNameError = AddErrorLabel(158, 103);

            AddFieldLabel("Danh mục *", 28, 139);
            cboCategory = new ComboBox { Location = new Point(158, 132), Size = new Size(294, 30), DropDownStyle = ComboBoxStyle.DropDownList, Font = UITheme.FontNormal };
            foreach (var category in _categories.Where(c => c.IsActive || c.CategoryID == _existing?.CategoryID))
                cboCategory.Items.Add(new CategoryItem(category.CategoryID, category.CategoryName));
            if (cboCategory.Items.Count > 0) cboCategory.SelectedIndex = 0;
            cboCategory.SelectedIndexChanged += (_, _) => SetFieldError(lblCategoryError, string.Empty);
            Controls.Add(cboCategory);
            lblCategoryError = AddErrorLabel(158, 163);

            AddFieldLabel("Giá (đ) *", 28, 201);
            numPrice = new NumericUpDown
            {
                Location = new Point(158, 194),
                Size = new Size(294, 30),
                Minimum = 0,
                Maximum = 10_000_000_000,
                ThousandsSeparator = true,
                DecimalPlaces = 0,
                Font = UITheme.FontNormal
            };
            numPrice.ValueChanged += (_, _) => { if (numPrice.Value > 0) SetFieldError(lblPriceError, string.Empty); };
            Controls.Add(numPrice);
            lblPriceError = AddErrorLabel(158, 225);

            AddFieldLabel("Mô tả", 28, 260);
            txtDesc = new TextBox
            {
                Location = new Point(158, 254),
                Size = new Size(294, 72),
                BorderStyle = BorderStyle.FixedSingle,
                Font = UITheme.FontNormal,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
            Controls.Add(txtDesc);

            AddFieldLabel("Ảnh món", 28, 348);
            picPreview = new PictureBox
            {
                Location = new Point(158, 340),
                Size = new Size(112, 88),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = UITheme.Light,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            lblImagePlaceholder = new Label
            {
                Text = "🍽️",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI Emoji", 28f),
                ForeColor = UITheme.TextMuted,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            picPreview.Controls.Add(lblImagePlaceholder);
            Controls.Add(picPreview);

            var btnChooseImage = new Button
            {
                Text = "Chọn tệp ảnh...",
                Location = new Point(282, 340),
                Size = new Size(170, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = UITheme.AlternateRow,
                ForeColor = UITheme.TextPrimary,
                Font = UITheme.FontNormal,
                Cursor = Cursors.Hand
            };
            btnChooseImage.FlatAppearance.BorderColor = UITheme.Border;
            btnChooseImage.FlatAppearance.BorderSize = 1;
            UITheme.ApplyRoundedCorners(btnChooseImage);
            UITheme.ApplyHoverEffect(btnChooseImage, UITheme.AlternateRow);
            btnChooseImage.Click += BtnChooseImage_Click;
            Controls.Add(btnChooseImage);

            lblImageName = new Label
            {
                Text = "Chưa chọn ảnh",
                Location = new Point(282, 380),
                Size = new Size(170, 42),
                Font = UITheme.FontSmall,
                ForeColor = UITheme.TextMuted,
                AutoEllipsis = true
            };
            Controls.Add(lblImageName);

            chkAvailable = new CheckBox
            {
                Appearance = Appearance.Button,
                AutoSize = false,
                Location = new Point(158, 438),
                Size = new Size(130, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = UITheme.SuccessSoft,
                ForeColor = UITheme.Success,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };
            chkAvailable.FlatAppearance.BorderSize = 0;
            UITheme.ApplyRoundedCorners(chkAvailable);
            chkAvailable.CheckedChanged += (_, _) => UpdateAvailabilitySwitch();
            Controls.Add(chkAvailable);

            var btnCancel = new Button
            {
                Text = "Hủy",
                Location = new Point(228, 494),
                Size = new Size(104, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = UITheme.AlternateRow,
                ForeColor = UITheme.TextPrimary,
                Font = UITheme.FontNormal,
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            UITheme.ApplyRoundedCorners(btnCancel);
            UITheme.ApplyHoverEffect(btnCancel, UITheme.AlternateRow);
            Controls.Add(btnCancel);

            var btnSave = new Button
            {
                Text = "Lưu",
                Location = new Point(348, 494),
                Size = new Size(104, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = UITheme.Success,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            UITheme.ApplyRoundedCorners(btnSave);
            UITheme.ApplyHoverEffect(btnSave, UITheme.Success, ControlPaint.Dark(UITheme.Success, 0.08f));
            btnSave.Click += BtnSave_Click;
            Controls.Add(btnSave);

            AcceptButton = btnSave;
            CancelButton = btnCancel;
        }

        private void FillData(MenuItemModel item)
        {
            txtName.Text = item.ItemName;
            numPrice.Value = Math.Min(numPrice.Maximum, Math.Max(numPrice.Minimum, decimal.Truncate(item.Price)));
            txtDesc.Text = item.Description ?? string.Empty;
            chkAvailable.Checked = item.IsAvailable;
            for (var i = 0; i < cboCategory.Items.Count; i++)
            {
                if (cboCategory.Items[i] is CategoryItem category && category.Id == item.CategoryID)
                {
                    cboCategory.SelectedIndex = i;
                    break;
                }
            }

            _selectedImagePath = item.ImagePath;
            if (!string.IsNullOrWhiteSpace(item.ImagePath))
            {
                lblImageName.Text = Path.GetFileName(item.ImagePath);
                var path = ResolveImagePath(item.ImagePath);
                if (path != null) SetPreviewImage(path);
            }
        }

        private void BtnChooseImage_Click(object? sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog
            {
                Title = "Chọn ảnh món ăn",
                Filter = "Tệp ảnh|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Tất cả tệp|*.*",
                CheckFileExists = true,
                Multiselect = false
            };
            if (!string.IsNullOrWhiteSpace(_selectedImagePath))
            {
                var directory = Path.GetDirectoryName(_selectedImagePath);
                if (!string.IsNullOrWhiteSpace(directory) && Directory.Exists(directory)) dialog.InitialDirectory = directory;
            }

            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            _selectedImagePath = dialog.FileName;
            lblImageName.Text = Path.GetFileName(dialog.FileName);
            SetPreviewImage(dialog.FileName);
        }

        private void SetPreviewImage(string path)
        {
            try
            {
                using var source = Image.FromFile(path);
                var preview = new Bitmap(source);
                var oldImage = picPreview.Image;
                picPreview.Image = preview;
                oldImage?.Dispose();
                lblImagePlaceholder.Visible = false;
            }
            catch
            {
                lblImagePlaceholder.Visible = true;
            }
        }

        private static string? ResolveImagePath(string imagePath)
        {
            var path = Path.IsPathRooted(imagePath) ? imagePath : Path.Combine(AppContext.BaseDirectory, imagePath);
            return File.Exists(path) ? path : null;
        }

        private void UpdateAvailabilitySwitch()
        {
            if (chkAvailable == null) return;
            chkAvailable.Text = chkAvailable.Checked ? "●  Đang bán" : "○  Đã ẩn";
            chkAvailable.BackColor = chkAvailable.Checked ? UITheme.SuccessSoft : UITheme.AlternateRow;
            chkAvailable.ForeColor = chkAvailable.Checked ? UITheme.Success : UITheme.TextMuted;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            var isValid = true;
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                SetFieldError(lblNameError, "Vui lòng nhập tên món.");
                isValid = false;
            }
            else SetFieldError(lblNameError, string.Empty);

            if (cboCategory.SelectedItem is not CategoryItem)
            {
                SetFieldError(lblCategoryError, "Vui lòng chọn danh mục.");
                isValid = false;
            }
            else SetFieldError(lblCategoryError, string.Empty);

            if (numPrice.Value <= 0)
            {
                SetFieldError(lblPriceError, "Giá phải lớn hơn 0.");
                isValid = false;
            }
            else SetFieldError(lblPriceError, string.Empty);

            if (!isValid)
            {
                if (string.IsNullOrWhiteSpace(txtName.Text)) txtName.Focus();
                else if (cboCategory.SelectedItem is not CategoryItem) cboCategory.Focus();
                else numPrice.Focus();
                return;
            }

            var selectedCategory = (CategoryItem)cboCategory.SelectedItem!;
            ResultItem = new MenuItemModel
            {
                MenuItemID = _existing?.MenuItemID ?? 0,
                ItemName = txtName.Text.Trim(),
                CategoryID = selectedCategory.Id,
                Price = numPrice.Value,
                Description = string.IsNullOrWhiteSpace(txtDesc.Text) ? null : txtDesc.Text.Trim(),
                ImagePath = _selectedImagePath,
                IsAvailable = chkAvailable.Checked
            };
            DialogResult = DialogResult.OK;
        }

        private void AddFieldLabel(string text, int x, int y)
        {
            Controls.Add(new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(120, 24),
                Font = UITheme.FontNormal,
                ForeColor = UITheme.TextPrimary,
                TextAlign = ContentAlignment.MiddleLeft
            });
        }

        private Label AddErrorLabel(int x, int y)
        {
            var label = new Label
            {
                Location = new Point(x, y),
                Size = new Size(294, 18),
                Font = UITheme.FontSmall,
                ForeColor = UITheme.Danger,
                AutoEllipsis = true
            };
            Controls.Add(label);
            return label;
        }

        private static void SetFieldError(Label label, string message) => label.Text = message;

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            var image = picPreview?.Image;
            if (picPreview != null) picPreview.Image = null;
            image?.Dispose();
            base.OnFormClosed(e);
        }

        private sealed record CategoryItem(int Id, string Name)
        {
            public override string ToString() => Name;
        }
    }
}
