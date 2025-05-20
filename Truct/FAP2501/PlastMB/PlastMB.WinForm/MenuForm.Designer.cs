using System.Drawing;
using System.Windows.Forms;

namespace PlastMB
{
partial class MenuForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MenuForm));
            tableLayoutPanel1 = new TableLayoutPanel();
            menuButton1 = new UserControls.MenuButton();
            menuButton2 = new UserControls.MenuButton();
            label1 = new Label();
            titleBar1 = new UserControls.TitleBar();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 5;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 6F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 41F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 6F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 41F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 6F));
            tableLayoutPanel1.Controls.Add(menuButton1, 1, 1);
            tableLayoutPanel1.Controls.Add(menuButton2, 3, 1);
            tableLayoutPanel1.Controls.Add(label1, 4, 4);
            tableLayoutPanel1.Location = new Point(0, 48);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 5;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            tableLayoutPanel1.Size = new Size(1024, 552);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // menuButton1
            // 
            menuButton1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            menuButton1.BorderColor = Color.DarkOrange;
            menuButton1.BorderWidth = 3F;
            menuButton1.ColorBottom = Color.FromArgb(255, 190, 135);
            menuButton1.ColorTop = Color.FromArgb(255, 235, 218);
            menuButton1.Cursor = Cursors.Hand;
            menuButton1.Location = new Point(167, 85);
            menuButton1.Name = "menuButton1";
            menuButton1.Radius = 20;
            menuButton1.Size = new Size(310, 159);
            menuButton1.TabIndex = 0;
            menuButton1.Title = "設定画面";
            menuButton1.Click += menuButton1_Click;
            // 
            // menuButton2
            // 
            menuButton2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            menuButton2.BorderColor = Color.DarkOrange;
            menuButton2.BorderWidth = 3F;
            menuButton2.ColorBottom = Color.FromArgb(255, 190, 135);
            menuButton2.ColorTop = Color.FromArgb(255, 235, 218);
            menuButton2.Cursor = Cursors.Hand;
            menuButton2.Location = new Point(544, 85);
            menuButton2.Name = "menuButton2";
            menuButton2.Radius = 20;
            menuButton2.Size = new Size(310, 159);
            menuButton2.TabIndex = 1;
            menuButton2.Title = "履歴画面";
            menuButton2.Click += menuButton2_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(974, 534);
            label1.Margin = new Padding(3);
            label1.Name = "label1";
            label1.Size = new Size(47, 15);
            label1.TabIndex = 2;
            label1.Text = "ver1.0.1";
            // 
            // titleBar1
            // 
            titleBar1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            titleBar1.BackColor = Color.FromArgb(240, 244, 249);
            titleBar1.Location = new Point(0, 0);
            titleBar1.Name = "titleBar1";
            //titleBar1.Size = new Size(1024, 48);
            titleBar1.TabIndex = 0;
            titleBar1.Title = "メニュー画面";
            titleBar1.CloseClicked += titleBar1_CloseClicked;
            titleBar1.MouseDowned += titleBar1_MouseDowned;
            // 
            // MenuForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1024, 600);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(titleBar1);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MenuForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "メニュー画面";
            FormClosing += MenuForm_FormClosing;
            VisibleChanged += MenuForm_VisibleChanged;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private TableLayoutPanel tableLayoutPanel1;
        private UserControls.MenuButton menuButton1;
        private UserControls.MenuButton menuButton2;
        private Label label1;
        private UserControls.TitleBar titleBar1;
    }
}