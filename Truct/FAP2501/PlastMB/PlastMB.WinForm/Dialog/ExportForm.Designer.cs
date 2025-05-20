using System.Drawing;
using System.Windows.Forms;

namespace PlastMB.Dialog
{
partial class ExportForm
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportForm));
        dataGridView1 = new DataGridView();
        faButton1 = new UserControls.FAButton();
        ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
        SuspendLayout();
        // 
        // dataGridView1
        // 
        dataGridView1.AllowUserToAddRows = false;
        dataGridView1.AllowUserToDeleteRows = false;
        dataGridView1.AllowUserToResizeColumns = false;
        dataGridView1.AllowUserToResizeRows = false;
        dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        dataGridView1.BackgroundColor = SystemColors.Window;
        dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
        dataGridViewCellStyle1.BackColor = SystemColors.Window;
        dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        dataGridViewCellStyle1.ForeColor = SystemColors.ControlText;
        dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
        dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
        dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
        dataGridView1.DefaultCellStyle = dataGridViewCellStyle1;
        dataGridView1.Location = new Point(30, 30);
        dataGridView1.Name = "dataGridView1";
        dataGridView1.ReadOnly = true;
        dataGridView1.RowHeadersVisible = false;
        dataGridView1.RowTemplate.Height = 25;
        dataGridView1.ScrollBars = ScrollBars.Vertical;
        dataGridView1.Size = new Size(1291, 633);
        dataGridView1.TabIndex = 0;
        // 
        // faButton1
        // 
        faButton1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        faButton1.ColorBottom = Color.FromArgb(255, 190, 135);
        faButton1.ColorTop = Color.FromArgb(255, 235, 218);
        faButton1.Cursor = Cursors.Hand;
        faButton1.FlatAppearance.BorderSize = 0;
        faButton1.FlatStyle = FlatStyle.Flat;
        faButton1.Font = new Font("Arial", 11F, FontStyle.Bold, GraphicsUnit.Point);
        faButton1.Location = new Point(1238, 681);
        faButton1.Name = "faButton1";
        faButton1.Size = new Size(83, 30);
        faButton1.TabIndex = 1;
        faButton1.Text = "OK";
        faButton1.UseVisualStyleBackColor = true;
        faButton1.Click += faButton1_Click;
        // 
        // ExportForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1350, 729);
        Controls.Add(faButton1);
        Controls.Add(dataGridView1);
        Icon = (Icon)resources.GetObject("$this.Icon");
        MinimizeBox = false;
        Name = "ExportForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "ExportForm";
        Load += ExportForm_Load;
        ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private DataGridView dataGridView1;
    private UserControls.FAButton faButton1;
}
}