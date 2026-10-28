namespace QuanLyNhaHang.Forms
{
    partial class FormPayment
    {
        private void InitializeComponent()
        {
            pnlHead = new Panel();
            lblHead = new Label();
            footer = new Panel();
            btnCancel = new Button();
            btnConfirm = new Button();
            lblConfirmHint = new Label();
            chkQrReceived = new CheckBox();
            bodyViewport = new Panel();
            content = new TableLayoutPanel();
            summary = new Panel();
            lblTotalCaption = new Label();
            lblFinal = new Label();
            lblTotal = new Label();
            lblDiscountCaption = new Label();
            lblDiscount = new Label();
            discountRow = new Panel();
            lblDiscountInput = new Label();
            txtDiscount = new TextBox();
            btnDiscountPercent = new Button();
            btnDiscountAmount = new Button();
            methodRow = new FlowLayoutPanel();
            btnMethodCash = new Button();
            btnMethodTransfer = new Button();
            btnMethodCard = new Button();
            panelHost = new Panel();
            pnlCash = new Panel();
            lblCashLabel = new Label();
            txtCash = new TextBox();
            lblQuickCaption = new Label();
            cashQuickPanel = new Panel();
            btnCash50 = new Button();
            btnCash100 = new Button();
            btnCash200 = new Button();
            btnCash500 = new Button();
            btnCashExact = new Button();
            lblChangeLabel = new Label();
            lblChange = new Label();
            pnlTransfer = new Panel();
            picQr = new PictureBox();
            lblQrStatus = new Label();
            lblQrCountdown = new Label();
            lblQrBankCaption = new Label();
            lblQrBankValue = new Label();
            btnCopyBank = new Button();
            lblQrAccountCaption = new Label();
            lblQrAccountValue = new Label();
            btnCopyAccount = new Button();
            lblQrOwnerCaption = new Label();
            lblQrOwnerValue = new Label();
            btnCopyOwner = new Button();
            lblQrAmountCaption = new Label();
            lblQrAmountValue = new Label();
            btnCopyAmount = new Button();
            lblQrContentCaption = new Label();
            lblQrContentValue = new Label();
            btnCopyContent = new Button();
            btnRefreshQr = new Button();
            lblQrNote = new Label();
            pnlCard = new Panel();
            lblCardIcon = new Label();
            lblCardTitle = new Label();
            lblCardInfo = new Label();
            lblApprovalCaption = new Label();
            txtCardApprovalCode = new TextBox();
            cboDiscountMode = new ComboBox();
            numDiscount = new NumericUpDown();
            cboMethod = new ComboBox();
            numCash = new NumericUpDown();
            pnlHead.SuspendLayout();
            footer.SuspendLayout();
            bodyViewport.SuspendLayout();
            content.SuspendLayout();
            summary.SuspendLayout();
            discountRow.SuspendLayout();
            methodRow.SuspendLayout();
            panelHost.SuspendLayout();
            pnlCash.SuspendLayout();
            cashQuickPanel.SuspendLayout();
            pnlTransfer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picQr).BeginInit();
            pnlCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numDiscount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCash).BeginInit();
            SuspendLayout();
            // 
            // pnlHead
            // 
            pnlHead.BackColor = Color.FromArgb(37, 99, 235);
            pnlHead.Controls.Add(lblHead);
            pnlHead.Dock = DockStyle.Top;
            pnlHead.Location = new Point(0, 0);
            pnlHead.Name = "pnlHead";
            pnlHead.Size = new Size(560, 56);
            pnlHead.TabIndex = 0;
            // 
            // lblHead
            // 
            lblHead.Dock = DockStyle.Fill;
            lblHead.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblHead.ForeColor = Color.White;
            lblHead.Location = new Point(0, 0);
            lblHead.Name = "lblHead";
            lblHead.Size = new Size(560, 56);
            lblHead.TabIndex = 0;
            lblHead.Text = "💳  THANH TOÁN HÓA ĐƠN";
            lblHead.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // footer
            // 
            footer.BackColor = Color.White;
            footer.Controls.Add(btnCancel);
            footer.Controls.Add(btnConfirm);
            footer.Controls.Add(lblConfirmHint);
            footer.Controls.Add(chkQrReceived);
            footer.Dock = DockStyle.Bottom;
            footer.Location = new Point(0, 608);
            footer.Name = "footer";
            footer.Padding = new Padding(16, 8, 16, 8);
            footer.Size = new Size(560, 112);
            footer.TabIndex = 2;
            footer.Resize += footer_Resize;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.White;
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Segoe UI", 10F);
            btnCancel.ForeColor = Color.FromArgb(107, 114, 128);
            btnCancel.Location = new Point(16, 52);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(92, 44);
            btnCancel.TabIndex = 0;
            btnCancel.Text = "Hủy";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnConfirm
            // 
            btnConfirm.BackColor = Color.FromArgb(22, 163, 74);
            btnConfirm.FlatAppearance.BorderSize = 0;
            btnConfirm.FlatStyle = FlatStyle.Flat;
            btnConfirm.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnConfirm.ForeColor = Color.White;
            btnConfirm.Location = new Point(324, 52);
            btnConfirm.Name = "btnConfirm";
            btnConfirm.Size = new Size(220, 44);
            btnConfirm.TabIndex = 2;
            btnConfirm.Text = "✔  Xác nhận thanh toán";
            btnConfirm.UseVisualStyleBackColor = false;
            btnConfirm.Click += BtnOK_Click;
            // 
            // lblConfirmHint
            // 
            lblConfirmHint.Font = new Font("Segoe UI", 9F);
            lblConfirmHint.ForeColor = Color.FromArgb(107, 114, 128);
            lblConfirmHint.Location = new Point(116, 52);
            lblConfirmHint.Name = "lblConfirmHint";
            lblConfirmHint.Size = new Size(192, 44);
            lblConfirmHint.TabIndex = 1;
            lblConfirmHint.TextAlign = ContentAlignment.MiddleCenter;
            lblConfirmHint.Visible = false;
            // 
            // chkQrReceived
            // 
            chkQrReceived.AutoSize = true;
            chkQrReceived.Font = new Font("Segoe UI", 9F);
            chkQrReceived.ForeColor = Color.FromArgb(31, 41, 55);
            chkQrReceived.Location = new Point(16, 8);
            chkQrReceived.Name = "chkQrReceived";
            chkQrReceived.Size = new Size(200, 19);
            chkQrReceived.TabIndex = 3;
            chkQrReceived.Text = "Tôi đã kiểm tra tiền vào tài khoản";
            chkQrReceived.Visible = false;
            chkQrReceived.CheckedChanged += chkQrReceived_CheckedChanged;
            // 
            // bodyViewport
            // 
            bodyViewport.AutoScroll = true;
            bodyViewport.BackColor = Color.White;
            bodyViewport.Controls.Add(content);
            bodyViewport.Dock = DockStyle.Fill;
            bodyViewport.Location = new Point(0, 56);
            bodyViewport.Name = "bodyViewport";
            bodyViewport.Size = new Size(560, 552);
            bodyViewport.TabIndex = 1;
            bodyViewport.ClientSizeChanged += bodyViewport_ClientSizeChanged;
            // 
            // content
            // 
            content.BackColor = Color.White;
            content.ColumnCount = 1;
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            content.Controls.Add(summary, 0, 0);
            content.Controls.Add(discountRow, 0, 1);
            content.Controls.Add(methodRow, 0, 2);
            content.Controls.Add(panelHost, 0, 3);
            content.Dock = DockStyle.Top;
            content.Location = new Point(0, 0);
            content.Margin = new Padding(0);
            content.Name = "content";
            content.Padding = new Padding(24, 16, 24, 12);
            content.RowCount = 4;
            content.RowStyles.Add(new RowStyle(SizeType.Absolute, 140F));
            content.RowStyles.Add(new RowStyle(SizeType.Absolute, 66F));
            content.RowStyles.Add(new RowStyle(SizeType.Absolute, 78F));
            content.RowStyles.Add(new RowStyle(SizeType.Absolute, 360F));
            content.Size = new Size(543, 672);
            content.TabIndex = 0;
            // 
            // summary
            // 
            summary.BackColor = Color.White;
            summary.Controls.Add(lblTotalCaption);
            summary.Controls.Add(lblFinal);
            summary.Controls.Add(lblTotal);
            summary.Controls.Add(lblDiscountCaption);
            summary.Controls.Add(lblDiscount);
            summary.Dock = DockStyle.Fill;
            summary.Location = new Point(27, 19);
            summary.Name = "summary";
            summary.Size = new Size(489, 134);
            summary.TabIndex = 0;
            // 
            // lblTotalCaption
            // 
            lblTotalCaption.AutoSize = true;
            lblTotalCaption.Font = new Font("Segoe UI", 9F);
            lblTotalCaption.ForeColor = Color.FromArgb(107, 114, 128);
            lblTotalCaption.Location = new Point(0, 2);
            lblTotalCaption.Name = "lblTotalCaption";
            lblTotalCaption.Size = new Size(145, 15);
            lblTotalCaption.TabIndex = 0;
            lblTotalCaption.Text = "TỔNG CẦN THANH TOÁN";
            // 
            // lblFinal
            // 
            lblFinal.AutoSize = true;
            lblFinal.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblFinal.ForeColor = Color.FromArgb(22, 163, 74);
            lblFinal.Location = new Point(0, 20);
            lblFinal.Name = "lblFinal";
            lblFinal.Size = new Size(67, 45);
            lblFinal.TabIndex = 1;
            lblFinal.Text = "0 đ";
            // 
            // lblTotal
            // 
            lblTotal.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblTotal.AutoSize = true;
            lblTotal.Font = new Font("Segoe UI", 10F);
            lblTotal.ForeColor = Color.FromArgb(31, 41, 55);
            lblTotal.Location = new Point(331, 32);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(90, 19);
            lblTotal.TabIndex = 2;
            lblTotal.Text = "Tạm tính: 0 đ";
            // 
            // lblDiscountCaption
            // 
            lblDiscountCaption.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblDiscountCaption.AutoSize = true;
            lblDiscountCaption.Font = new Font("Segoe UI", 10F);
            lblDiscountCaption.ForeColor = Color.FromArgb(31, 41, 55);
            lblDiscountCaption.Location = new Point(331, 59);
            lblDiscountCaption.Name = "lblDiscountCaption";
            lblDiscountCaption.Size = new Size(66, 19);
            lblDiscountCaption.TabIndex = 3;
            lblDiscountCaption.Text = "Giảm giá:";
            // 
            // lblDiscount
            // 
            lblDiscount.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblDiscount.AutoSize = true;
            lblDiscount.Font = new Font("Segoe UI", 10F);
            lblDiscount.ForeColor = Color.FromArgb(220, 38, 38);
            lblDiscount.Location = new Point(463, 59);
            lblDiscount.Name = "lblDiscount";
            lblDiscount.Size = new Size(29, 19);
            lblDiscount.TabIndex = 4;
            lblDiscount.Text = "0 đ";
            // 
            // discountRow
            // 
            discountRow.BackColor = Color.White;
            discountRow.Controls.Add(lblDiscountInput);
            discountRow.Controls.Add(txtDiscount);
            discountRow.Controls.Add(btnDiscountPercent);
            discountRow.Controls.Add(btnDiscountAmount);
            discountRow.Dock = DockStyle.Fill;
            discountRow.Location = new Point(27, 159);
            discountRow.Name = "discountRow";
            discountRow.Size = new Size(489, 60);
            discountRow.TabIndex = 1;
            // 
            // lblDiscountInput
            // 
            lblDiscountInput.AutoSize = true;
            lblDiscountInput.Font = new Font("Segoe UI", 10F);
            lblDiscountInput.ForeColor = Color.FromArgb(31, 41, 55);
            lblDiscountInput.Location = new Point(0, 11);
            lblDiscountInput.Name = "lblDiscountInput";
            lblDiscountInput.Size = new Size(63, 19);
            lblDiscountInput.TabIndex = 0;
            lblDiscountInput.Text = "Giảm giá";
            // 
            // txtDiscount
            // 
            txtDiscount.BorderStyle = BorderStyle.FixedSingle;
            txtDiscount.Font = new Font("Segoe UI", 10F);
            txtDiscount.Location = new Point(100, 5);
            txtDiscount.Name = "txtDiscount";
            txtDiscount.Size = new Size(150, 25);
            txtDiscount.TabIndex = 1;
            txtDiscount.TextAlign = HorizontalAlignment.Right;
            txtDiscount.TextChanged += DiscountTextChanged;
            txtDiscount.Leave += txtDiscount_Leave;
            // 
            // btnDiscountPercent
            // 
            btnDiscountPercent.BackColor = Color.FromArgb(243, 244, 246);
            btnDiscountPercent.FlatAppearance.BorderColor = Color.FromArgb(229, 231, 235);
            btnDiscountPercent.FlatStyle = FlatStyle.Flat;
            btnDiscountPercent.Font = new Font("Segoe UI", 10F);
            btnDiscountPercent.ForeColor = Color.FromArgb(107, 114, 128);
            btnDiscountPercent.Location = new Point(260, 4);
            btnDiscountPercent.Name = "btnDiscountPercent";
            btnDiscountPercent.Size = new Size(54, 34);
            btnDiscountPercent.TabIndex = 2;
            btnDiscountPercent.Text = "%";
            btnDiscountPercent.UseVisualStyleBackColor = false;
            btnDiscountPercent.Click += btnDiscountPercent_Click;
            // 
            // btnDiscountAmount
            // 
            btnDiscountAmount.BackColor = Color.FromArgb(243, 244, 246);
            btnDiscountAmount.FlatAppearance.BorderColor = Color.FromArgb(229, 231, 235);
            btnDiscountAmount.FlatStyle = FlatStyle.Flat;
            btnDiscountAmount.Font = new Font("Segoe UI", 10F);
            btnDiscountAmount.ForeColor = Color.FromArgb(107, 114, 128);
            btnDiscountAmount.Location = new Point(318, 4);
            btnDiscountAmount.Name = "btnDiscountAmount";
            btnDiscountAmount.Size = new Size(54, 34);
            btnDiscountAmount.TabIndex = 3;
            btnDiscountAmount.Text = "đ";
            btnDiscountAmount.UseVisualStyleBackColor = false;
            btnDiscountAmount.Click += btnDiscountAmount_Click;
            // 
            // methodRow
            // 
            methodRow.BackColor = Color.White;
            methodRow.Controls.Add(btnMethodCash);
            methodRow.Controls.Add(btnMethodTransfer);
            methodRow.Controls.Add(btnMethodCard);
            methodRow.Dock = DockStyle.Fill;
            methodRow.Location = new Point(24, 222);
            methodRow.Margin = new Padding(0);
            methodRow.Name = "methodRow";
            methodRow.Size = new Size(495, 78);
            methodRow.TabIndex = 2;
            methodRow.WrapContents = false;
            // 
            // btnMethodCash
            // 
            btnMethodCash.BackColor = Color.White;
            btnMethodCash.FlatAppearance.BorderColor = Color.FromArgb(229, 231, 235);
            btnMethodCash.FlatStyle = FlatStyle.Flat;
            btnMethodCash.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnMethodCash.ForeColor = Color.FromArgb(31, 41, 55);
            btnMethodCash.Location = new Point(0, 0);
            btnMethodCash.Margin = new Padding(0, 0, 8, 0);
            btnMethodCash.Name = "btnMethodCash";
            btnMethodCash.Size = new Size(120, 68);
            btnMethodCash.TabIndex = 0;
            btnMethodCash.Tag = 0;
            btnMethodCash.Text = "₫\nTiền mặt";
            btnMethodCash.UseVisualStyleBackColor = false;
            btnMethodCash.Click += methodButton_Click;
            // 
            // btnMethodTransfer
            // 
            btnMethodTransfer.BackColor = Color.White;
            btnMethodTransfer.FlatAppearance.BorderColor = Color.FromArgb(229, 231, 235);
            btnMethodTransfer.FlatStyle = FlatStyle.Flat;
            btnMethodTransfer.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnMethodTransfer.ForeColor = Color.FromArgb(31, 41, 55);
            btnMethodTransfer.Location = new Point(128, 0);
            btnMethodTransfer.Margin = new Padding(0, 0, 8, 0);
            btnMethodTransfer.Name = "btnMethodTransfer";
            btnMethodTransfer.Size = new Size(120, 68);
            btnMethodTransfer.TabIndex = 1;
            btnMethodTransfer.Tag = 1;
            btnMethodTransfer.Text = "▦\r\nChuyển khoản\r\nQR";
            btnMethodTransfer.UseVisualStyleBackColor = false;
            btnMethodTransfer.Click += methodButton_Click;
            // 
            // btnMethodCard
            // 
            btnMethodCard.BackColor = Color.White;
            btnMethodCard.FlatAppearance.BorderColor = Color.FromArgb(229, 231, 235);
            btnMethodCard.FlatStyle = FlatStyle.Flat;
            btnMethodCard.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnMethodCard.ForeColor = Color.FromArgb(31, 41, 55);
            btnMethodCard.Location = new Point(256, 0);
            btnMethodCard.Margin = new Padding(0, 0, 8, 0);
            btnMethodCard.Name = "btnMethodCard";
            btnMethodCard.Size = new Size(120, 68);
            btnMethodCard.TabIndex = 2;
            btnMethodCard.Tag = 2;
            btnMethodCard.Text = "▰\nThẻ";
            btnMethodCard.UseVisualStyleBackColor = false;
            btnMethodCard.Click += methodButton_Click;
            // 
            // panelHost
            // 
            panelHost.BackColor = Color.White;
            panelHost.Controls.Add(pnlCash);
            panelHost.Controls.Add(pnlTransfer);
            panelHost.Controls.Add(pnlCard);
            panelHost.Dock = DockStyle.Fill;
            panelHost.Location = new Point(27, 303);
            panelHost.Name = "panelHost";
            panelHost.Padding = new Padding(0, 4, 0, 0);
            panelHost.Size = new Size(489, 354);
            panelHost.TabIndex = 3;
            // 
            // pnlCash
            // 
            pnlCash.BackColor = Color.White;
            pnlCash.Controls.Add(lblCashLabel);
            pnlCash.Controls.Add(txtCash);
            pnlCash.Controls.Add(lblQuickCaption);
            pnlCash.Controls.Add(cashQuickPanel);
            pnlCash.Controls.Add(lblChangeLabel);
            pnlCash.Controls.Add(lblChange);
            pnlCash.Dock = DockStyle.Fill;
            pnlCash.Location = new Point(0, 4);
            pnlCash.Name = "pnlCash";
            pnlCash.Size = new Size(489, 350);
            pnlCash.TabIndex = 0;
            // 
            // lblCashLabel
            // 
            lblCashLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblCashLabel.ForeColor = Color.FromArgb(31, 41, 55);
            lblCashLabel.Location = new Point(18, 18);
            lblCashLabel.Name = "lblCashLabel";
            lblCashLabel.Size = new Size(250, 19);
            lblCashLabel.TabIndex = 0;
            lblCashLabel.Text = "Tiền khách đưa";
            // 
            // txtCash
            // 
            txtCash.BorderStyle = BorderStyle.FixedSingle;
            txtCash.Font = new Font("Segoe UI", 10F);
            txtCash.Location = new Point(18, 42);
            txtCash.Name = "txtCash";
            txtCash.Size = new Size(250, 25);
            txtCash.TabIndex = 1;
            txtCash.TextAlign = HorizontalAlignment.Right;
            txtCash.TextChanged += CashTextChanged;
            txtCash.Leave += txtCash_Leave;
            // 
            // lblQuickCaption
            // 
            lblQuickCaption.AutoSize = true;
            lblQuickCaption.Font = new Font("Segoe UI", 9F);
            lblQuickCaption.ForeColor = Color.FromArgb(107, 114, 128);
            lblQuickCaption.Location = new Point(18, 132);
            lblQuickCaption.Name = "lblQuickCaption";
            lblQuickCaption.Size = new Size(74, 15);
            lblQuickCaption.TabIndex = 3;
            lblQuickCaption.Text = "Gợi ý nhanh:";
            // 
            // cashQuickPanel
            // 
            cashQuickPanel.Controls.Add(btnCash50);
            cashQuickPanel.Controls.Add(btnCash100);
            cashQuickPanel.Controls.Add(btnCash200);
            cashQuickPanel.Controls.Add(btnCash500);
            cashQuickPanel.Controls.Add(btnCashExact);
            cashQuickPanel.Location = new Point(18, 88);
            cashQuickPanel.Name = "cashQuickPanel";
            cashQuickPanel.Size = new Size(376, 34);
            cashQuickPanel.TabIndex = 4;
            // 
            // btnCash50
            // 
            btnCash50.BackColor = Color.FromArgb(249, 250, 251);
            btnCash50.FlatAppearance.BorderSize = 0;
            btnCash50.FlatStyle = FlatStyle.Flat;
            btnCash50.Font = new Font("Segoe UI", 10F);
            btnCash50.ForeColor = Color.FromArgb(31, 41, 55);
            btnCash50.Location = new Point(0, 0);
            btnCash50.Name = "btnCash50";
            btnCash50.Size = new Size(48, 30);
            btnCash50.TabIndex = 0;
            btnCash50.Tag = new decimal(new int[] { 50000, 0, 0, 0 });
            btnCash50.Text = "50k";
            btnCash50.UseVisualStyleBackColor = false;
            btnCash50.Click += cashQuickButton_Click;
            // 
            // btnCash100
            // 
            btnCash100.BackColor = Color.FromArgb(249, 250, 251);
            btnCash100.FlatAppearance.BorderSize = 0;
            btnCash100.FlatStyle = FlatStyle.Flat;
            btnCash100.Font = new Font("Segoe UI", 10F);
            btnCash100.ForeColor = Color.FromArgb(31, 41, 55);
            btnCash100.Location = new Point(52, 0);
            btnCash100.Name = "btnCash100";
            btnCash100.Size = new Size(48, 30);
            btnCash100.TabIndex = 1;
            btnCash100.Tag = new decimal(new int[] { 100000, 0, 0, 0 });
            btnCash100.Text = "100k";
            btnCash100.UseVisualStyleBackColor = false;
            btnCash100.Click += cashQuickButton_Click;
            // 
            // btnCash200
            // 
            btnCash200.BackColor = Color.FromArgb(249, 250, 251);
            btnCash200.FlatAppearance.BorderSize = 0;
            btnCash200.FlatStyle = FlatStyle.Flat;
            btnCash200.Font = new Font("Segoe UI", 10F);
            btnCash200.ForeColor = Color.FromArgb(31, 41, 55);
            btnCash200.Location = new Point(104, 0);
            btnCash200.Name = "btnCash200";
            btnCash200.Size = new Size(48, 30);
            btnCash200.TabIndex = 2;
            btnCash200.Tag = new decimal(new int[] { 200000, 0, 0, 0 });
            btnCash200.Text = "200k";
            btnCash200.UseVisualStyleBackColor = false;
            btnCash200.Click += cashQuickButton_Click;
            // 
            // btnCash500
            // 
            btnCash500.BackColor = Color.FromArgb(249, 250, 251);
            btnCash500.FlatAppearance.BorderSize = 0;
            btnCash500.FlatStyle = FlatStyle.Flat;
            btnCash500.Font = new Font("Segoe UI", 10F);
            btnCash500.ForeColor = Color.FromArgb(31, 41, 55);
            btnCash500.Location = new Point(156, 0);
            btnCash500.Name = "btnCash500";
            btnCash500.Size = new Size(48, 30);
            btnCash500.TabIndex = 3;
            btnCash500.Tag = new decimal(new int[] { 500000, 0, 0, 0 });
            btnCash500.Text = "500k";
            btnCash500.UseVisualStyleBackColor = false;
            btnCash500.Click += cashQuickButton_Click;
            // 
            // btnCashExact
            // 
            btnCashExact.BackColor = Color.FromArgb(219, 234, 254);
            btnCashExact.FlatAppearance.BorderSize = 0;
            btnCashExact.FlatStyle = FlatStyle.Flat;
            btnCashExact.Font = new Font("Segoe UI", 10F);
            btnCashExact.ForeColor = Color.FromArgb(37, 99, 235);
            btnCashExact.Location = new Point(208, 0);
            btnCashExact.Name = "btnCashExact";
            btnCashExact.Size = new Size(88, 30);
            btnCashExact.TabIndex = 4;
            btnCashExact.Text = "Vừa đủ";
            btnCashExact.UseVisualStyleBackColor = false;
            btnCashExact.Click += btnCashExact_Click;
            // 
            // lblChangeLabel
            // 
            lblChangeLabel.Font = new Font("Segoe UI", 10F);
            lblChangeLabel.ForeColor = Color.FromArgb(107, 114, 128);
            lblChangeLabel.Location = new Point(300, 42);
            lblChangeLabel.Name = "lblChangeLabel";
            lblChangeLabel.Size = new Size(140, 19);
            lblChangeLabel.TabIndex = 5;
            lblChangeLabel.Text = "Tiền thừa";
            lblChangeLabel.Visible = false;
            // 
            // lblChange
            // 
            lblChange.AutoSize = true;
            lblChange.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            lblChange.ForeColor = Color.FromArgb(22, 163, 74);
            lblChange.Location = new Point(300, 62);
            lblChange.Name = "lblChange";
            lblChange.Size = new Size(43, 28);
            lblChange.TabIndex = 6;
            lblChange.Text = "0 đ";
            // 
            // pnlTransfer
            // 
            pnlTransfer.AutoScroll = true;
            pnlTransfer.BackColor = Color.White;
            pnlTransfer.Controls.Add(picQr);
            pnlTransfer.Controls.Add(lblQrStatus);
            pnlTransfer.Controls.Add(lblQrCountdown);
            pnlTransfer.Controls.Add(lblQrBankCaption);
            pnlTransfer.Controls.Add(lblQrBankValue);
            pnlTransfer.Controls.Add(btnCopyBank);
            pnlTransfer.Controls.Add(lblQrAccountCaption);
            pnlTransfer.Controls.Add(lblQrAccountValue);
            pnlTransfer.Controls.Add(btnCopyAccount);
            pnlTransfer.Controls.Add(lblQrOwnerCaption);
            pnlTransfer.Controls.Add(lblQrOwnerValue);
            pnlTransfer.Controls.Add(btnCopyOwner);
            pnlTransfer.Controls.Add(lblQrAmountCaption);
            pnlTransfer.Controls.Add(lblQrAmountValue);
            pnlTransfer.Controls.Add(btnCopyAmount);
            pnlTransfer.Controls.Add(lblQrContentCaption);
            pnlTransfer.Controls.Add(lblQrContentValue);
            pnlTransfer.Controls.Add(btnCopyContent);
            pnlTransfer.Controls.Add(btnRefreshQr);
            pnlTransfer.Controls.Add(lblQrNote);
            pnlTransfer.Dock = DockStyle.Fill;
            pnlTransfer.Location = new Point(0, 4);
            pnlTransfer.Name = "pnlTransfer";
            pnlTransfer.Size = new Size(489, 350);
            pnlTransfer.TabIndex = 1;
            pnlTransfer.Visible = false;
            // 
            // picQr
            // 
            picQr.BackColor = Color.White;
            picQr.BorderStyle = BorderStyle.FixedSingle;
            picQr.Location = new Point(130, 10);
            picQr.Name = "picQr";
            picQr.Size = new Size(240, 240);
            picQr.SizeMode = PictureBoxSizeMode.Zoom;
            picQr.TabIndex = 0;
            picQr.TabStop = false;
            // 
            // lblQrStatus
            // 
            lblQrStatus.Font = new Font("Segoe UI", 10F);
            lblQrStatus.ForeColor = Color.FromArgb(107, 114, 128);
            lblQrStatus.Location = new Point(10, 254);
            lblQrStatus.Name = "lblQrStatus";
            lblQrStatus.Size = new Size(492, 24);
            lblQrStatus.TabIndex = 1;
            lblQrStatus.Text = "Đang chờ tạo mã QR...";
            lblQrStatus.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblQrCountdown
            // 
            lblQrCountdown.Font = new Font("Segoe UI", 9F);
            lblQrCountdown.ForeColor = Color.FromArgb(107, 114, 128);
            lblQrCountdown.Location = new Point(10, 278);
            lblQrCountdown.Name = "lblQrCountdown";
            lblQrCountdown.Size = new Size(492, 20);
            lblQrCountdown.TabIndex = 2;
            lblQrCountdown.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblQrBankCaption
            // 
            lblQrBankCaption.Font = new Font("Segoe UI", 9F);
            lblQrBankCaption.ForeColor = Color.FromArgb(107, 114, 128);
            lblQrBankCaption.Location = new Point(14, 313);
            lblQrBankCaption.Name = "lblQrBankCaption";
            lblQrBankCaption.Size = new Size(112, 22);
            lblQrBankCaption.TabIndex = 3;
            lblQrBankCaption.Text = "Ngân hàng";
            lblQrBankCaption.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblQrBankValue
            // 
            lblQrBankValue.AutoEllipsis = true;
            lblQrBankValue.Font = new Font("Segoe UI", 10F);
            lblQrBankValue.ForeColor = Color.FromArgb(31, 41, 55);
            lblQrBankValue.Location = new Point(128, 311);
            lblQrBankValue.Name = "lblQrBankValue";
            lblQrBankValue.Size = new Size(300, 24);
            lblQrBankValue.TabIndex = 4;
            lblQrBankValue.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnCopyBank
            // 
            btnCopyBank.BackColor = Color.FromArgb(249, 250, 251);
            btnCopyBank.FlatAppearance.BorderSize = 0;
            btnCopyBank.FlatStyle = FlatStyle.Flat;
            btnCopyBank.Font = new Font("Segoe UI", 9F);
            btnCopyBank.ForeColor = Color.FromArgb(31, 41, 55);
            btnCopyBank.Location = new Point(428, 308);
            btnCopyBank.Name = "btnCopyBank";
            btnCopyBank.Size = new Size(76, 28);
            btnCopyBank.TabIndex = 5;
            btnCopyBank.Tag = lblQrBankValue;
            btnCopyBank.Text = "Sao chép";
            btnCopyBank.UseVisualStyleBackColor = false;
            btnCopyBank.Click += copyQrValue_Click;
            // 
            // lblQrAccountCaption
            // 
            lblQrAccountCaption.Font = new Font("Segoe UI", 9F);
            lblQrAccountCaption.ForeColor = Color.FromArgb(107, 114, 128);
            lblQrAccountCaption.Location = new Point(14, 349);
            lblQrAccountCaption.Name = "lblQrAccountCaption";
            lblQrAccountCaption.Size = new Size(112, 22);
            lblQrAccountCaption.TabIndex = 4;
            lblQrAccountCaption.Text = "Số tài khoản";
            lblQrAccountCaption.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblQrAccountValue
            // 
            lblQrAccountValue.AutoEllipsis = true;
            lblQrAccountValue.Font = new Font("Segoe UI", 10F);
            lblQrAccountValue.ForeColor = Color.FromArgb(31, 41, 55);
            lblQrAccountValue.Location = new Point(128, 347);
            lblQrAccountValue.Name = "lblQrAccountValue";
            lblQrAccountValue.Size = new Size(300, 24);
            lblQrAccountValue.TabIndex = 5;
            lblQrAccountValue.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnCopyAccount
            // 
            btnCopyAccount.BackColor = Color.FromArgb(249, 250, 251);
            btnCopyAccount.FlatAppearance.BorderSize = 0;
            btnCopyAccount.FlatStyle = FlatStyle.Flat;
            btnCopyAccount.Font = new Font("Segoe UI", 9F);
            btnCopyAccount.ForeColor = Color.FromArgb(31, 41, 55);
            btnCopyAccount.Location = new Point(428, 344);
            btnCopyAccount.Name = "btnCopyAccount";
            btnCopyAccount.Size = new Size(76, 28);
            btnCopyAccount.TabIndex = 6;
            btnCopyAccount.Tag = lblQrAccountValue;
            btnCopyAccount.Text = "Sao chép";
            btnCopyAccount.UseVisualStyleBackColor = false;
            btnCopyAccount.Click += copyQrValue_Click;
            // 
            // lblQrOwnerCaption
            // 
            lblQrOwnerCaption.Font = new Font("Segoe UI", 9F);
            lblQrOwnerCaption.ForeColor = Color.FromArgb(107, 114, 128);
            lblQrOwnerCaption.Location = new Point(14, 385);
            lblQrOwnerCaption.Name = "lblQrOwnerCaption";
            lblQrOwnerCaption.Size = new Size(112, 22);
            lblQrOwnerCaption.TabIndex = 5;
            lblQrOwnerCaption.Text = "Chủ tài khoản";
            lblQrOwnerCaption.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblQrOwnerValue
            // 
            lblQrOwnerValue.AutoEllipsis = true;
            lblQrOwnerValue.Font = new Font("Segoe UI", 10F);
            lblQrOwnerValue.ForeColor = Color.FromArgb(31, 41, 55);
            lblQrOwnerValue.Location = new Point(128, 383);
            lblQrOwnerValue.Name = "lblQrOwnerValue";
            lblQrOwnerValue.Size = new Size(300, 24);
            lblQrOwnerValue.TabIndex = 6;
            lblQrOwnerValue.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnCopyOwner
            // 
            btnCopyOwner.BackColor = Color.FromArgb(249, 250, 251);
            btnCopyOwner.FlatAppearance.BorderSize = 0;
            btnCopyOwner.FlatStyle = FlatStyle.Flat;
            btnCopyOwner.Font = new Font("Segoe UI", 9F);
            btnCopyOwner.ForeColor = Color.FromArgb(31, 41, 55);
            btnCopyOwner.Location = new Point(428, 380);
            btnCopyOwner.Name = "btnCopyOwner";
            btnCopyOwner.Size = new Size(76, 28);
            btnCopyOwner.TabIndex = 7;
            btnCopyOwner.Tag = lblQrOwnerValue;
            btnCopyOwner.Text = "Sao chép";
            btnCopyOwner.UseVisualStyleBackColor = false;
            btnCopyOwner.Click += copyQrValue_Click;
            // 
            // lblQrAmountCaption
            // 
            lblQrAmountCaption.Font = new Font("Segoe UI", 9F);
            lblQrAmountCaption.ForeColor = Color.FromArgb(107, 114, 128);
            lblQrAmountCaption.Location = new Point(14, 421);
            lblQrAmountCaption.Name = "lblQrAmountCaption";
            lblQrAmountCaption.Size = new Size(112, 22);
            lblQrAmountCaption.TabIndex = 6;
            lblQrAmountCaption.Text = "Số tiền";
            lblQrAmountCaption.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblQrAmountValue
            // 
            lblQrAmountValue.AutoEllipsis = true;
            lblQrAmountValue.Font = new Font("Segoe UI", 10F);
            lblQrAmountValue.ForeColor = Color.FromArgb(31, 41, 55);
            lblQrAmountValue.Location = new Point(128, 419);
            lblQrAmountValue.Name = "lblQrAmountValue";
            lblQrAmountValue.Size = new Size(300, 24);
            lblQrAmountValue.TabIndex = 7;
            lblQrAmountValue.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnCopyAmount
            // 
            btnCopyAmount.BackColor = Color.FromArgb(249, 250, 251);
            btnCopyAmount.FlatAppearance.BorderSize = 0;
            btnCopyAmount.FlatStyle = FlatStyle.Flat;
            btnCopyAmount.Font = new Font("Segoe UI", 9F);
            btnCopyAmount.ForeColor = Color.FromArgb(31, 41, 55);
            btnCopyAmount.Location = new Point(428, 416);
            btnCopyAmount.Name = "btnCopyAmount";
            btnCopyAmount.Size = new Size(76, 28);
            btnCopyAmount.TabIndex = 8;
            btnCopyAmount.Tag = lblQrAmountValue;
            btnCopyAmount.Text = "Sao chép";
            btnCopyAmount.UseVisualStyleBackColor = false;
            btnCopyAmount.Click += copyQrValue_Click;
            // 
            // lblQrContentCaption
            // 
            lblQrContentCaption.Font = new Font("Segoe UI", 9F);
            lblQrContentCaption.ForeColor = Color.FromArgb(107, 114, 128);
            lblQrContentCaption.Location = new Point(14, 457);
            lblQrContentCaption.Name = "lblQrContentCaption";
            lblQrContentCaption.Size = new Size(112, 22);
            lblQrContentCaption.TabIndex = 7;
            lblQrContentCaption.Text = "Nội dung";
            lblQrContentCaption.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblQrContentValue
            // 
            lblQrContentValue.AutoEllipsis = true;
            lblQrContentValue.Font = new Font("Segoe UI", 10F);
            lblQrContentValue.ForeColor = Color.FromArgb(31, 41, 55);
            lblQrContentValue.Location = new Point(128, 455);
            lblQrContentValue.Name = "lblQrContentValue";
            lblQrContentValue.Size = new Size(300, 24);
            lblQrContentValue.TabIndex = 8;
            lblQrContentValue.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnCopyContent
            // 
            btnCopyContent.BackColor = Color.FromArgb(249, 250, 251);
            btnCopyContent.FlatAppearance.BorderSize = 0;
            btnCopyContent.FlatStyle = FlatStyle.Flat;
            btnCopyContent.Font = new Font("Segoe UI", 9F);
            btnCopyContent.ForeColor = Color.FromArgb(31, 41, 55);
            btnCopyContent.Location = new Point(428, 452);
            btnCopyContent.Name = "btnCopyContent";
            btnCopyContent.Size = new Size(76, 28);
            btnCopyContent.TabIndex = 9;
            btnCopyContent.Tag = lblQrContentValue;
            btnCopyContent.Text = "Sao chép";
            btnCopyContent.UseVisualStyleBackColor = false;
            btnCopyContent.Click += copyQrValue_Click;
            // 
            // btnRefreshQr
            // 
            btnRefreshQr.BackColor = Color.FromArgb(219, 234, 254);
            btnRefreshQr.FlatAppearance.BorderSize = 0;
            btnRefreshQr.FlatStyle = FlatStyle.Flat;
            btnRefreshQr.Font = new Font("Segoe UI", 10F);
            btnRefreshQr.ForeColor = Color.FromArgb(37, 99, 235);
            btnRefreshQr.Location = new Point(190, 488);
            btnRefreshQr.Name = "btnRefreshQr";
            btnRefreshQr.Size = new Size(132, 34);
            btnRefreshQr.TabIndex = 8;
            btnRefreshQr.Text = "Làm mới QR";
            btnRefreshQr.UseVisualStyleBackColor = false;
            btnRefreshQr.Click += btnRefreshQr_Click;
            // 
            // lblQrNote
            // 
            lblQrNote.Font = new Font("Segoe UI", 9F);
            lblQrNote.ForeColor = Color.FromArgb(107, 114, 128);
            lblQrNote.Location = new Point(10, 526);
            lblQrNote.Name = "lblQrNote";
            lblQrNote.Size = new Size(492, 24);
            lblQrNote.TabIndex = 9;
            lblQrNote.Text = "QR chỉ dùng cho hóa đơn này. Thay đổi số tiền sẽ tạo mã QR mới.";
            lblQrNote.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlCard
            // 
            pnlCard.BackColor = Color.White;
            pnlCard.Controls.Add(lblCardIcon);
            pnlCard.Controls.Add(lblCardTitle);
            pnlCard.Controls.Add(lblCardInfo);
            pnlCard.Controls.Add(lblApprovalCaption);
            pnlCard.Controls.Add(txtCardApprovalCode);
            pnlCard.Dock = DockStyle.Fill;
            pnlCard.Location = new Point(0, 4);
            pnlCard.Name = "pnlCard";
            pnlCard.Size = new Size(489, 350);
            pnlCard.TabIndex = 2;
            pnlCard.Visible = false;
            // 
            // lblCardIcon
            // 
            lblCardIcon.BackColor = Color.FromArgb(243, 244, 246);
            lblCardIcon.Font = new Font("Segoe UI", 46F, FontStyle.Bold);
            lblCardIcon.ForeColor = Color.FromArgb(30, 58, 95);
            lblCardIcon.Location = new Point(22, 28);
            lblCardIcon.Name = "lblCardIcon";
            lblCardIcon.Size = new Size(112, 112);
            lblCardIcon.TabIndex = 0;
            lblCardIcon.Text = "▣";
            lblCardIcon.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblCardTitle
            // 
            lblCardTitle.AutoSize = true;
            lblCardTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblCardTitle.ForeColor = Color.FromArgb(31, 41, 55);
            lblCardTitle.Location = new Point(156, 34);
            lblCardTitle.Name = "lblCardTitle";
            lblCardTitle.Size = new Size(169, 21);
            lblCardTitle.TabIndex = 1;
            lblCardTitle.Text = "Thanh toán bằng thẻ";
            // 
            // lblCardInfo
            // 
            lblCardInfo.AutoSize = true;
            lblCardInfo.Font = new Font("Segoe UI", 10F);
            lblCardInfo.ForeColor = Color.FromArgb(107, 114, 128);
            lblCardInfo.Location = new Point(156, 66);
            lblCardInfo.Name = "lblCardInfo";
            lblCardInfo.Size = new Size(275, 38);
            lblCardInfo.TabIndex = 2;
            lblCardInfo.Text = "Thực hiện thanh toán trên thiết bị quẹt thẻ,\nsau đó xác nhận giao dịch thành công.";
            // 
            // lblApprovalCaption
            // 
            lblApprovalCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblApprovalCaption.ForeColor = Color.FromArgb(31, 41, 55);
            lblApprovalCaption.Location = new Point(156, 112);
            lblApprovalCaption.Name = "lblApprovalCaption";
            lblApprovalCaption.Size = new Size(270, 17);
            lblApprovalCaption.TabIndex = 3;
            lblApprovalCaption.Text = "Mã chuẩn chi*";
            // 
            // txtCardApprovalCode
            // 
            txtCardApprovalCode.BorderStyle = BorderStyle.FixedSingle;
            txtCardApprovalCode.CharacterCasing = CharacterCasing.Upper;
            txtCardApprovalCode.Font = new Font("Segoe UI", 10F);
            txtCardApprovalCode.Location = new Point(156, 136);
            txtCardApprovalCode.MaxLength = 12;
            txtCardApprovalCode.Name = "txtCardApprovalCode";
            txtCardApprovalCode.PlaceholderText = "Nhập mã chuẩn chi (4–12 ký tự)";
            txtCardApprovalCode.Size = new Size(270, 25);
            txtCardApprovalCode.TabIndex = 4;
            txtCardApprovalCode.TextChanged += txtCardApprovalCode_TextChanged;
            // 
            // cboDiscountMode
            // 
            cboDiscountMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cboDiscountMode.Items.AddRange(new object[] { "%", "đ" });
            cboDiscountMode.Location = new Point(0, 0);
            cboDiscountMode.Name = "cboDiscountMode";
            cboDiscountMode.Size = new Size(121, 23);
            cboDiscountMode.TabIndex = 4;
            cboDiscountMode.Visible = false;
            cboDiscountMode.SelectedIndexChanged += cboDiscountMode_SelectedIndexChanged;
            // 
            // numDiscount
            // 
            numDiscount.DecimalPlaces = 2;
            numDiscount.Location = new Point(0, 0);
            numDiscount.Name = "numDiscount";
            numDiscount.Size = new Size(120, 23);
            numDiscount.TabIndex = 5;
            numDiscount.Visible = false;
            numDiscount.ValueChanged += numDiscount_ValueChanged;
            // 
            // cboMethod
            // 
            cboMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            cboMethod.Items.AddRange(new object[] { "Tiền mặt", "Chuyển khoản QR", "Thẻ" });
            cboMethod.Location = new Point(0, 0);
            cboMethod.Name = "cboMethod";
            cboMethod.Size = new Size(121, 23);
            cboMethod.TabIndex = 0;
            cboMethod.Visible = false;
            cboMethod.SelectedIndexChanged += cboMethod_SelectedIndexChanged;
            // 
            // numCash
            // 
            numCash.Location = new Point(0, 0);
            numCash.Maximum = new decimal(new int[] { 1000000000, 0, 0, 0 });
            numCash.Name = "numCash";
            numCash.Size = new Size(120, 23);
            numCash.TabIndex = 2;
            numCash.Visible = false;
            numCash.ValueChanged += numCash_ValueChanged;
            // 
            // FormPayment
            // 
            AcceptButton = btnConfirm;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.White;
            CancelButton = btnCancel;
            ClientSize = new Size(560, 720);
            Controls.Add(bodyViewport);
            Controls.Add(footer);
            Controls.Add(pnlHead);
            Font = new Font("Segoe UI", 10F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(520, 640);
            Name = "FormPayment";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Thanh toán hóa đơn";
            KeyDown += PaymentForm_KeyDown;
            pnlHead.ResumeLayout(false);
            footer.ResumeLayout(false);
            footer.PerformLayout();
            bodyViewport.ResumeLayout(false);
            content.ResumeLayout(false);
            summary.ResumeLayout(false);
            summary.PerformLayout();
            discountRow.ResumeLayout(false);
            discountRow.PerformLayout();
            methodRow.ResumeLayout(false);
            panelHost.ResumeLayout(false);
            pnlCash.ResumeLayout(false);
            pnlCash.PerformLayout();
            cashQuickPanel.ResumeLayout(false);
            pnlTransfer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picQr).EndInit();
            pnlCard.ResumeLayout(false);
            pnlCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numDiscount).EndInit();
            ((System.ComponentModel.ISupportInitialize)numCash).EndInit();
            ResumeLayout(false);
        }

        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private Panel pnlHead = null!, footer = null!, bodyViewport = null!, summary = null!, discountRow = null!,
            panelHost = null!, pnlCash = null!, cashQuickPanel = null!, pnlTransfer = null!, pnlCard = null!;
        private TableLayoutPanel content = null!;
        private FlowLayoutPanel methodRow = null!;
        private Label lblHead = null!, lblConfirmHint = null!, lblTotalCaption = null!, lblFinal = null!, lblTotal = null!,
            lblDiscountCaption = null!, lblDiscount = null!, lblDiscountInput = null!, lblCashLabel = null!,
            lblQuickCaption = null!, lblChangeLabel = null!, lblChange = null!, lblQrStatus = null!, lblQrCountdown = null!,
            lblQrBankCaption = null!, lblQrBankValue = null!, lblQrAccountCaption = null!, lblQrAccountValue = null!,
            lblQrOwnerCaption = null!, lblQrOwnerValue = null!, lblQrAmountCaption = null!, lblQrAmountValue = null!,
            lblQrContentCaption = null!, lblQrContentValue = null!, lblQrNote = null!, lblCardIcon = null!,
            lblCardTitle = null!, lblCardInfo = null!, lblApprovalCaption = null!;
        private Button btnCancel = null!, btnConfirm = null!, btnDiscountPercent = null!, btnDiscountAmount = null!,
            btnMethodCash = null!, btnMethodTransfer = null!, btnMethodCard = null!, btnCash50 = null!, btnCash100 = null!,
            btnCash200 = null!, btnCash500 = null!, btnCashExact = null!, btnCopyBank = null!, btnCopyAccount = null!,
            btnCopyOwner = null!, btnCopyAmount = null!, btnCopyContent = null!, btnRefreshQr = null!;
        private CheckBox chkQrReceived = null!;
        private ComboBox cboDiscountMode = null!, cboMethod = null!;
        private NumericUpDown numDiscount = null!, numCash = null!;
        private TextBox txtDiscount = null!, txtCash = null!, txtCardApprovalCode = null!;
        private PictureBox picQr = null!;
    }
}
