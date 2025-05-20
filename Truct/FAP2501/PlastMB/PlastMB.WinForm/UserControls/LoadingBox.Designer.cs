using System.Drawing;
using System.Windows.Forms;

namespace PlastMB.UserControls
{
partial class LoadingBox
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        panel1 = new Panel();
        pictureBox1 = new PictureBox();
        label1 = new Label();
        panel1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
        SuspendLayout();
        // 
        // panel1
        // 
        panel1.Anchor = AnchorStyles.None;
        panel1.Controls.Add(label1);
        panel1.Controls.Add(pictureBox1);
        panel1.Location = new Point(50, 50);
        panel1.Name = "panel1";
        panel1.Size = new Size(100, 100);
        panel1.TabIndex = 0;
        // 
        // pictureBox1
        // 
        pictureBox1.Image = Properties.Resources.progress;
        pictureBox1.Location = new Point(34, 15);
        pictureBox1.Name = "pictureBox1";
        pictureBox1.Size = new Size(32, 32);
        pictureBox1.TabIndex = 0;
        pictureBox1.TabStop = false;
        // 
        // label1
        // 
        label1.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
        label1.Location = new Point(0, 66);
        label1.Name = "label1";
        label1.Size = new Size(100, 15);
        label1.TabIndex = 1;
        label1.Text = "Đang tải...";
        label1.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // LoadingBox
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.Transparent;
        Controls.Add(panel1);
        Name = "LoadingBox";
        Size = new Size(200, 200);
        panel1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private Panel panel1;
    private PictureBox pictureBox1;
    private Label label1;
}
}