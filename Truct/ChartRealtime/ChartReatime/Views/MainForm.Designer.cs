using System.Threading;

namespace ChartRealtime
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea5 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend5 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea6 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend6 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea7 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend7 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chbRealTime = new System.Windows.Forms.CheckBox();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.splMainContainer = new System.Windows.Forms.SplitContainer();
            this.btnViewHistory = new System.Windows.Forms.Button();
            this.pnlHeaderLeft = new System.Windows.Forms.Panel();
            this.lblTitleMenu = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.pnlFilter = new System.Windows.Forms.Panel();
            this.txtmM = new System.Windows.Forms.NumericUpDown();
            this.lblLength = new System.Windows.Forms.Label();
            this.lblTo = new System.Windows.Forms.Label();
            this.lblFrom = new System.Windows.Forms.Label();
            this.lblFilter = new System.Windows.Forms.Label();
            this.lblRealTime = new System.Windows.Forms.Label();
            this.btnLoadChart = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tblLayoutLeft = new System.Windows.Forms.TableLayoutPanel();
            this.tblLayoutTopLeft = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel32 = new System.Windows.Forms.Panel();
            this.chrEastBO = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.pnlBoHeaderEast = new System.Windows.Forms.Panel();
            this.lblEastWhiteTitle = new System.Windows.Forms.Label();
            this.lblTitleeastBO = new System.Windows.Forms.Label();
            this.lblEastRedTitle = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.lblEastColorWhite = new System.Windows.Forms.Label();
            this.lblEastColorOrange = new System.Windows.Forms.Label();
            this.lblBoLeftTitle = new System.Windows.Forms.Label();
            this.lblEastColorRed = new System.Windows.Forms.Label();
            this.lblEastColorBlue = new System.Windows.Forms.Label();
            this.lblEastBlueTitle = new System.Windows.Forms.Label();
            this.lblEastOrangeTitle = new System.Windows.Forms.Label();
            this.tblLayoutBottomLeft = new System.Windows.Forms.TableLayoutPanel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.chrEastRateTempChange = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel45 = new System.Windows.Forms.Panel();
            this.label58 = new System.Windows.Forms.Label();
            this.panel38 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.chrEastElectrodeTemp = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel36 = new System.Windows.Forms.Panel();
            this.label26 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.tblLayoutRight = new System.Windows.Forms.TableLayoutPanel();
            this.tblLayoutTopRight = new System.Windows.Forms.TableLayoutPanel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel19 = new System.Windows.Forms.Panel();
            this.chrWesternBO = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.pnlBoHeaderWestern = new System.Windows.Forms.Panel();
            this.lblWesternWhiteTitle = new System.Windows.Forms.Label();
            this.lblWesternBlueTitle = new System.Windows.Forms.Label();
            this.lblWesternRedTitle = new System.Windows.Forms.Label();
            this.lblWesternOrangeTitle = new System.Windows.Forms.Label();
            this.lblWesternColorWhite = new System.Windows.Forms.Label();
            this.lblTitleWesternBO = new System.Windows.Forms.Label();
            this.lblWesternColorOrange = new System.Windows.Forms.Label();
            this.lblWesternColorRed = new System.Windows.Forms.Label();
            this.lblBOWesternTitle = new System.Windows.Forms.Label();
            this.lblWesternColorBlue = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.tblLayoutBottomRight = new System.Windows.Forms.TableLayoutPanel();
            this.panel11 = new System.Windows.Forms.Panel();
            this.chrWesternElectrodeTemp = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel22 = new System.Windows.Forms.Panel();
            this.label32 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.chart13 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel12 = new System.Windows.Forms.Panel();
            this.chrWesternRateTempChange = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel49 = new System.Windows.Forms.Panel();
            this.label57 = new System.Windows.Forms.Label();
            this.panel24 = new System.Windows.Forms.Panel();
            this.pnlHeaderRight = new System.Windows.Forms.Panel();
            this.lblTitleChart = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splMainContainer)).BeginInit();
            this.splMainContainer.Panel1.SuspendLayout();
            this.splMainContainer.Panel2.SuspendLayout();
            this.splMainContainer.SuspendLayout();
            this.pnlHeaderLeft.SuspendLayout();
            this.pnlFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtmM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tblLayoutLeft.SuspendLayout();
            this.tblLayoutTopLeft.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel32.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chrEastBO)).BeginInit();
            this.pnlBoHeaderEast.SuspendLayout();
            this.tblLayoutBottomLeft.SuspendLayout();
            this.panel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chrEastRateTempChange)).BeginInit();
            this.panel45.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chrEastElectrodeTemp)).BeginInit();
            this.panel36.SuspendLayout();
            this.tblLayoutRight.SuspendLayout();
            this.tblLayoutTopRight.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel19.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chrWesternBO)).BeginInit();
            this.pnlBoHeaderWestern.SuspendLayout();
            this.tblLayoutBottomRight.SuspendLayout();
            this.panel11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chrWesternElectrodeTemp)).BeginInit();
            this.panel22.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart13)).BeginInit();
            this.panel12.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chrWesternRateTempChange)).BeginInit();
            this.panel49.SuspendLayout();
            this.pnlHeaderRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // chbRealTime
            // 
            this.chbRealTime.AutoSize = true;
            this.chbRealTime.Checked = true;
            this.chbRealTime.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbRealTime.Location = new System.Drawing.Point(117, 10);
            this.chbRealTime.Margin = new System.Windows.Forms.Padding(2);
            this.chbRealTime.Name = "chbRealTime";
            this.chbRealTime.Size = new System.Drawing.Size(15, 14);
            this.chbRealTime.TabIndex = 2;
            this.chbRealTime.UseVisualStyleBackColor = true;
            this.chbRealTime.CheckedChanged += new System.EventHandler(this.chbRealTime_CheckedChanged);
            // 
            // dtpTo
            // 
            this.dtpTo.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.dtpTo.Enabled = false;
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTo.Location = new System.Drawing.Point(4, 176);
            this.dtpTo.Margin = new System.Windows.Forms.Padding(2);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(143, 20);
            this.dtpTo.TabIndex = 8;
            // 
            // dtpFrom
            // 
            this.dtpFrom.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.dtpFrom.Enabled = false;
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFrom.Location = new System.Drawing.Point(4, 132);
            this.dtpFrom.Margin = new System.Windows.Forms.Padding(2);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(144, 20);
            this.dtpFrom.TabIndex = 7;
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
            this.splMainContainer.Panel1.Controls.Add(this.btnViewHistory);
            this.splMainContainer.Panel1.Controls.Add(this.pnlHeaderLeft);
            this.splMainContainer.Panel1.Controls.Add(this.pnlFilter);
            this.splMainContainer.Panel1MinSize = 30;
            // 
            // splMainContainer.Panel2
            // 
            this.splMainContainer.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.splMainContainer.Panel2.Controls.Add(this.splitContainer1);
            this.splMainContainer.Panel2.Controls.Add(this.pnlHeaderRight);
            this.splMainContainer.Size = new System.Drawing.Size(1904, 1041);
            this.splMainContainer.SplitterDistance = 150;
            this.splMainContainer.SplitterWidth = 3;
            this.splMainContainer.TabIndex = 11;
            // 
            // btnViewHistory
            // 
            this.btnViewHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnViewHistory.BackColor = System.Drawing.Color.Orange;
            this.btnViewHistory.Font = new System.Drawing.Font("MS Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnViewHistory.Image = ((System.Drawing.Image)(resources.GetObject("btnViewHistory.Image")));
            this.btnViewHistory.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnViewHistory.Location = new System.Drawing.Point(12, 979);
            this.btnViewHistory.Margin = new System.Windows.Forms.Padding(2);
            this.btnViewHistory.Name = "btnViewHistory";
            this.btnViewHistory.Size = new System.Drawing.Size(130, 40);
            this.btnViewHistory.TabIndex = 19;
            this.btnViewHistory.Text = "  警報履歴";
            this.btnViewHistory.UseVisualStyleBackColor = false;
            this.btnViewHistory.Click += new System.EventHandler(this.btnHistory_Click);
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
            this.lblTitleMenu.Font = new System.Drawing.Font("MS Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitleMenu.Location = new System.Drawing.Point(16, 3);
            this.lblTitleMenu.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTitleMenu.Name = "lblTitleMenu";
            this.lblTitleMenu.Size = new System.Drawing.Size(75, 15);
            this.lblTitleMenu.TabIndex = 3;
            this.lblTitleMenu.Text = "表示条件";
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
            this.pnlFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlFilter.Controls.Add(this.txtmM);
            this.pnlFilter.Controls.Add(this.lblLength);
            this.pnlFilter.Controls.Add(this.lblTo);
            this.pnlFilter.Controls.Add(this.lblFrom);
            this.pnlFilter.Controls.Add(this.lblFilter);
            this.pnlFilter.Controls.Add(this.lblRealTime);
            this.pnlFilter.Controls.Add(this.chbRealTime);
            this.pnlFilter.Controls.Add(this.dtpFrom);
            this.pnlFilter.Controls.Add(this.dtpTo);
            this.pnlFilter.Controls.Add(this.btnLoadChart);
            this.pnlFilter.Location = new System.Drawing.Point(1, 75);
            this.pnlFilter.Margin = new System.Windows.Forms.Padding(2);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Size = new System.Drawing.Size(150, 364);
            this.pnlFilter.TabIndex = 17;
            // 
            // txtmM
            // 
            this.txtmM.DecimalPlaces = 2;
            this.txtmM.Enabled = false;
            this.txtmM.InterceptArrowKeys = false;
            this.txtmM.Location = new System.Drawing.Point(4, 226);
            this.txtmM.Maximum = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.txtmM.Name = "txtmM";
            this.txtmM.Size = new System.Drawing.Size(143, 20);
            this.txtmM.TabIndex = 30;
            this.txtmM.ThousandsSeparator = true;
            // 
            // lblLength
            // 
            this.lblLength.AutoSize = true;
            this.lblLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLength.Location = new System.Drawing.Point(5, 206);
            this.lblLength.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblLength.Name = "lblLength";
            this.lblLength.Size = new System.Drawing.Size(82, 17);
            this.lblLength.TabIndex = 21;
            this.lblLength.Text = "鋳造長(mm)";
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTo.Location = new System.Drawing.Point(5, 154);
            this.lblTo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(68, 17);
            this.lblTo.TabIndex = 20;
            this.lblTo.Text = "期間（TO)";
            // 
            // lblFrom
            // 
            this.lblFrom.AutoSize = true;
            this.lblFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFrom.Location = new System.Drawing.Point(5, 110);
            this.lblFrom.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(88, 17);
            this.lblFrom.TabIndex = 19;
            this.lblFrom.Text = "期間（FROM)";
            // 
            // lblFilter
            // 
            this.lblFilter.AutoSize = true;
            this.lblFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFilter.Location = new System.Drawing.Point(3, 70);
            this.lblFilter.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblFilter.Name = "lblFilter";
            this.lblFilter.Size = new System.Drawing.Size(68, 17);
            this.lblFilter.TabIndex = 18;
            this.lblFilter.Text = "表示条件";
            // 
            // lblRealTime
            // 
            this.lblRealTime.AutoSize = true;
            this.lblRealTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRealTime.Location = new System.Drawing.Point(8, 8);
            this.lblRealTime.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblRealTime.Name = "lblRealTime";
            this.lblRealTime.Size = new System.Drawing.Size(105, 17);
            this.lblRealTime.TabIndex = 16;
            this.lblRealTime.Text = "リアルタイム表示";
            // 
            // btnLoadChart
            // 
            this.btnLoadChart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLoadChart.BackColor = System.Drawing.Color.Orange;
            this.btnLoadChart.Font = new System.Drawing.Font("MS Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoadChart.Image = ((System.Drawing.Image)(resources.GetObject("btnLoadChart.Image")));
            this.btnLoadChart.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLoadChart.Location = new System.Drawing.Point(18, 278);
            this.btnLoadChart.Margin = new System.Windows.Forms.Padding(2);
            this.btnLoadChart.Name = "btnLoadChart";
            this.btnLoadChart.Size = new System.Drawing.Size(112, 40);
            this.btnLoadChart.TabIndex = 0;
            this.btnLoadChart.Text = " 表示";
            this.btnLoadChart.UseVisualStyleBackColor = false;
            this.btnLoadChart.Click += new System.EventHandler(this.btn_loadchart_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(139)))), ((int)(((byte)(0)))));
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.Black;
            this.splitContainer1.Panel1.Controls.Add(this.tblLayoutLeft);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.Black;
            this.splitContainer1.Panel2.Controls.Add(this.tblLayoutRight);
            this.splitContainer1.Size = new System.Drawing.Size(1751, 1016);
            this.splitContainer1.SplitterDistance = 870;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 19;
            // 
            // tblLayoutLeft
            // 
            this.tblLayoutLeft.ColumnCount = 1;
            this.tblLayoutLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblLayoutLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tblLayoutLeft.Controls.Add(this.tblLayoutTopLeft, 0, 0);
            this.tblLayoutLeft.Controls.Add(this.tblLayoutBottomLeft, 0, 1);
            this.tblLayoutLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblLayoutLeft.Location = new System.Drawing.Point(0, 0);
            this.tblLayoutLeft.Margin = new System.Windows.Forms.Padding(2);
            this.tblLayoutLeft.Name = "tblLayoutLeft";
            this.tblLayoutLeft.RowCount = 2;
            this.tblLayoutLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 61.64875F));
            this.tblLayoutLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 38.35125F));
            this.tblLayoutLeft.Size = new System.Drawing.Size(870, 1016);
            this.tblLayoutLeft.TabIndex = 2;
            // 
            // tblLayoutTopLeft
            // 
            this.tblLayoutTopLeft.ColumnCount = 1;
            this.tblLayoutTopLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblLayoutTopLeft.Controls.Add(this.panel2, 0, 0);
            this.tblLayoutTopLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblLayoutTopLeft.Location = new System.Drawing.Point(2, 2);
            this.tblLayoutTopLeft.Margin = new System.Windows.Forms.Padding(2);
            this.tblLayoutTopLeft.Name = "tblLayoutTopLeft";
            this.tblLayoutTopLeft.RowCount = 1;
            this.tblLayoutTopLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblLayoutTopLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 622F));
            this.tblLayoutTopLeft.Size = new System.Drawing.Size(866, 622);
            this.tblLayoutTopLeft.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel32);
            this.panel2.Controls.Add(this.pnlBoHeaderEast);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(2, 2);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(862, 618);
            this.panel2.TabIndex = 0;
            // 
            // panel32
            // 
            this.panel32.Controls.Add(this.chrEastBO);
            this.panel32.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel32.Location = new System.Drawing.Point(0, 52);
            this.panel32.Margin = new System.Windows.Forms.Padding(2);
            this.panel32.Name = "panel32";
            this.panel32.Size = new System.Drawing.Size(862, 566);
            this.panel32.TabIndex = 1;
            // 
            // chrEastBO
            // 
            chartArea1.Name = "ChartArea1";
            this.chrEastBO.ChartAreas.Add(chartArea1);
            this.chrEastBO.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chrEastBO.Legends.Add(legend1);
            this.chrEastBO.Location = new System.Drawing.Point(0, 0);
            this.chrEastBO.Margin = new System.Windows.Forms.Padding(2);
            this.chrEastBO.Name = "chrEastBO";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chrEastBO.Series.Add(series1);
            this.chrEastBO.Size = new System.Drawing.Size(862, 566);
            this.chrEastBO.TabIndex = 9;
            // 
            // pnlBoHeaderEast
            // 
            this.pnlBoHeaderEast.Controls.Add(this.lblEastWhiteTitle);
            this.pnlBoHeaderEast.Controls.Add(this.lblTitleeastBO);
            this.pnlBoHeaderEast.Controls.Add(this.lblEastRedTitle);
            this.pnlBoHeaderEast.Controls.Add(this.label18);
            this.pnlBoHeaderEast.Controls.Add(this.lblEastColorWhite);
            this.pnlBoHeaderEast.Controls.Add(this.lblEastColorOrange);
            this.pnlBoHeaderEast.Controls.Add(this.lblBoLeftTitle);
            this.pnlBoHeaderEast.Controls.Add(this.lblEastColorRed);
            this.pnlBoHeaderEast.Controls.Add(this.lblEastColorBlue);
            this.pnlBoHeaderEast.Controls.Add(this.lblEastBlueTitle);
            this.pnlBoHeaderEast.Controls.Add(this.lblEastOrangeTitle);
            this.pnlBoHeaderEast.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBoHeaderEast.Location = new System.Drawing.Point(0, 0);
            this.pnlBoHeaderEast.Margin = new System.Windows.Forms.Padding(2);
            this.pnlBoHeaderEast.Name = "pnlBoHeaderEast";
            this.pnlBoHeaderEast.Size = new System.Drawing.Size(862, 52);
            this.pnlBoHeaderEast.TabIndex = 0;
            // 
            // lblEastWhiteTitle
            // 
            this.lblEastWhiteTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEastWhiteTitle.AutoSize = true;
            this.lblEastWhiteTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblEastWhiteTitle.Location = new System.Drawing.Point(512, 33);
            this.lblEastWhiteTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblEastWhiteTitle.Name = "lblEastWhiteTitle";
            this.lblEastWhiteTitle.Size = new System.Drawing.Size(79, 13);
            this.lblEastWhiteTitle.TabIndex = 14;
            this.lblEastWhiteTitle.Text = "再溶解危険度";
            // 
            // lblTitleeastBO
            // 
            this.lblTitleeastBO.AutoSize = true;
            this.lblTitleeastBO.Font = new System.Drawing.Font("MS Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitleeastBO.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblTitleeastBO.Location = new System.Drawing.Point(392, 8);
            this.lblTitleeastBO.Name = "lblTitleeastBO";
            this.lblTitleeastBO.Size = new System.Drawing.Size(72, 19);
            this.lblTitleeastBO.TabIndex = 19;
            this.lblTitleeastBO.Text = "東短辺";
            // 
            // lblEastRedTitle
            // 
            this.lblEastRedTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEastRedTitle.AutoSize = true;
            this.lblEastRedTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblEastRedTitle.Location = new System.Drawing.Point(512, 16);
            this.lblEastRedTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblEastRedTitle.Name = "lblEastRedTitle";
            this.lblEastRedTitle.Size = new System.Drawing.Size(156, 13);
            this.lblEastRedTitle.TabIndex = 13;
            this.lblEastRedTitle.Text = "初期凝固遅れ・再溶解危険度";
            // 
            // label18
            // 
            this.label18.BackColor = System.Drawing.Color.Red;
            this.label18.Location = new System.Drawing.Point(2, 44);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(120, 3);
            this.label18.TabIndex = 13;
            // 
            // lblEastColorWhite
            // 
            this.lblEastColorWhite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEastColorWhite.BackColor = System.Drawing.Color.White;
            this.lblEastColorWhite.Location = new System.Drawing.Point(497, 35);
            this.lblEastColorWhite.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblEastColorWhite.Name = "lblEastColorWhite";
            this.lblEastColorWhite.Size = new System.Drawing.Size(8, 8);
            this.lblEastColorWhite.TabIndex = 12;
            // 
            // lblEastColorOrange
            // 
            this.lblEastColorOrange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEastColorOrange.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(139)))), ((int)(((byte)(0)))));
            this.lblEastColorOrange.Location = new System.Drawing.Point(673, 35);
            this.lblEastColorOrange.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblEastColorOrange.Name = "lblEastColorOrange";
            this.lblEastColorOrange.Size = new System.Drawing.Size(8, 8);
            this.lblEastColorOrange.TabIndex = 15;
            // 
            // lblBoLeftTitle
            // 
            this.lblBoLeftTitle.AutoSize = true;
            this.lblBoLeftTitle.Font = new System.Drawing.Font("MS Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBoLeftTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblBoLeftTitle.Location = new System.Drawing.Point(2, 28);
            this.lblBoLeftTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblBoLeftTitle.Name = "lblBoLeftTitle";
            this.lblBoLeftTitle.Size = new System.Drawing.Size(127, 15);
            this.lblBoLeftTitle.TabIndex = 12;
            this.lblBoLeftTitle.Text = "BO危険度・警告";
            // 
            // lblEastColorRed
            // 
            this.lblEastColorRed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEastColorRed.BackColor = System.Drawing.Color.Red;
            this.lblEastColorRed.Location = new System.Drawing.Point(497, 19);
            this.lblEastColorRed.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblEastColorRed.Name = "lblEastColorRed";
            this.lblEastColorRed.Size = new System.Drawing.Size(8, 8);
            this.lblEastColorRed.TabIndex = 11;
            // 
            // lblEastColorBlue
            // 
            this.lblEastColorBlue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEastColorBlue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(125)))), ((int)(((byte)(241)))));
            this.lblEastColorBlue.Location = new System.Drawing.Point(673, 21);
            this.lblEastColorBlue.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblEastColorBlue.Name = "lblEastColorBlue";
            this.lblEastColorBlue.Size = new System.Drawing.Size(8, 8);
            this.lblEastColorBlue.TabIndex = 14;
            // 
            // lblEastBlueTitle
            // 
            this.lblEastBlueTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEastBlueTitle.AutoSize = true;
            this.lblEastBlueTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblEastBlueTitle.Location = new System.Drawing.Point(688, 21);
            this.lblEastBlueTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblEastBlueTitle.Name = "lblEastBlueTitle";
            this.lblEastBlueTitle.Size = new System.Drawing.Size(43, 13);
            this.lblEastBlueTitle.TabIndex = 10;
            this.lblEastBlueTitle.Text = "危険度";
            // 
            // lblEastOrangeTitle
            // 
            this.lblEastOrangeTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEastOrangeTitle.AutoSize = true;
            this.lblEastOrangeTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblEastOrangeTitle.Location = new System.Drawing.Point(688, 34);
            this.lblEastOrangeTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblEastOrangeTitle.Name = "lblEastOrangeTitle";
            this.lblEastOrangeTitle.Size = new System.Drawing.Size(31, 13);
            this.lblEastOrangeTitle.TabIndex = 11;
            this.lblEastOrangeTitle.Text = "警告";
            // 
            // tblLayoutBottomLeft
            // 
            this.tblLayoutBottomLeft.ColumnCount = 2;
            this.tblLayoutBottomLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblLayoutBottomLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblLayoutBottomLeft.Controls.Add(this.panel8, 1, 0);
            this.tblLayoutBottomLeft.Controls.Add(this.panel5, 0, 0);
            this.tblLayoutBottomLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblLayoutBottomLeft.Location = new System.Drawing.Point(2, 628);
            this.tblLayoutBottomLeft.Margin = new System.Windows.Forms.Padding(2);
            this.tblLayoutBottomLeft.Name = "tblLayoutBottomLeft";
            this.tblLayoutBottomLeft.RowCount = 1;
            this.tblLayoutBottomLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 99.99999F));
            this.tblLayoutBottomLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 386F));
            this.tblLayoutBottomLeft.Size = new System.Drawing.Size(866, 386);
            this.tblLayoutBottomLeft.TabIndex = 1;
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.chrEastRateTempChange);
            this.panel8.Controls.Add(this.panel45);
            this.panel8.Controls.Add(this.panel38);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel8.Location = new System.Drawing.Point(435, 2);
            this.panel8.Margin = new System.Windows.Forms.Padding(2);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(429, 382);
            this.panel8.TabIndex = 1;
            // 
            // chrEastRateTempChange
            // 
            chartArea2.Name = "ChartArea1";
            this.chrEastRateTempChange.ChartAreas.Add(chartArea2);
            this.chrEastRateTempChange.Dock = System.Windows.Forms.DockStyle.Fill;
            legend2.Name = "Legend1";
            this.chrEastRateTempChange.Legends.Add(legend2);
            this.chrEastRateTempChange.Location = new System.Drawing.Point(0, 35);
            this.chrEastRateTempChange.Margin = new System.Windows.Forms.Padding(2);
            this.chrEastRateTempChange.Name = "chrEastRateTempChange";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chrEastRateTempChange.Series.Add(series2);
            this.chrEastRateTempChange.Size = new System.Drawing.Size(429, 322);
            this.chrEastRateTempChange.TabIndex = 3;
            this.chrEastRateTempChange.Text = "chart6";
            // 
            // panel45
            // 
            this.panel45.Controls.Add(this.label58);
            this.panel45.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel45.Location = new System.Drawing.Point(0, 357);
            this.panel45.Name = "panel45";
            this.panel45.Size = new System.Drawing.Size(429, 25);
            this.panel45.TabIndex = 0;
            // 
            // label58
            // 
            this.label58.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label58.AutoSize = true;
            this.label58.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label58.Location = new System.Drawing.Point(168, 9);
            this.label58.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(67, 13);
            this.label58.TabIndex = 9;
            this.label58.Text = "熱電対位置";
            // 
            // panel38
            // 
            this.panel38.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel38.Location = new System.Drawing.Point(0, 0);
            this.panel38.Margin = new System.Windows.Forms.Padding(2);
            this.panel38.Name = "panel38";
            this.panel38.Size = new System.Drawing.Size(429, 35);
            this.panel38.TabIndex = 0;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.chrEastElectrodeTemp);
            this.panel5.Controls.Add(this.panel1);
            this.panel5.Controls.Add(this.panel36);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(3, 3);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(427, 380);
            this.panel5.TabIndex = 2;
            // 
            // chrEastElectrodeTemp
            // 
            chartArea3.Name = "ChartArea1";
            this.chrEastElectrodeTemp.ChartAreas.Add(chartArea3);
            this.chrEastElectrodeTemp.Dock = System.Windows.Forms.DockStyle.Fill;
            legend3.Name = "Legend1";
            this.chrEastElectrodeTemp.Legends.Add(legend3);
            this.chrEastElectrodeTemp.Location = new System.Drawing.Point(0, 35);
            this.chrEastElectrodeTemp.Margin = new System.Windows.Forms.Padding(2);
            this.chrEastElectrodeTemp.Name = "chrEastElectrodeTemp";
            series3.ChartArea = "ChartArea1";
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            this.chrEastElectrodeTemp.Series.Add(series3);
            this.chrEastElectrodeTemp.Size = new System.Drawing.Size(427, 320);
            this.chrEastElectrodeTemp.TabIndex = 11;
            this.chrEastElectrodeTemp.Text = "chart5";
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 355);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(427, 25);
            this.panel1.TabIndex = 12;
            // 
            // panel36
            // 
            this.panel36.Controls.Add(this.label26);
            this.panel36.Controls.Add(this.label25);
            this.panel36.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel36.Location = new System.Drawing.Point(0, 0);
            this.panel36.Margin = new System.Windows.Forms.Padding(2);
            this.panel36.Name = "panel36";
            this.panel36.Size = new System.Drawing.Size(427, 35);
            this.panel36.TabIndex = 0;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("MS Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label26.Location = new System.Drawing.Point(2, 5);
            this.label26.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(109, 15);
            this.label26.TabIndex = 12;
            this.label26.Text = "初期凝固遅れ";
            // 
            // label25
            // 
            this.label25.BackColor = System.Drawing.Color.Red;
            this.label25.Location = new System.Drawing.Point(2, 25);
            this.label25.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(100, 3);
            this.label25.TabIndex = 13;
            // 
            // tblLayoutRight
            // 
            this.tblLayoutRight.ColumnCount = 1;
            this.tblLayoutRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblLayoutRight.Controls.Add(this.tblLayoutTopRight, 0, 0);
            this.tblLayoutRight.Controls.Add(this.tblLayoutBottomRight, 0, 1);
            this.tblLayoutRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblLayoutRight.Location = new System.Drawing.Point(0, 0);
            this.tblLayoutRight.Margin = new System.Windows.Forms.Padding(2);
            this.tblLayoutRight.Name = "tblLayoutRight";
            this.tblLayoutRight.RowCount = 2;
            this.tblLayoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 61.52927F));
            this.tblLayoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 38.47073F));
            this.tblLayoutRight.Size = new System.Drawing.Size(878, 1016);
            this.tblLayoutRight.TabIndex = 3;
            // 
            // tblLayoutTopRight
            // 
            this.tblLayoutTopRight.ColumnCount = 1;
            this.tblLayoutTopRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblLayoutTopRight.Controls.Add(this.panel4, 0, 0);
            this.tblLayoutTopRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblLayoutTopRight.Location = new System.Drawing.Point(2, 2);
            this.tblLayoutTopRight.Margin = new System.Windows.Forms.Padding(2);
            this.tblLayoutTopRight.Name = "tblLayoutTopRight";
            this.tblLayoutTopRight.RowCount = 1;
            this.tblLayoutTopRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblLayoutTopRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 621F));
            this.tblLayoutTopRight.Size = new System.Drawing.Size(874, 621);
            this.tblLayoutTopRight.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.panel19);
            this.panel4.Controls.Add(this.pnlBoHeaderWestern);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(2, 2);
            this.panel4.Margin = new System.Windows.Forms.Padding(2);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(870, 617);
            this.panel4.TabIndex = 0;
            // 
            // panel19
            // 
            this.panel19.Controls.Add(this.chrWesternBO);
            this.panel19.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel19.Location = new System.Drawing.Point(0, 52);
            this.panel19.Name = "panel19";
            this.panel19.Size = new System.Drawing.Size(870, 565);
            this.panel19.TabIndex = 1;
            // 
            // chrWesternBO
            // 
            chartArea4.Name = "ChartArea1";
            this.chrWesternBO.ChartAreas.Add(chartArea4);
            this.chrWesternBO.Dock = System.Windows.Forms.DockStyle.Fill;
            legend4.Name = "Legend1";
            this.chrWesternBO.Legends.Add(legend4);
            this.chrWesternBO.Location = new System.Drawing.Point(0, 0);
            this.chrWesternBO.Margin = new System.Windows.Forms.Padding(2);
            this.chrWesternBO.Name = "chrWesternBO";
            series4.ChartArea = "ChartArea1";
            series4.Legend = "Legend1";
            series4.Name = "Series1";
            this.chrWesternBO.Series.Add(series4);
            this.chrWesternBO.Size = new System.Drawing.Size(870, 565);
            this.chrWesternBO.TabIndex = 13;
            this.chrWesternBO.Text = "chart2";
            // 
            // pnlBoHeaderWestern
            // 
            this.pnlBoHeaderWestern.Controls.Add(this.lblWesternWhiteTitle);
            this.pnlBoHeaderWestern.Controls.Add(this.lblWesternBlueTitle);
            this.pnlBoHeaderWestern.Controls.Add(this.lblWesternRedTitle);
            this.pnlBoHeaderWestern.Controls.Add(this.lblWesternOrangeTitle);
            this.pnlBoHeaderWestern.Controls.Add(this.lblWesternColorWhite);
            this.pnlBoHeaderWestern.Controls.Add(this.lblTitleWesternBO);
            this.pnlBoHeaderWestern.Controls.Add(this.lblWesternColorOrange);
            this.pnlBoHeaderWestern.Controls.Add(this.lblWesternColorRed);
            this.pnlBoHeaderWestern.Controls.Add(this.lblBOWesternTitle);
            this.pnlBoHeaderWestern.Controls.Add(this.lblWesternColorBlue);
            this.pnlBoHeaderWestern.Controls.Add(this.label27);
            this.pnlBoHeaderWestern.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBoHeaderWestern.Location = new System.Drawing.Point(0, 0);
            this.pnlBoHeaderWestern.Name = "pnlBoHeaderWestern";
            this.pnlBoHeaderWestern.Size = new System.Drawing.Size(870, 52);
            this.pnlBoHeaderWestern.TabIndex = 0;
            // 
            // lblWesternWhiteTitle
            // 
            this.lblWesternWhiteTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWesternWhiteTitle.AutoSize = true;
            this.lblWesternWhiteTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblWesternWhiteTitle.Location = new System.Drawing.Point(484, 33);
            this.lblWesternWhiteTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblWesternWhiteTitle.Name = "lblWesternWhiteTitle";
            this.lblWesternWhiteTitle.Size = new System.Drawing.Size(79, 13);
            this.lblWesternWhiteTitle.TabIndex = 21;
            this.lblWesternWhiteTitle.Text = "再溶解危険度";
            // 
            // lblWesternBlueTitle
            // 
            this.lblWesternBlueTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWesternBlueTitle.AutoSize = true;
            this.lblWesternBlueTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblWesternBlueTitle.Location = new System.Drawing.Point(690, 21);
            this.lblWesternBlueTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblWesternBlueTitle.Name = "lblWesternBlueTitle";
            this.lblWesternBlueTitle.Size = new System.Drawing.Size(43, 13);
            this.lblWesternBlueTitle.TabIndex = 20;
            this.lblWesternBlueTitle.Text = "危険度";
            // 
            // lblWesternRedTitle
            // 
            this.lblWesternRedTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWesternRedTitle.AutoSize = true;
            this.lblWesternRedTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblWesternRedTitle.Location = new System.Drawing.Point(484, 16);
            this.lblWesternRedTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblWesternRedTitle.Name = "lblWesternRedTitle";
            this.lblWesternRedTitle.Size = new System.Drawing.Size(156, 13);
            this.lblWesternRedTitle.TabIndex = 20;
            this.lblWesternRedTitle.Text = "初期凝固遅れ・再溶解危険度";
            // 
            // lblWesternOrangeTitle
            // 
            this.lblWesternOrangeTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWesternOrangeTitle.AutoSize = true;
            this.lblWesternOrangeTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblWesternOrangeTitle.Location = new System.Drawing.Point(690, 36);
            this.lblWesternOrangeTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblWesternOrangeTitle.Name = "lblWesternOrangeTitle";
            this.lblWesternOrangeTitle.Size = new System.Drawing.Size(31, 13);
            this.lblWesternOrangeTitle.TabIndex = 19;
            this.lblWesternOrangeTitle.Text = "警告";
            // 
            // lblWesternColorWhite
            // 
            this.lblWesternColorWhite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWesternColorWhite.BackColor = System.Drawing.Color.White;
            this.lblWesternColorWhite.Location = new System.Drawing.Point(468, 35);
            this.lblWesternColorWhite.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblWesternColorWhite.Name = "lblWesternColorWhite";
            this.lblWesternColorWhite.Size = new System.Drawing.Size(8, 8);
            this.lblWesternColorWhite.TabIndex = 19;
            // 
            // lblTitleWesternBO
            // 
            this.lblTitleWesternBO.AutoSize = true;
            this.lblTitleWesternBO.Font = new System.Drawing.Font("MS Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitleWesternBO.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblTitleWesternBO.Location = new System.Drawing.Point(357, 8);
            this.lblTitleWesternBO.Name = "lblTitleWesternBO";
            this.lblTitleWesternBO.Size = new System.Drawing.Size(72, 19);
            this.lblTitleWesternBO.TabIndex = 18;
            this.lblTitleWesternBO.Text = "西短辺";
            // 
            // lblWesternColorOrange
            // 
            this.lblWesternColorOrange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWesternColorOrange.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(139)))), ((int)(((byte)(0)))));
            this.lblWesternColorOrange.Location = new System.Drawing.Point(673, 37);
            this.lblWesternColorOrange.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblWesternColorOrange.Name = "lblWesternColorOrange";
            this.lblWesternColorOrange.Size = new System.Drawing.Size(8, 8);
            this.lblWesternColorOrange.TabIndex = 17;
            // 
            // lblWesternColorRed
            // 
            this.lblWesternColorRed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWesternColorRed.BackColor = System.Drawing.Color.Red;
            this.lblWesternColorRed.Location = new System.Drawing.Point(467, 20);
            this.lblWesternColorRed.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblWesternColorRed.Name = "lblWesternColorRed";
            this.lblWesternColorRed.Size = new System.Drawing.Size(8, 8);
            this.lblWesternColorRed.TabIndex = 18;
            // 
            // lblBOWesternTitle
            // 
            this.lblBOWesternTitle.AutoSize = true;
            this.lblBOWesternTitle.Font = new System.Drawing.Font("MS Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBOWesternTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblBOWesternTitle.Location = new System.Drawing.Point(2, 28);
            this.lblBOWesternTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblBOWesternTitle.Name = "lblBOWesternTitle";
            this.lblBOWesternTitle.Size = new System.Drawing.Size(127, 15);
            this.lblBOWesternTitle.TabIndex = 14;
            this.lblBOWesternTitle.Text = "BO危険度・警告";
            // 
            // lblWesternColorBlue
            // 
            this.lblWesternColorBlue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWesternColorBlue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(125)))), ((int)(((byte)(241)))));
            this.lblWesternColorBlue.Location = new System.Drawing.Point(673, 21);
            this.lblWesternColorBlue.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblWesternColorBlue.Name = "lblWesternColorBlue";
            this.lblWesternColorBlue.Size = new System.Drawing.Size(8, 8);
            this.lblWesternColorBlue.TabIndex = 16;
            // 
            // label27
            // 
            this.label27.BackColor = System.Drawing.Color.Red;
            this.label27.Location = new System.Drawing.Point(2, 46);
            this.label27.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(120, 3);
            this.label27.TabIndex = 15;
            // 
            // tblLayoutBottomRight
            // 
            this.tblLayoutBottomRight.ColumnCount = 2;
            this.tblLayoutBottomRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblLayoutBottomRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblLayoutBottomRight.Controls.Add(this.panel11, 0, 0);
            this.tblLayoutBottomRight.Controls.Add(this.panel12, 1, 0);
            this.tblLayoutBottomRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblLayoutBottomRight.Location = new System.Drawing.Point(2, 627);
            this.tblLayoutBottomRight.Margin = new System.Windows.Forms.Padding(2);
            this.tblLayoutBottomRight.Name = "tblLayoutBottomRight";
            this.tblLayoutBottomRight.RowCount = 1;
            this.tblLayoutBottomRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 99.99999F));
            this.tblLayoutBottomRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 387F));
            this.tblLayoutBottomRight.Size = new System.Drawing.Size(874, 387);
            this.tblLayoutBottomRight.TabIndex = 1;
            // 
            // panel11
            // 
            this.panel11.Controls.Add(this.chrWesternElectrodeTemp);
            this.panel11.Controls.Add(this.panel3);
            this.panel11.Controls.Add(this.panel22);
            this.panel11.Controls.Add(this.chart13);
            this.panel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel11.Location = new System.Drawing.Point(2, 2);
            this.panel11.Margin = new System.Windows.Forms.Padding(2);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(433, 383);
            this.panel11.TabIndex = 0;
            // 
            // chrWesternElectrodeTemp
            // 
            chartArea5.Name = "ChartArea1";
            this.chrWesternElectrodeTemp.ChartAreas.Add(chartArea5);
            this.chrWesternElectrodeTemp.Dock = System.Windows.Forms.DockStyle.Fill;
            legend5.Name = "Legend1";
            this.chrWesternElectrodeTemp.Legends.Add(legend5);
            this.chrWesternElectrodeTemp.Location = new System.Drawing.Point(0, 35);
            this.chrWesternElectrodeTemp.Margin = new System.Windows.Forms.Padding(2);
            this.chrWesternElectrodeTemp.Name = "chrWesternElectrodeTemp";
            series5.ChartArea = "ChartArea1";
            series5.Legend = "Legend1";
            series5.Name = "Series1";
            this.chrWesternElectrodeTemp.Series.Add(series5);
            this.chrWesternElectrodeTemp.Size = new System.Drawing.Size(433, 323);
            this.chrWesternElectrodeTemp.TabIndex = 13;
            this.chrWesternElectrodeTemp.Text = "chart9";
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 358);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(433, 25);
            this.panel3.TabIndex = 14;
            // 
            // panel22
            // 
            this.panel22.Controls.Add(this.label32);
            this.panel22.Controls.Add(this.label31);
            this.panel22.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel22.Location = new System.Drawing.Point(0, 0);
            this.panel22.Name = "panel22";
            this.panel22.Size = new System.Drawing.Size(433, 35);
            this.panel22.TabIndex = 2;
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Font = new System.Drawing.Font("MS Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label32.Location = new System.Drawing.Point(2, 3);
            this.label32.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(109, 15);
            this.label32.TabIndex = 14;
            this.label32.Text = "初期凝固遅れ";
            // 
            // label31
            // 
            this.label31.BackColor = System.Drawing.Color.Red;
            this.label31.Location = new System.Drawing.Point(2, 21);
            this.label31.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(105, 3);
            this.label31.TabIndex = 15;
            // 
            // chart13
            // 
            chartArea6.Name = "ChartArea1";
            this.chart13.ChartAreas.Add(chartArea6);
            legend6.Name = "Legend1";
            this.chart13.Legends.Add(legend6);
            this.chart13.Location = new System.Drawing.Point(-465, 15);
            this.chart13.Margin = new System.Windows.Forms.Padding(2);
            this.chart13.Name = "chart13";
            series6.ChartArea = "ChartArea1";
            series6.Legend = "Legend1";
            series6.Name = "Series1";
            this.chart13.Series.Add(series6);
            this.chart13.Size = new System.Drawing.Size(174, 155);
            this.chart13.TabIndex = 1;
            this.chart13.Text = "chart5";
            // 
            // panel12
            // 
            this.panel12.Controls.Add(this.chrWesternRateTempChange);
            this.panel12.Controls.Add(this.panel49);
            this.panel12.Controls.Add(this.panel24);
            this.panel12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel12.Location = new System.Drawing.Point(439, 2);
            this.panel12.Margin = new System.Windows.Forms.Padding(2);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(433, 383);
            this.panel12.TabIndex = 1;
            // 
            // chrWesternRateTempChange
            // 
            chartArea7.Name = "ChartArea1";
            this.chrWesternRateTempChange.ChartAreas.Add(chartArea7);
            this.chrWesternRateTempChange.Dock = System.Windows.Forms.DockStyle.Fill;
            legend7.Name = "Legend1";
            this.chrWesternRateTempChange.Legends.Add(legend7);
            this.chrWesternRateTempChange.Location = new System.Drawing.Point(0, 35);
            this.chrWesternRateTempChange.Margin = new System.Windows.Forms.Padding(2);
            this.chrWesternRateTempChange.Name = "chrWesternRateTempChange";
            series7.ChartArea = "ChartArea1";
            series7.Legend = "Legend1";
            series7.Name = "Series1";
            this.chrWesternRateTempChange.Series.Add(series7);
            this.chrWesternRateTempChange.Size = new System.Drawing.Size(433, 323);
            this.chrWesternRateTempChange.TabIndex = 3;
            this.chrWesternRateTempChange.Text = "chart10";
            // 
            // panel49
            // 
            this.panel49.Controls.Add(this.label57);
            this.panel49.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel49.Location = new System.Drawing.Point(0, 358);
            this.panel49.Name = "panel49";
            this.panel49.Size = new System.Drawing.Size(433, 25);
            this.panel49.TabIndex = 0;
            // 
            // label57
            // 
            this.label57.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label57.AutoSize = true;
            this.label57.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label57.Location = new System.Drawing.Point(165, 10);
            this.label57.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(67, 13);
            this.label57.TabIndex = 8;
            this.label57.Text = "熱電対位置";
            // 
            // panel24
            // 
            this.panel24.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel24.Location = new System.Drawing.Point(0, 0);
            this.panel24.Name = "panel24";
            this.panel24.Size = new System.Drawing.Size(433, 35);
            this.panel24.TabIndex = 0;
            // 
            // pnlHeaderRight
            // 
            this.pnlHeaderRight.BackColor = System.Drawing.Color.SlateGray;
            this.pnlHeaderRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlHeaderRight.Controls.Add(this.lblTitleChart);
            this.pnlHeaderRight.Controls.Add(this.label39);
            this.pnlHeaderRight.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeaderRight.Location = new System.Drawing.Point(0, 0);
            this.pnlHeaderRight.Margin = new System.Windows.Forms.Padding(2);
            this.pnlHeaderRight.Name = "pnlHeaderRight";
            this.pnlHeaderRight.Size = new System.Drawing.Size(1751, 25);
            this.pnlHeaderRight.TabIndex = 18;
            // 
            // lblTitleChart
            // 
            this.lblTitleChart.AutoSize = true;
            this.lblTitleChart.Font = new System.Drawing.Font("MS Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitleChart.Location = new System.Drawing.Point(16, 3);
            this.lblTitleChart.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTitleChart.Name = "lblTitleChart";
            this.lblTitleChart.Size = new System.Drawing.Size(92, 15);
            this.lblTitleChart.TabIndex = 2;
            this.lblTitleChart.Text = "グラフ表示";
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(1904, 1041);
            this.Controls.Add(this.splMainContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BO監視グラフ";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.splMainContainer.Panel1.ResumeLayout(false);
            this.splMainContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splMainContainer)).EndInit();
            this.splMainContainer.ResumeLayout(false);
            this.pnlHeaderLeft.ResumeLayout(false);
            this.pnlHeaderLeft.PerformLayout();
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtmM)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tblLayoutLeft.ResumeLayout(false);
            this.tblLayoutTopLeft.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel32.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chrEastBO)).EndInit();
            this.pnlBoHeaderEast.ResumeLayout(false);
            this.pnlBoHeaderEast.PerformLayout();
            this.tblLayoutBottomLeft.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chrEastRateTempChange)).EndInit();
            this.panel45.ResumeLayout(false);
            this.panel45.PerformLayout();
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chrEastElectrodeTemp)).EndInit();
            this.panel36.ResumeLayout(false);
            this.panel36.PerformLayout();
            this.tblLayoutRight.ResumeLayout(false);
            this.tblLayoutTopRight.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel19.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chrWesternBO)).EndInit();
            this.pnlBoHeaderWestern.ResumeLayout(false);
            this.pnlBoHeaderWestern.PerformLayout();
            this.tblLayoutBottomRight.ResumeLayout(false);
            this.panel11.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chrWesternElectrodeTemp)).EndInit();
            this.panel22.ResumeLayout(false);
            this.panel22.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart13)).EndInit();
            this.panel12.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chrWesternRateTempChange)).EndInit();
            this.panel49.ResumeLayout(false);
            this.panel49.PerformLayout();
            this.pnlHeaderRight.ResumeLayout(false);
            this.pnlHeaderRight.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.CheckBox chbRealTime;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.Button btnLoadChart;
        private System.Windows.Forms.SplitContainer splMainContainer;
        private System.Windows.Forms.Panel pnlFilter;
        private System.Windows.Forms.Label lblRealTime;
        private System.Windows.Forms.Panel pnlHeaderLeft;
        private System.Windows.Forms.Panel pnlHeaderRight;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.Label lblTitleChart;
        private System.Windows.Forms.Label lblTitleMenu;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.Label lblFilter;
        private System.Windows.Forms.Label lblLength;
        private System.Windows.Forms.Button btnViewHistory;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tblLayoutRight;
        private System.Windows.Forms.TableLayoutPanel tblLayoutTopRight;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel19;
        private System.Windows.Forms.DataVisualization.Charting.Chart chrWesternBO;
        private System.Windows.Forms.Panel pnlBoHeaderWestern;
        private System.Windows.Forms.Label lblTitleWesternBO;
        private System.Windows.Forms.Label lblWesternColorOrange;
        private System.Windows.Forms.Label lblBOWesternTitle;
        private System.Windows.Forms.Label lblWesternColorBlue;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label lblWesternColorWhite;
        private System.Windows.Forms.Label lblWesternColorRed;
        private System.Windows.Forms.TableLayoutPanel tblLayoutBottomRight;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.DataVisualization.Charting.Chart chrWesternElectrodeTemp;
        private System.Windows.Forms.Panel panel22;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart13;
        private System.Windows.Forms.Panel panel12;
        private System.Windows.Forms.DataVisualization.Charting.Chart chrWesternRateTempChange;
        private System.Windows.Forms.Panel panel24;
        private System.Windows.Forms.TableLayoutPanel tblLayoutLeft;
        private System.Windows.Forms.TableLayoutPanel tblLayoutTopLeft;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel32;
        private System.Windows.Forms.DataVisualization.Charting.Chart chrEastBO;
        private System.Windows.Forms.Panel pnlBoHeaderEast;
        private System.Windows.Forms.Label lblTitleeastBO;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label lblEastColorOrange;
        private System.Windows.Forms.Label lblBoLeftTitle;
        private System.Windows.Forms.Label lblEastColorBlue;
        private System.Windows.Forms.Label lblEastOrangeTitle;
        private System.Windows.Forms.Label lblEastColorWhite;
        private System.Windows.Forms.Label lblEastColorRed;
        private System.Windows.Forms.TableLayoutPanel tblLayoutBottomLeft;
        private System.Windows.Forms.DataVisualization.Charting.Chart chrEastElectrodeTemp;
        private System.Windows.Forms.Panel panel36;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.DataVisualization.Charting.Chart chrEastRateTempChange;
        private System.Windows.Forms.Panel panel38;
        private System.Windows.Forms.Label lblEastRedTitle;
        private System.Windows.Forms.Label lblEastWhiteTitle;
        private System.Windows.Forms.Label lblEastBlueTitle;
        private System.Windows.Forms.Label lblWesternBlueTitle;
        private System.Windows.Forms.Label lblWesternOrangeTitle;
        private System.Windows.Forms.Label lblWesternWhiteTitle;
        private System.Windows.Forms.Label lblWesternRedTitle;
        private System.Windows.Forms.Panel panel49;
        private System.Windows.Forms.Label label57;
        private System.Windows.Forms.NumericUpDown txtmM;
        private System.Windows.Forms.Panel panel45;
        private System.Windows.Forms.Label label58;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel5;
    }
}

