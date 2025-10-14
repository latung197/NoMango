using System;
using System.Drawing;
using System.Windows.Forms;

namespace AIOI
{
    public partial class MainMenuForm : Form
    {
        private bool isCollapsed = false;

        public MainMenuForm()
        {
            InitializeComponent();
        }

        private void MainMenuForm_Load(object sender, EventArgs e)
        {
            ShowDashboard();
        }

        private void ShowDashboard()
        {
            lblTitle.Text = "DASHBOARD";
            pnlContent.Controls.Clear();

            Label lbl = new Label
            {
                Text = "Chào mừng đến với hệ thống!",
                AutoSize = true,
                Location = new Point(50, 50),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };
            pnlContent.Controls.Add(lbl);
        }

        private void btnDashboard_Click(object sender, EventArgs e) => ShowDashboard();
        private void btnQuanLyNhanVien_Click(object sender, EventArgs e) => lblTitle.Text = "QUẢN LÝ NHÂN VIÊN";
        private void btnQuanLySanPham_Click(object sender, EventArgs e) => lblTitle.Text = "QUẢN LÝ SẢN PHẨM";
        private void btnHoaDon_Click(object sender, EventArgs e) => lblTitle.Text = "QUẢN LÝ HÓA ĐƠN";
        private void btnBaoCao_Click(object sender, EventArgs e) => lblTitle.Text = "BÁO CÁO THỐNG KÊ";
        private void btnCaiDat_Click(object sender, EventArgs e) => lblTitle.Text = "CÀI ĐẶT HỆ THỐNG";
        private void btnThoat_Click(object sender, EventArgs e) => Application.Exit();
        private void btnClose_Click(object sender, EventArgs e) => this.Close();
        private void btnToggle_Click(object sender, EventArgs e)
        {
            if (isCollapsed)
            {
                // mở rộng
                pnlMenu.Width = 220;
                foreach (Control ctrl in pnlMenu.Controls)
                {
                    if (ctrl is Button btn && btn.Tag != null)
                    {
                        btn.Text = btn.Tag.ToString();
                        btn.TextAlign = ContentAlignment.MiddleLeft;
                        btn.ImageAlign = ContentAlignment.MiddleLeft;
                        btn.Padding = new Padding(10, 0, 0, 0);
                    }
                }
                isCollapsed = false;
            }
            else
            {
                // thu gọn
                pnlMenu.Width = 60;
                foreach (Control ctrl in pnlMenu.Controls)
                {
                    if (ctrl is Button btn)
                    {
                        btn.Tag = btn.Text;
                        btn.Text = "";
                        btn.ImageAlign = ContentAlignment.MiddleCenter;
                    }
                }
                isCollapsed = true;
            }
        }
    }
}
