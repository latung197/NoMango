namespace ChartRealtime
{
    partial class HistoryForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HistoryForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblTitleGrdHistory = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.pnlHeaderRight = new System.Windows.Forms.Panel();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.splMainContainer = new System.Windows.Forms.SplitContainer();
            this.btnClose = new System.Windows.Forms.Button();
            this.pnlHeaderLeft = new System.Windows.Forms.Panel();
            this.lblTitleMenu = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.pnlFilter = new System.Windows.Forms.Panel();
            this.txtToWarningPoint = new System.Windows.Forms.NumericUpDown();
            this.txtFromWarningPoint = new System.Windows.Forms.NumericUpDown();
            this.txtmM = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.lbltFrom = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.lbldTo = new System.Windows.Forms.Label();
            this.lbldFrom = new System.Windows.Forms.Label();
            this.btnFilterHistory = new System.Windows.Forms.Button();
            this.pnRight = new System.Windows.Forms.Panel();
            this.grdHistory = new System.Windows.Forms.DataGridView();
            this.警報日時 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.位置 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.鋳造長 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.初期凝固遅れ危険度 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.再溶解危険度 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.警報値 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel3 = new System.Windows.Forms.Panel();
            this.cboPageSize = new System.Windows.Forms.ComboBox();
            this.btnBack = new System.Windows.Forms.Button();
            this.lblPage = new System.Windows.Forms.Label();
            this.btnNext = new System.Windows.Forms.Button();
            this.pnlHeaderRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splMainContainer)).BeginInit();
            this.splMainContainer.Panel1.SuspendLayout();
            this.splMainContainer.Panel2.SuspendLayout();
            this.splMainContainer.SuspendLayout();
            this.pnlHeaderLeft.SuspendLayout();
            this.pnlFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtToWarningPoint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFromWarningPoint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtmM)).BeginInit();
            this.pnRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdHistory)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitleGrdHistory
            // 
            this.lblTitleGrdHistory.AutoSize = true;
            this.lblTitleGrdHistory.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitleGrdHistory.Location = new System.Drawing.Point(16, 3);
            this.lblTitleGrdHistory.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTitleGrdHistory.Name = "lblTitleGrdHistory";
            this.lblTitleGrdHistory.Size = new System.Drawing.Size(68, 17);
            this.lblTitleGrdHistory.TabIndex = 2;
            this.lblTitleGrdHistory.Text = "検索結果";
            // 
            // label39
            // 
            this.label39.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(139)))), ((int)(((byte)(0)))));
            this.label39.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label39.Location = new System.Drawing.Point(0, 0);
            this.label39.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(12, 25);
            this.label39.TabIndex = 1;
            // 
            // pnlHeaderRight
            // 
            this.pnlHeaderRight.BackColor = System.Drawing.Color.SlateGray;
            this.pnlHeaderRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlHeaderRight.Controls.Add(this.lblTitleGrdHistory);
            this.pnlHeaderRight.Controls.Add(this.label39);
            this.pnlHeaderRight.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeaderRight.Location = new System.Drawing.Point(0, 0);
            this.pnlHeaderRight.Margin = new System.Windows.Forms.Padding(2);
            this.pnlHeaderRight.Name = "pnlHeaderRight";
            this.pnlHeaderRight.Size = new System.Drawing.Size(1751, 25);
            this.pnlHeaderRight.TabIndex = 18;
            // 
            // dtpFrom
            // 
            this.dtpFrom.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFrom.Location = new System.Drawing.Point(4, 65);
            this.dtpFrom.Margin = new System.Windows.Forms.Padding(2);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(143, 20);
            this.dtpFrom.TabIndex = 7;
            // 
            // dtpTo
            // 
            this.dtpTo.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTo.Location = new System.Drawing.Point(4, 109);
            this.dtpTo.Margin = new System.Windows.Forms.Padding(2);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(143, 20);
            this.dtpTo.TabIndex = 8;
            // 
            // splMainContainer
            // 
            this.splMainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splMainContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splMainContainer.IsSplitterFixed = true;
            this.splMainContainer.Location = new System.Drawing.Point(0, 0);
            this.splMainContainer.Margin = new System.Windows.Forms.Padding(2);
            this.splMainContainer.Name = "splMainContainer";
            // 
            // splMainContainer.Panel1
            // 
            this.splMainContainer.Panel1.BackColor = System.Drawing.Color.DimGray;
            this.splMainContainer.Panel1.Controls.Add(this.btnClose);
            this.splMainContainer.Panel1.Controls.Add(this.pnlHeaderLeft);
            this.splMainContainer.Panel1.Controls.Add(this.pnlFilter);
            // 
            // splMainContainer.Panel2
            // 
            this.splMainContainer.Panel2.BackColor = System.Drawing.Color.White;
            this.splMainContainer.Panel2.Controls.Add(this.pnRight);
            this.splMainContainer.Panel2.Controls.Add(this.pnlHeaderRight);
            this.splMainContainer.Size = new System.Drawing.Size(1904, 862);
            this.splMainContainer.SplitterDistance = 150;
            this.splMainContainer.SplitterWidth = 3;
            this.splMainContainer.TabIndex = 12;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.Orange;
            this.btnClose.Font = new System.Drawing.Font("MS Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(12, 800);
            this.btnClose.Margin = new System.Windows.Forms.Padding(2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(126, 40);
            this.btnClose.TabIndex = 19;
            this.btnClose.Text = "戻る";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // pnlHeaderLeft
            // 
            this.pnlHeaderLeft.BackColor = System.Drawing.Color.SlateGray;
            this.pnlHeaderLeft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlHeaderLeft.Controls.Add(this.lblTitleMenu);
            this.pnlHeaderLeft.Controls.Add(this.label38);
            this.pnlHeaderLeft.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeaderLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlHeaderLeft.Margin = new System.Windows.Forms.Padding(2);
            this.pnlHeaderLeft.Name = "pnlHeaderLeft";
            this.pnlHeaderLeft.Size = new System.Drawing.Size(150, 25);
            this.pnlHeaderLeft.TabIndex = 18;
            // 
            // lblTitleMenu
            // 
            this.lblTitleMenu.AutoSize = true;
            this.lblTitleMenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitleMenu.Location = new System.Drawing.Point(16, 3);
            this.lblTitleMenu.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTitleMenu.Name = "lblTitleMenu";
            this.lblTitleMenu.Size = new System.Drawing.Size(68, 17);
            this.lblTitleMenu.TabIndex = 3;
            this.lblTitleMenu.Text = "検索条件";
            // 
            // label38
            // 
            this.label38.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(139)))), ((int)(((byte)(0)))));
            this.label38.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label38.Location = new System.Drawing.Point(0, 0);
            this.label38.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(12, 25);
            this.label38.TabIndex = 0;
            // 
            // pnlFilter
            // 
            this.pnlFilter.Controls.Add(this.txtToWarningPoint);
            this.pnlFilter.Controls.Add(this.txtFromWarningPoint);
            this.pnlFilter.Controls.Add(this.txtmM);
            this.pnlFilter.Controls.Add(this.label1);
            this.pnlFilter.Controls.Add(this.lbltFrom);
            this.pnlFilter.Controls.Add(this.label45);
            this.pnlFilter.Controls.Add(this.lbldTo);
            this.pnlFilter.Controls.Add(this.lbldFrom);
            this.pnlFilter.Controls.Add(this.dtpFrom);
            this.pnlFilter.Controls.Add(this.dtpTo);
            this.pnlFilter.Controls.Add(this.btnFilterHistory);
            this.pnlFilter.Location = new System.Drawing.Point(2, 25);
            this.pnlFilter.Margin = new System.Windows.Forms.Padding(2);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Size = new System.Drawing.Size(150, 501);
            this.pnlFilter.TabIndex = 17;
            // 
            // txtToWarningPoint
            // 
            this.txtToWarningPoint.DecimalPlaces = 2;
            this.txtToWarningPoint.InterceptArrowKeys = false;
            this.txtToWarningPoint.Location = new System.Drawing.Point(4, 243);
            this.txtToWarningPoint.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.txtToWarningPoint.Name = "txtToWarningPoint";
            this.txtToWarningPoint.Size = new System.Drawing.Size(141, 20);
            this.txtToWarningPoint.TabIndex = 31;
            this.txtToWarningPoint.ThousandsSeparator = true;
            // 
            // txtFromWarningPoint
            // 
            this.txtFromWarningPoint.DecimalPlaces = 2;
            this.txtFromWarningPoint.InterceptArrowKeys = false;
            this.txtFromWarningPoint.Location = new System.Drawing.Point(4, 199);
            this.txtFromWarningPoint.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.txtFromWarningPoint.Name = "txtFromWarningPoint";
            this.txtFromWarningPoint.Size = new System.Drawing.Size(141, 20);
            this.txtFromWarningPoint.TabIndex = 30;
            this.txtFromWarningPoint.ThousandsSeparator = true;
            // 
            // txtmM
            // 
            this.txtmM.DecimalPlaces = 2;
            this.txtmM.InterceptArrowKeys = false;
            this.txtmM.Location = new System.Drawing.Point(4, 157);
            this.txtmM.Maximum = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.txtmM.Name = "txtmM";
            this.txtmM.Size = new System.Drawing.Size(141, 20);
            this.txtmM.TabIndex = 29;
            this.txtmM.ThousandsSeparator = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label1.Location = new System.Drawing.Point(4, 223);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 17);
            this.label1.TabIndex = 26;
            this.label1.Text = "警報値（TO)";
            // 
            // lbltFrom
            // 
            this.lbltFrom.AutoSize = true;
            this.lbltFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbltFrom.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lbltFrom.Location = new System.Drawing.Point(4, 179);
            this.lbltFrom.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbltFrom.Name = "lbltFrom";
            this.lbltFrom.Size = new System.Drawing.Size(104, 17);
            this.lbltFrom.TabIndex = 25;
            this.lbltFrom.Text = "警報値（FROM）";
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label45.Location = new System.Drawing.Point(4, 136);
            this.label45.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(71, 17);
            this.label45.TabIndex = 21;
            this.label45.Text = "鋳造長(m)";
            // 
            // lbldTo
            // 
            this.lbldTo.AutoSize = true;
            this.lbldTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbldTo.Location = new System.Drawing.Point(4, 87);
            this.lbldTo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbldTo.Name = "lbldTo";
            this.lbldTo.Size = new System.Drawing.Size(96, 17);
            this.lbldTo.TabIndex = 20;
            this.lbldTo.Text = "警報日時（TO)";
            // 
            // lbldFrom
            // 
            this.lbldFrom.AutoSize = true;
            this.lbldFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbldFrom.Location = new System.Drawing.Point(4, 43);
            this.lbldFrom.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbldFrom.Name = "lbldFrom";
            this.lbldFrom.Size = new System.Drawing.Size(116, 17);
            this.lbldFrom.TabIndex = 19;
            this.lbldFrom.Text = "警報日時（FROM)";
            // 
            // btnFilterHistory
            // 
            this.btnFilterHistory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFilterHistory.AutoSize = true;
            this.btnFilterHistory.BackColor = System.Drawing.Color.Orange;
            this.btnFilterHistory.Font = new System.Drawing.Font("MS Gothic", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFilterHistory.Image = ((System.Drawing.Image)(resources.GetObject("btnFilterHistory.Image")));
            this.btnFilterHistory.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFilterHistory.Location = new System.Drawing.Point(17, 296);
            this.btnFilterHistory.Margin = new System.Windows.Forms.Padding(2);
            this.btnFilterHistory.Name = "btnFilterHistory";
            this.btnFilterHistory.Size = new System.Drawing.Size(106, 40);
            this.btnFilterHistory.TabIndex = 0;
            this.btnFilterHistory.Text = " 表示";
            this.btnFilterHistory.UseVisualStyleBackColor = false;
            this.btnFilterHistory.Click += new System.EventHandler(this.btnFilterHistory_Click);
            // 
            // pnRight
            // 
            this.pnRight.Controls.Add(this.grdHistory);
            this.pnRight.Controls.Add(this.panel3);
            this.pnRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnRight.Location = new System.Drawing.Point(0, 25);
            this.pnRight.Name = "pnRight";
            this.pnRight.Size = new System.Drawing.Size(1751, 837);
            this.pnRight.TabIndex = 19;
            // 
            // grdHistory
            // 
            this.grdHistory.AllowUserToAddRows = false;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdHistory.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.grdHistory.BackgroundColor = System.Drawing.Color.Black;
            this.grdHistory.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.grdHistory.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.ControlLightLight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grdHistory.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.grdHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.警報日時,
            this.位置,
            this.鋳造長,
            this.初期凝固遅れ危険度,
            this.再溶解危険度,
            this.警報値});
            this.grdHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdHistory.GridColor = System.Drawing.Color.Black;
            this.grdHistory.Location = new System.Drawing.Point(0, 0);
            this.grdHistory.Name = "grdHistory";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.DarkGray;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grdHistory.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.grdHistory.RowHeadersVisible = false;
            this.grdHistory.RowHeadersWidth = 51;
            this.grdHistory.Size = new System.Drawing.Size(1751, 797);
            this.grdHistory.TabIndex = 0;
            // 
            // 警報日時
            // 
            this.警報日時.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.警報日時.DataPropertyName = "警報日時";
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.DarkGray;
            this.警報日時.DefaultCellStyle = dataGridViewCellStyle3;
            this.警報日時.HeaderText = "警報日時";
            this.警報日時.MinimumWidth = 6;
            this.警報日時.Name = "警報日時";
            this.警報日時.ReadOnly = true;
            // 
            // 位置
            // 
            this.位置.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.位置.DataPropertyName = "位置";
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.DarkGray;
            this.位置.DefaultCellStyle = dataGridViewCellStyle4;
            this.位置.HeaderText = "位置";
            this.位置.MinimumWidth = 6;
            this.位置.Name = "位置";
            this.位置.ReadOnly = true;
            // 
            // 鋳造長
            // 
            this.鋳造長.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.鋳造長.DataPropertyName = "鋳造長";
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.DarkGray;
            this.鋳造長.DefaultCellStyle = dataGridViewCellStyle5;
            this.鋳造長.HeaderText = "鋳造長(m)";
            this.鋳造長.MinimumWidth = 6;
            this.鋳造長.Name = "鋳造長";
            this.鋳造長.ReadOnly = true;
            // 
            // 初期凝固遅れ危険度
            // 
            this.初期凝固遅れ危険度.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.初期凝固遅れ危険度.DataPropertyName = "初期凝固遅れ危険度";
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.DarkGray;
            this.初期凝固遅れ危険度.DefaultCellStyle = dataGridViewCellStyle6;
            this.初期凝固遅れ危険度.HeaderText = "初期凝固遅れ危険度(%)";
            this.初期凝固遅れ危険度.MinimumWidth = 6;
            this.初期凝固遅れ危険度.Name = "初期凝固遅れ危険度";
            this.初期凝固遅れ危険度.ReadOnly = true;
            // 
            // 再溶解危険度
            // 
            this.再溶解危険度.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.再溶解危険度.DataPropertyName = "再溶解危険度";
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.DarkGray;
            this.再溶解危険度.DefaultCellStyle = dataGridViewCellStyle7;
            this.再溶解危険度.HeaderText = "再溶解危険度(%)";
            this.再溶解危険度.MinimumWidth = 6;
            this.再溶解危険度.Name = "再溶解危険度";
            this.再溶解危険度.ReadOnly = true;
            // 
            // 警報値
            // 
            this.警報値.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.警報値.DataPropertyName = "警報値";
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.DarkGray;
            this.警報値.DefaultCellStyle = dataGridViewCellStyle8;
            this.警報値.HeaderText = "警報値";
            this.警報値.MinimumWidth = 6;
            this.警報値.Name = "警報値";
            this.警報値.ReadOnly = true;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.SlateGray;
            this.panel3.Controls.Add(this.cboPageSize);
            this.panel3.Controls.Add(this.btnBack);
            this.panel3.Controls.Add(this.lblPage);
            this.panel3.Controls.Add(this.btnNext);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 797);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1751, 40);
            this.panel3.TabIndex = 0;
            // 
            // cboPageSize
            // 
            this.cboPageSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cboPageSize.FormattingEnabled = true;
            this.cboPageSize.Items.AddRange(new object[] {
            "10"});
            this.cboPageSize.Location = new System.Drawing.Point(1442, 8);
            this.cboPageSize.Name = "cboPageSize";
            this.cboPageSize.Size = new System.Drawing.Size(121, 21);
            this.cboPageSize.TabIndex = 4;
            this.cboPageSize.SelectedIndexChanged += new System.EventHandler(this.cboPageSize_SelectedIndexChanged);
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBack.Location = new System.Drawing.Point(1643, 6);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(20, 23);
            this.btnBack.TabIndex = 2;
            this.btnBack.Text = "<";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // lblPage
            // 
            this.lblPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPage.AutoSize = true;
            this.lblPage.Location = new System.Drawing.Point(1569, 12);
            this.lblPage.Name = "lblPage";
            this.lblPage.Size = new System.Drawing.Size(0, 13);
            this.lblPage.TabIndex = 3;
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Location = new System.Drawing.Point(1676, 6);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(20, 23);
            this.btnNext.TabIndex = 1;
            this.btnNext.Text = ">";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // HistoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1904, 862);
            this.Controls.Add(this.splMainContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "HistoryForm";
            this.Text = "警報履歴";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.HistoryForm_Load);
            this.pnlHeaderRight.ResumeLayout(false);
            this.pnlHeaderRight.PerformLayout();
            this.splMainContainer.Panel1.ResumeLayout(false);
            this.splMainContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splMainContainer)).EndInit();
            this.splMainContainer.ResumeLayout(false);
            this.pnlHeaderLeft.ResumeLayout(false);
            this.pnlHeaderLeft.PerformLayout();
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtToWarningPoint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFromWarningPoint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtmM)).EndInit();
            this.pnRight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdHistory)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnFilterHistory;
        private System.Windows.Forms.Label lblTitleGrdHistory;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.Panel pnlHeaderRight;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.SplitContainer splMainContainer;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel pnlHeaderLeft;
        private System.Windows.Forms.Label lblTitleMenu;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Panel pnlFilter;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.Label lbldTo;
        private System.Windows.Forms.Label lbldFrom;
        private System.Windows.Forms.Panel pnRight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbltFrom;
        private System.Windows.Forms.DataGridView grdHistory;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ComboBox cboPageSize;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Label lblPage;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.NumericUpDown txtmM;
        private System.Windows.Forms.NumericUpDown txtToWarningPoint;
        private System.Windows.Forms.NumericUpDown txtFromWarningPoint;
        private System.Windows.Forms.DataGridViewTextBoxColumn 警報日時;
        private System.Windows.Forms.DataGridViewTextBoxColumn 位置;
        private System.Windows.Forms.DataGridViewTextBoxColumn 鋳造長;
        private System.Windows.Forms.DataGridViewTextBoxColumn 初期凝固遅れ危険度;
        private System.Windows.Forms.DataGridViewTextBoxColumn 再溶解危険度;
        private System.Windows.Forms.DataGridViewTextBoxColumn 警報値;
    }
}