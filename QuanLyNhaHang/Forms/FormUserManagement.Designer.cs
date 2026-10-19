using QuanLyNhaHang.Helpers;

namespace QuanLyNhaHang.Forms
{
    partial class FormUserManagement
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && components != null) components.Dispose(); base.Dispose(disposing); }

        private DataGridView dgvUsers = null!;
        private TextBox txtSearch = null!;
        private Label lblCount = null!;
        private Button btnAdd = null!, btnEdit = null!, btnToggle = null!;

        private void InitializeComponent()
        {
            this.BackColor = UITheme.Light;
            this.Load += FormUserManagement_Load;

            var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.White };
            pnlHeader.Paint += (s, e) => { using var p = new Pen(Color.FromArgb(220, 220, 220)); e.Graphics.DrawLine(p, 0, 59, pnlHeader.Width, 59); };
            this.Controls.Add(pnlHeader);
            new Label { Text = "👥 Quản Lý Nhân Viên", Font = UITheme.FontTitle, ForeColor = UITheme.PrimaryDark, AutoSize = true, Location = new Point(20, 17) }.Also(l => pnlHeader.Controls.Add(l));

            var pnlBar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = Color.White };
            pnlBar.Paint += (s, e) => { using var p = new Pen(Color.FromArgb(220, 220, 220)); e.Graphics.DrawLine(p, 0, 51, pnlBar.Width, 51); };
            this.Controls.Add(pnlBar);

            int bx = 10;
            Button Mk(string t, Color c, EventHandler h) { var b = UITheme.CreateButton(t, c); b.Location = new Point(bx, 9); b.Width = 140; b.Click += h; bx += 148; pnlBar.Controls.Add(b); return b; }
            btnAdd = Mk("➕ Thêm NV", UITheme.Success, btnAdd_Click);
            btnEdit = Mk("✏️ Sửa thông tin", UITheme.Info, btnEdit_Click);
            btnToggle = Mk("🔒 Bật/Tắt TK", UITheme.Warning, btnToggle_Click);

            txtSearch = new TextBox { PlaceholderText = "🔍 Tìm theo tên hoặc username...", Font = UITheme.FontNormal, Size = new Size(250, 32), Location = new Point(bx + 10, 10), BorderStyle = BorderStyle.FixedSingle };
            txtSearch.TextChanged += txtSearch_TextChanged;
            pnlBar.Controls.Add(txtSearch);

            lblCount = new Label { Font = UITheme.FontSmall, ForeColor = UITheme.TextMuted, AutoSize = true, Location = new Point(bx + 275, 17) };
            pnlBar.Controls.Add(lblCount);

            dgvUsers = UITheme.CreateDataGrid();
            dgvUsers.Dock = DockStyle.Fill;
            dgvUsers.Columns.AddRange(
                new DataGridViewTextBoxColumn { HeaderText = "ID", Width = 50, AutoSizeMode = DataGridViewAutoSizeColumnMode.None },
                new DataGridViewTextBoxColumn { HeaderText = "Username", FillWeight = 20 },
                new DataGridViewTextBoxColumn { HeaderText = "Họ tên", FillWeight = 28 },
                new DataGridViewTextBoxColumn { HeaderText = "Vai trò", FillWeight = 20 },
                new DataGridViewTextBoxColumn { HeaderText = "SĐT", FillWeight = 18 },
                new DataGridViewTextBoxColumn { HeaderText = "Trạng thái", FillWeight = 18 },
                new DataGridViewTextBoxColumn { HeaderText = "Ngày tạo", FillWeight = 16 }
            );
            this.Controls.Add(dgvUsers);

            this.Controls.SetChildIndex(dgvUsers, 0);
            this.Controls.SetChildIndex(pnlBar, 1);
            this.Controls.SetChildIndex(pnlHeader, 2);
        }
    }
}