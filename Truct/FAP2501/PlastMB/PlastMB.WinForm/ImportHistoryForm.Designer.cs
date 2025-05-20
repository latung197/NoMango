using System.Drawing;
using System.Windows.Forms;

namespace PlastMB
{
    partial class ImportHistoryForm
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportHistoryForm));
            titleBar1 = new PlastMB.UserControls.TitleBar();
            faButton1 = new PlastMB.UserControls.FAButton();
            grvHistoryDetail = new DataGridView();
            transparentPanel1 = new PlastMB.UserControls.TransparentPanel();
            faButton3 = new PlastMB.UserControls.FAButton();
            loadingBox1 = new PlastMB.UserControls.LoadingBox();
            grvHistoryFile = new DataGridView();
            faButton2 = new PlastMB.UserControls.FAButton();
            txtFileName = new TextBox();
            txtMachine = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            dtpdFrom = new DateTimePicker();
            dtpdTo = new DateTimePicker();
            label4 = new Label();
            label5 = new Label();
            ((System.ComponentModel.ISupportInitialize)grvHistoryDetail).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grvHistoryFile).BeginInit();
            SuspendLayout();
            // 
            // titleBar1
            // 
            titleBar1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            titleBar1.BackColor = Color.FromArgb(240, 244, 249);
            titleBar1.Location = new Point(0, 0);
            titleBar1.Name = "titleBar1";
            //titleBar1.Size = new Size(1024, 48);
            titleBar1.TabIndex = 0;
            titleBar1.Title = "履歴画面";
            titleBar1.CloseClicked += titleBar1_CloseClicked;
            titleBar1.MouseDowned += titleBar1_MouseDowned;
            // 
            // faButton1
            // 
            faButton1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            faButton1.ColorBottom = Color.FromArgb(255, 190, 135);
            faButton1.ColorTop = Color.FromArgb(255, 235, 218);
            faButton1.Cursor = Cursors.Hand;
            faButton1.FlatAppearance.BorderSize = 0;
            faButton1.FlatStyle = FlatStyle.Flat;
            faButton1.Font = new Font("Arial", 11F, FontStyle.Bold);
            faButton1.Location = new Point(870, 134);
            faButton1.Name = "faButton1";
            faButton1.Size = new Size(91, 38);
            faButton1.TabIndex = 1;
            faButton1.Text = "検索";
            faButton1.UseVisualStyleBackColor = true;
            faButton1.Click += faButton1_Click;
            // 
            // grvHistoryDetail
            // 
            grvHistoryDetail.AllowUserToAddRows = false;
            grvHistoryDetail.AllowUserToDeleteRows = false;
            grvHistoryDetail.AllowUserToResizeColumns = false;
            grvHistoryDetail.AllowUserToResizeRows = false;
            grvHistoryDetail.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grvHistoryDetail.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grvHistoryDetail.BackgroundColor = SystemColors.Window;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            grvHistoryDetail.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            grvHistoryDetail.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grvHistoryDetail.Location = new Point(521, 178);
            grvHistoryDetail.Name = "grvHistoryDetail";
            grvHistoryDetail.ReadOnly = true;
            grvHistoryDetail.RowHeadersVisible = false;
            grvHistoryDetail.RowTemplate.Height = 30;
            grvHistoryDetail.ScrollBars = ScrollBars.Vertical;
            grvHistoryDetail.Size = new Size(489, 357);
            grvHistoryDetail.TabIndex = 3;
            grvHistoryDetail.CellClick += dataGridView1_CellClick;
            grvHistoryDetail.CellMouseEnter += dataGridView1_CellMouseEnter;
            // 
            // transparentPanel1
            // 
            transparentPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            transparentPanel1.BackColor = Color.Transparent;
            transparentPanel1.Location = new Point(0, 55);
            transparentPanel1.Name = "transparentPanel1";
            transparentPanel1.Size = new Size(1024, 545);
            transparentPanel1.TabIndex = 5;
            transparentPanel1.Visible = false;
            // 
            // faButton3
            // 
            faButton3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            faButton3.ColorBottom = Color.FromArgb(255, 190, 135);
            faButton3.ColorTop = Color.FromArgb(255, 235, 218);
            faButton3.FlatAppearance.BorderSize = 0;
            faButton3.FlatStyle = FlatStyle.Flat;
            faButton3.Font = new Font("Arial", 11F, FontStyle.Bold);
            faButton3.Location = new Point(17, 550);
            faButton3.Name = "faButton3";
            faButton3.Size = new Size(140, 38);
            faButton3.TabIndex = 0;
            faButton3.Text = "戻る";
            faButton3.UseVisualStyleBackColor = true;
            faButton3.Click += faButton3_Click;
            // 
            // loadingBox1
            // 
            loadingBox1.Anchor = AnchorStyles.None;
            loadingBox1.BackColor = Color.Transparent;
            loadingBox1.BorderStyle = BorderStyle.FixedSingle;
            loadingBox1.Location = new Point(151, 331);
            loadingBox1.Name = "loadingBox1";
            loadingBox1.Size = new Size(200, 100);
            loadingBox1.TabIndex = 6;
            loadingBox1.Title = "データ取得中...";
            // 
            // grvHistoryFile
            // 
            grvHistoryFile.AllowUserToAddRows = false;
            grvHistoryFile.AllowUserToDeleteRows = false;
            grvHistoryFile.AllowUserToResizeColumns = false;
            grvHistoryFile.AllowUserToResizeRows = false;
            grvHistoryFile.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            grvHistoryFile.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grvHistoryFile.BackgroundColor = SystemColors.Window;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = SystemColors.Control;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            grvHistoryFile.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            grvHistoryFile.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grvHistoryFile.Location = new Point(17, 178);
            grvHistoryFile.Name = "grvHistoryFile";
            grvHistoryFile.ReadOnly = true;
            grvHistoryFile.RowHeadersVisible = false;
            grvHistoryFile.RowTemplate.Height = 30;
            grvHistoryFile.ScrollBars = ScrollBars.Vertical;
            grvHistoryFile.Size = new Size(498, 357);
            grvHistoryFile.TabIndex = 7;
            grvHistoryFile.CellClick += grvHistoryFile_CellClick;
            // 
            // faButton2
            // 
            faButton2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            faButton2.ColorBottom = Color.FromArgb(255, 190, 135);
            faButton2.ColorTop = Color.FromArgb(255, 235, 218);
            faButton2.Cursor = Cursors.Hand;
            faButton2.FlatAppearance.BorderSize = 0;
            faButton2.FlatStyle = FlatStyle.Flat;
            faButton2.Font = new Font("Arial", 11F, FontStyle.Bold);
            faButton2.Location = new Point(731, 134);
            faButton2.Name = "faButton2";
            faButton2.Size = new Size(94, 38);
            faButton2.TabIndex = 4;
            faButton2.Text = "クリア";
            faButton2.UseVisualStyleBackColor = true;
            faButton2.Click += faButton2_Click;
            // 
            // txtFileName
            // 
            txtFileName.Location = new Point(89, 121);
            txtFileName.Name = "txtFileName";
            txtFileName.Size = new Size(273, 23);
            txtFileName.TabIndex = 9;
            // 
            // txtMachine
            // 
            txtMachine.Location = new Point(616, 79);
            txtMachine.Name = "txtMachine";
            txtMachine.Size = new Size(276, 23);
            txtMachine.TabIndex = 10;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(21, 82);
            label1.Name = "label1";
            label1.Size = new Size(39, 15);
            label1.TabIndex = 11;
            label1.Text = "日付 :";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(21, 125);
            label2.Name = "label2";
            label2.Size = new Size(52, 15);
            label2.TabIndex = 12;
            label2.Text = "ファイ名 :";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(548, 82);
            label3.Name = "label3";
            label3.Size = new Size(55, 15);
            label3.TabIndex = 13;
            label3.Text = "機械名 : ";
            // 
            // dtpdFrom
            // 
            dtpdFrom.CustomFormat = "yyyy/MM/dd";
            dtpdFrom.Format = DateTimePickerFormat.Custom;
            dtpdFrom.Location = new Point(89, 79);
            dtpdFrom.Name = "dtpdFrom";
            dtpdFrom.Size = new Size(125, 23);
            dtpdFrom.TabIndex = 14;
            // 
            // dtpdTo
            // 
            dtpdTo.CustomFormat = "yyyy/MM/dd";
            dtpdTo.Format = DateTimePickerFormat.Custom;
            dtpdTo.Location = new Point(237, 79);
            dtpdTo.Name = "dtpdTo";
            dtpdTo.Size = new Size(125, 23);
            dtpdTo.TabIndex = 15;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(21, 160);
            label4.Name = "label4";
            label4.Size = new Size(70, 15);
            label4.TabIndex = 16;
            label4.Text = "ファイル一覧";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(521, 160);
            label5.Name = "label5";
            label5.Size = new Size(64, 15);
            label5.TabIndex = 17;
            label5.Text = "データ一覧";
            // 
            // ImportHistoryForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1024, 600);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(dtpdTo);
            Controls.Add(dtpdFrom);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtMachine);
            Controls.Add(txtFileName);
            Controls.Add(loadingBox1);
            Controls.Add(grvHistoryFile);
            Controls.Add(faButton3);
            Controls.Add(faButton2);
            Controls.Add(grvHistoryDetail);
            Controls.Add(faButton1);
            Controls.Add(titleBar1);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(1024, 600);
            Name = "ImportHistoryForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "履歴画面";
            WindowState = FormWindowState.Maximized;
            FormClosed += HAForm_FormClosed;
            Load += ImportHistoryForm_Load;
            ((System.ComponentModel.ISupportInitialize)grvHistoryDetail).EndInit();
            ((System.ComponentModel.ISupportInitialize)grvHistoryFile).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private UserControls.TitleBar titleBar1;
        private UserControls.FAButton faButton1;
        private DataGridView grvHistoryDetail;
        private UserControls.FAButton faButton3;
        private UserControls.TransparentPanel transparentPanel1;
        private UserControls.LoadingBox loadingBox1;
        private DataGridView grvHistoryFile;
        private UserControls.FAButton faButton2;
        private TextBox txtFileName;
        private TextBox txtMachine;
        private Label label1;
        private Label label2;
        private Label label3;
        private DateTimePicker dtpdFrom;
        private DateTimePicker dtpdTo;
        private Label label4;
        private Label label5;
    }
}