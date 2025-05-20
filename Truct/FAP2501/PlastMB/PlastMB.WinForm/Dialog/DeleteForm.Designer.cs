using System.Drawing;
using System.Windows.Forms;

namespace PlastMB.Dialog
{
    partial class DeleteForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeleteForm));
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
            faButton1.Location = new Point(321, 284);
            faButton1.Margin = new Padding(3, 4, 3, 4);
            faButton1.Name = "faButton1";
            faButton1.Size = new Size(95, 40);
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
            faButton2.Location = new Point(454, 284);
            faButton2.Margin = new Padding(3, 4, 3, 4);
            faButton2.Name = "faButton2";
            faButton2.Size = new Size(95, 40);
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
            label1.Location = new Point(34, 180);
            label1.Name = "label1";
            label1.Size = new Size(248, 30);
            label1.TabIndex = 3;
            label1.Text = "Mật khẩu không đúng.";
            label1.Visible = false;
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBox1.Location = new Point(34, 120);
            textBox1.Margin = new Padding(3, 4, 3, 4);
            textBox1.Name = "textBox1";
            textBox1.PasswordChar = '*';
            textBox1.Size = new Size(514, 27);
            textBox1.TabIndex = 4;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // DeleteForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(582, 348);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Controls.Add(faButton2);
            Controls.Add(faButton1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 4, 3, 4);
            Name = "DeleteForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Nhập mật khẩu để xóa tệp";
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