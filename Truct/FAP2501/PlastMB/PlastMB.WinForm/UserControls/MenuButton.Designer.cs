using System.Drawing;
using System.Windows.Forms;

namespace PlastMB.UserControls
{
partial class MenuButton
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
        label1 = new Label();
        infoBadge1 = new PlastMB.UserControls.InfoBadge();
        SuspendLayout();
        // 
        // label1
        // 
        label1.BackColor = Color.Transparent;
        label1.Font = new Font("Segoe UI", 19F, FontStyle.Regular, GraphicsUnit.Point);
        label1.Location = new Point(37, 51);
        label1.Name = "label1";
        label1.Size = new Size(247, 72);
        label1.TabIndex = 0;
        label1.Text = "Path Form";
        label1.TextAlign = ContentAlignment.MiddleCenter;
        label1.Click += label1_Click;
        label1.MouseLeave += label1_MouseLeave;
        label1.MouseHover += label1_MouseHover;
        // 
        // infoBadge1
        // 
        infoBadge1.BackColor = Color.Transparent;
        infoBadge1.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point);
        infoBadge1.ForeColor = Color.White;
        infoBadge1.Location = new Point(265, -6);
        infoBadge1.Name = "infoBadge1";
        infoBadge1.Size = new Size(50, 40);
        infoBadge1.TabIndex = 1;
        infoBadge1.Text = "0";
        infoBadge1.Visible = false;
        // 
        // MenuButton
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(infoBadge1);
        Controls.Add(label1);
        Cursor = Cursors.Hand;
        Name = "MenuButton";
        Size = new Size(310, 160);
        Load += MenuButton_Load;
        MouseLeave += MenuButton_MouseLeave;
        MouseHover += MenuButton_MouseHover;
        ResumeLayout(false);
    }

    #endregion

    private Label label1;
    private PlastMB.UserControls.InfoBadge infoBadge1;
}
}