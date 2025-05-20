using System.Drawing;
using System.Windows.Forms;

namespace PlastMB
{
    partial class SettingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingForm));
            titleBar1 = new UserControls.TitleBar();
            tableLayoutPanel1 = new TableLayoutPanel();
            label1 = new Label();
            textBox1 = new TextBox();
            label2 = new Label();
            panel1 = new Panel();
            label3 = new Label();
            nrd_TimeReadCsv = new NumericUpDown();
            faButton1 = new UserControls.FAButton();
            faButton2 = new UserControls.FAButton();
            tableLayoutPanel1.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nrd_TimeReadCsv).BeginInit();
            SuspendLayout();
            // 
            // titleBar1
            // 
            titleBar1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            titleBar1.BackColor = Color.FromArgb(240, 244, 249);
            titleBar1.Location = new Point(0, 0);
            titleBar1.Name = "titleBar1";
            titleBar1.Size = new Size(1024, 48);
            titleBar1.TabIndex = 0;
            titleBar1.Title = "設定画面";
            titleBar1.CloseClicked += titleBar1_CloseClicked;
            titleBar1.MouseDowned += titleBar1_MouseDowned;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 5;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11.04053F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 31.8616676F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 4.37784F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42.79805F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 9.921916F));
            tableLayoutPanel1.Controls.Add(label1, 1, 0);
            tableLayoutPanel1.Controls.Add(textBox1, 3, 0);
            tableLayoutPanel1.Controls.Add(label2, 1, 1);
            tableLayoutPanel1.Controls.Add(panel1, 3, 1);
            tableLayoutPanel1.Location = new Point(0, 75);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 6;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 16.664999F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 16.664999F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 16.664999F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 16.664999F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 16.664999F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6750031F));
            tableLayoutPanel1.Size = new Size(1016, 434);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(115, 25);
            label1.Name = "label1";
            label1.Size = new Size(317, 21);
            label1.TabIndex = 0;
            label1.Text = "フォルダ設定：";
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBox1.BackColor = SystemColors.Control;
            textBox1.Cursor = Cursors.Hand;
            textBox1.Font = new Font("Segoe UI", 14.25F);
            textBox1.Location = new Point(482, 19);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(428, 33);
            textBox1.TabIndex = 4;
            textBox1.Text = "。。。";
            textBox1.TextAlign = HorizontalAlignment.Right;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(115, 97);
            label2.Name = "label2";
            label2.Size = new Size(317, 21);
            label2.TabIndex = 5;
            label2.Text = "監視インタバル：";
            // 
            // panel1
            // 
            panel1.Controls.Add(label3);
            panel1.Controls.Add(nrd_TimeReadCsv);
            panel1.Location = new Point(482, 75);
            panel1.Name = "panel1";
            panel1.Size = new Size(273, 66);
            panel1.TabIndex = 6;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(172, 20);
            label3.Name = "label3";
            label3.Size = new Size(26, 21);
            label3.TabIndex = 7;
            label3.Text = "分";
            // 
            // nrd_TimeReadCsv
            // 
            nrd_TimeReadCsv.Location = new Point(0, 20);
            nrd_TimeReadCsv.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            nrd_TimeReadCsv.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nrd_TimeReadCsv.Name = "nrd_TimeReadCsv";
            nrd_TimeReadCsv.Size = new Size(165, 23);
            nrd_TimeReadCsv.TabIndex = 0;
            nrd_TimeReadCsv.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // faButton1
            // 
            faButton1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            faButton1.ColorBottom = Color.FromArgb(255, 190, 135);
            faButton1.ColorTop = Color.FromArgb(255, 235, 218);
            faButton1.FlatAppearance.BorderSize = 0;
            faButton1.FlatStyle = FlatStyle.Flat;
            faButton1.Font = new Font("Arial", 11F, FontStyle.Bold);
            faButton1.Location = new Point(561, 515);
            faButton1.Name = "faButton1";
            faButton1.Size = new Size(140, 38);
            faButton1.TabIndex = 2;
            faButton1.Text = "戻る";
            faButton1.UseVisualStyleBackColor = true;
            faButton1.Click += button1_Click;
            // 
            // faButton2
            // 
            faButton2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            faButton2.ColorBottom = Color.FromArgb(255, 190, 135);
            faButton2.ColorTop = Color.FromArgb(255, 235, 218);
            faButton2.FlatAppearance.BorderSize = 0;
            faButton2.FlatStyle = FlatStyle.Flat;
            faButton2.Font = new Font("Arial", 11F, FontStyle.Bold);
            faButton2.Location = new Point(770, 515);
            faButton2.Name = "faButton2";
            faButton2.Size = new Size(140, 38);
            faButton2.TabIndex = 3;
            faButton2.Text = "保存";
            faButton2.UseVisualStyleBackColor = true;
            faButton2.Click += button2_Click;
            // 
            // SettingForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1024, 600);
            Controls.Add(faButton2);
            Controls.Add(faButton1);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(titleBar1);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(1024, 600);
            Name = "SettingForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "設定画面";
            WindowState = FormWindowState.Maximized;
            FormClosed += SettingForm_FormClosed;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nrd_TimeReadCsv).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private UserControls.TitleBar titleBar1;
        private TableLayoutPanel tableLayoutPanel1;
        private Label label1;
        private TextBox textBox1;
        private UserControls.FAButton faButton1;
        private UserControls.FAButton faButton2;
        private Label label2;
        private Panel panel1;
        private NumericUpDown nrd_TimeReadCsv;
        private Label label3;
    }
}