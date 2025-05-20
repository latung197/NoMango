using System.Drawing;
using System.Windows.Forms;

namespace PlastMB.Dialog
{
    partial class EditForm
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(EditForm));
            faButton1 = new UserControls.FAButton();
            faButton2 = new UserControls.FAButton();
            label1 = new Label();
            textBox1 = new TextBox();
            SuspendLayout();
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
            faButton1.Location = new Point(556, 213);
            faButton1.Name = "faButton1";
            faButton1.Size = new Size(83, 30);
            faButton1.TabIndex = 1;
            faButton1.Text = "OK";
            faButton1.UseVisualStyleBackColor = true;
            faButton1.Click += faButton1_Click;
            // 
            // faButton2
            // 
            faButton2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            faButton2.ColorBottom = Color.FromArgb(255, 190, 135);
            faButton2.ColorTop = Color.FromArgb(255, 235, 218);
            faButton2.Cursor = Cursors.Hand;
            faButton2.FlatAppearance.BorderSize = 0;
            faButton2.FlatStyle = FlatStyle.Flat;
            faButton2.Font = new Font("Arial", 11F, FontStyle.Bold, GraphicsUnit.Point);
            faButton2.Location = new Point(672, 213);
            faButton2.Name = "faButton2";
            faButton2.Size = new Size(83, 30);
            faButton2.TabIndex = 2;
            faButton2.Text = "Cancel";
            faButton2.UseVisualStyleBackColor = true;
            faButton2.Click += faButton2_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 13F, FontStyle.Bold, GraphicsUnit.Point);
            label1.ForeColor = Color.Red;
            label1.Location = new Point(30, 213);
            label1.Name = "label1";
            label1.Size = new Size(344, 25);
            label1.TabIndex = 3;
            label1.Text = "Tệp CSV có tên không đúng định dạng.";
            label1.Visible = false;
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBox1.Location = new Point(30, 90);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(725, 23);
            textBox1.TabIndex = 4;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // EditForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 261);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Controls.Add(faButton2);
            Controls.Add(faButton1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "EditForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Đổi tên tệp CSV";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private UserControls.FAButton faButton1;
        private UserControls.FAButton faButton2;
        private Label label1;
        private TextBox textBox1;
    }
}