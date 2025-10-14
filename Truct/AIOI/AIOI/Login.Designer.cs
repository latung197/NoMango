using System.Drawing;
using System.Windows.Forms;

namespace AIOI
{
    partial class Login
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnExit;
        private CheckBox chkShowPassword;
        private LinkLabel lnkForgotPassword;
        private Label label1;
        private Label label2;
        private Panel panel1;
        private Label lblTitle;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panel1 = new Panel();
            this.lblTitle = new Label();
            this.label1 = new Label();
            this.label2 = new Label();
            this.txtUsername = new TextBox();
            this.txtPassword = new TextBox();
            this.chkShowPassword = new CheckBox();
            this.lnkForgotPassword = new LinkLabel();
            this.btnLogin = new Button();
            this.btnExit = new Button();

            this.SuspendLayout();

            // Form Login
            this.ClientSize = new System.Drawing.Size(400, 350);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Đăng nhập Hệ thống ERP";

            // panel1
            this.panel1.Dock = DockStyle.Fill;
            this.panel1.BackColor = Color.White;

            // lblTitle
            this.lblTitle.Text = "HỆ THỐNG ERP";
            this.lblTitle.Font = new Font("Arial", 16, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.FromArgb(0, 102, 204);
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new Point(120, 30);

            // label1
            this.label1.Text = "Tên đăng nhập:";
            this.label1.Location = new Point(50, 100);
            this.label1.Size = new Size(100, 20);

            // txtUsername
            this.txtUsername.Location = new Point(150, 97);
            this.txtUsername.Size = new Size(200, 25);

            // label2
            this.label2.Text = "Mật khẩu:";
            this.label2.Location = new Point(50, 150);
            this.label2.Size = new Size(100, 20);

            // txtPassword
            this.txtPassword.Location = new Point(150, 147);
            this.txtPassword.Size = new Size(200, 25);
            this.txtPassword.UseSystemPasswordChar = true;
            this.txtPassword.KeyPress += new KeyPressEventHandler(this.txtPassword_KeyPress);

            // chkShowPassword
            this.chkShowPassword.Text = "Hiện mật khẩu";
            this.chkShowPassword.Location = new Point(150, 180);
            this.chkShowPassword.CheckedChanged += new System.EventHandler(this.chkShowPassword_CheckedChanged);

            // lnkForgotPassword
            this.lnkForgotPassword.Text = "Quên mật khẩu?";
            this.lnkForgotPassword.Location = new Point(270, 180);
            this.lnkForgotPassword.LinkClicked += new LinkLabelLinkClickedEventHandler(this.lnkForgotPassword_LinkClicked);

            // btnLogin
            this.btnLogin.Text = "ĐĂNG NHẬP";
            this.btnLogin.Location = new Point(100, 230);
            this.btnLogin.Size = new Size(120, 35);
            this.btnLogin.BackColor = Color.FromArgb(0, 102, 204);
            this.btnLogin.ForeColor = Color.White;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);

            // btnExit
            this.btnExit.Text = "THOÁT";
            this.btnExit.Location = new Point(230, 230);
            this.btnExit.Size = new Size(120, 35);
            this.btnExit.BackColor = Color.FromArgb(204, 0, 0);
            this.btnExit.ForeColor = Color.White;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);

            // Add controls
            this.panel1.Controls.Add(this.lblTitle);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtUsername);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txtPassword);
            this.panel1.Controls.Add(this.chkShowPassword);
            this.panel1.Controls.Add(this.lnkForgotPassword);
            this.panel1.Controls.Add(this.btnLogin);
            this.panel1.Controls.Add(this.btnExit);

            this.Controls.Add(this.panel1);

            this.ResumeLayout(false);
        }
    }
}
