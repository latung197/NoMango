using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AIOI
{
    public partial class FormMain : Form
    {
        private string currentUser;
        private string userRole;
        private Panel contentPanel;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatus;

        public FormMain(string username, string role = "User")
        {
            InitializeComponent();
            currentUser = username;
            userRole = role;
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Cấu hình form chính
            this.Text = $"Hệ thống ERP - {currentUser} ({userRole})";
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(1024, 768);

            // Tạo menu strip
            CreateMainMenu();

            // Tạo toolbar
            CreateToolbar();

            // Tạo status bar
            CreateStatusBar();

            // Tạo panel nội dung
            CreateContentPanel();

            // Tạo sidebar (tùy chọn)
            CreateSidebar();

            UpdateStatus($"Chào mừng {currentUser} đến với hệ thống ERP");
        }

        private void CreateMainMenu()
        {
            MenuStrip mainMenu = new MenuStrip();
            mainMenu.Dock = DockStyle.Top;
            mainMenu.Font = new Font("Segoe UI", 10);

            // Menu Hệ thống
            ToolStripMenuItem menuSystem = new ToolStripMenuItem("&Hệ thống");

            ToolStripMenuItem menuChangePassword = new ToolStripMenuItem("Đổi mật khẩu");
            menuChangePassword.Click += MenuChangePassword_Click;

            ToolStripMenuItem menuUserManager = new ToolStripMenuItem("Quản lý người dùng");
            menuUserManager.Click += MenuUserManager_Click;
            menuUserManager.Enabled = (userRole == "Admin"); // Chỉ Admin mới dùng được

            ToolStripMenuItem menuExit = new ToolStripMenuItem("Thoát");
            menuExit.Click += MenuExit_Click;

            menuSystem.DropDownItems.AddRange(new ToolStripItem[] {
                menuChangePassword,
                new ToolStripSeparator(),
                menuUserManager,
                new ToolStripSeparator(),
                menuExit
            });

            // Menu Bán hàng
            ToolStripMenuItem menuSales = new ToolStripMenuItem("&Bán hàng");

            ToolStripMenuItem menuCustomer = new ToolStripMenuItem("Quản lý khách hàng");
            menuCustomer.Click += MenuCustomer_Click;

            ToolStripMenuItem menuOrder = new ToolStripMenuItem("Quản lý đơn hàng");
            menuOrder.Click += MenuOrder_Click;

            ToolStripMenuItem menuInvoice = new ToolStripMenuItem("Hóa đơn");
            menuInvoice.Click += MenuInvoice_Click;

            menuSales.DropDownItems.AddRange(new ToolStripItem[] {
                menuCustomer,
                menuOrder,
                menuInvoice
            });

            // Menu Mua hàng
            ToolStripMenuItem menuPurchase = new ToolStripMenuItem("&Mua hàng");

            ToolStripMenuItem menuSupplier = new ToolStripMenuItem("Nhà cung cấp");
            menuSupplier.Click += MenuSupplier_Click;

            ToolStripMenuItem menuPurchaseOrder = new ToolStripMenuItem("Đơn mua hàng");
            menuPurchaseOrder.Click += MenuPurchaseOrder_Click;

            menuPurchase.DropDownItems.AddRange(new ToolStripItem[] {
                menuSupplier,
                menuPurchaseOrder
            });

            // Menu Kho
            ToolStripMenuItem menuInventory = new ToolStripMenuItem("&Kho");

            ToolStripMenuItem menuProduct = new ToolStripMenuItem("Sản phẩm");
            menuProduct.Click += MenuProduct_Click;

            ToolStripMenuItem menuStock = new ToolStripMenuItem("Tồn kho");
            menuStock.Click += MenuStock_Click;

            ToolStripMenuItem menuImport = new ToolStripMenuItem("Nhập kho");
            menuImport.Click += MenuImport_Click;

            ToolStripMenuItem menuExport = new ToolStripMenuItem("Xuất kho");
            menuExport.Click += MenuExport_Click;

            menuInventory.DropDownItems.AddRange(new ToolStripItem[] {
                menuProduct,
                menuStock,
                new ToolStripSeparator(),
                menuImport,
                menuExport
            });

            // Menu Sản xuất
            ToolStripMenuItem menuProduction = new ToolStripMenuItem("&Sản xuất");

            ToolStripMenuItem menuBOM = new ToolStripMenuItem("Định mức nguyên liệu");
            menuBOM.Click += MenuBOM_Click;

            ToolStripMenuItem menuPlan = new ToolStripMenuItem("Kế hoạch sản xuất");
            menuPlan.Click += MenuPlan_Click;

            menuProduction.DropDownItems.AddRange(new ToolStripItem[] {
                menuBOM,
                menuPlan
            });

            // Menu Tài chính
            ToolStripMenuItem menuFinance = new ToolStripMenuItem("&Tài chính");

            ToolStripMenuItem menuAccounting = new ToolStripMenuItem("Kế toán");
            menuAccounting.Click += MenuAccounting_Click;

            ToolStripMenuItem menuReport = new ToolStripMenuItem("Báo cáo");
            menuReport.Click += MenuReport_Click;

            menuFinance.DropDownItems.AddRange(new ToolStripItem[] {
                menuAccounting,
                menuReport
            });

            // Menu Nhân sự
            ToolStripMenuItem menuHR = new ToolStripMenuItem("&Nhân sự");

            ToolStripMenuItem menuEmployee = new ToolStripMenuItem("Nhân viên");
            menuEmployee.Click += MenuEmployee_Click;

            ToolStripMenuItem menuSalary = new ToolStripMenuItem("Tính lương");
            menuSalary.Click += MenuSalary_Click;

            ToolStripMenuItem menuTimekeeping = new ToolStripMenuItem("Chấm công");
            menuTimekeeping.Click += MenuTimekeeping_Click;

            menuHR.DropDownItems.AddRange(new ToolStripItem[] {
                menuEmployee,
                menuSalary,
                menuTimekeeping
            });

            // Menu Trợ giúp
            ToolStripMenuItem menuHelp = new ToolStripMenuItem("&Trợ giúp");

            ToolStripMenuItem menuAbout = new ToolStripMenuItem("Giới thiệu");
            menuAbout.Click += MenuAbout_Click;

            ToolStripMenuItem menuManual = new ToolStripMenuItem("Hướng dẫn");
            menuManual.Click += MenuManual_Click;

            menuHelp.DropDownItems.AddRange(new ToolStripItem[] {
                menuAbout,
                menuManual
            });

            // Thêm tất cả menu vào MenuStrip
            mainMenu.Items.AddRange(new ToolStripItem[] {
                menuSystem, menuSales, menuPurchase, menuInventory,
                menuProduction, menuFinance, menuHR, menuHelp
            });

            this.Controls.Add(mainMenu);
            this.MainMenuStrip = mainMenu;
        }

        private void CreateToolbar()
        {
            ToolStrip toolbar = new ToolStrip();
            toolbar.Dock = DockStyle.Top;
            toolbar.Font = new Font("Segoe UI", 9);
            toolbar.ImageScalingSize = new Size(32, 32);

            // Thêm các nút toolbar
            ToolStripButton btnCustomer = new ToolStripButton("Khách hàng");
            btnCustomer.Click += MenuCustomer_Click;
            btnCustomer.Image = CreateIcon(Color.Blue, "C");

            ToolStripButton btnProduct = new ToolStripButton("Sản phẩm");
            btnProduct.Click += MenuProduct_Click;
            btnProduct.Image = CreateIcon(Color.Green, "P");

            ToolStripButton btnOrder = new ToolStripButton("Đơn hàng");
            btnOrder.Click += MenuOrder_Click;
            btnOrder.Image = CreateIcon(Color.Orange, "O");

            ToolStripButton btnReport = new ToolStripButton("Báo cáo");
            btnReport.Click += MenuReport_Click;
            btnReport.Image = CreateIcon(Color.Red, "R");

            ToolStripButton btnLogout = new ToolStripButton("Đăng xuất");
            btnLogout.Click += BtnLogout_Click;
            btnLogout.Image = CreateIcon(Color.Gray, "X");

            toolbar.Items.AddRange(new ToolStripItem[] {
                btnCustomer, btnProduct, btnOrder, btnReport,
                new ToolStripSeparator(), btnLogout
            });

            this.Controls.Add(toolbar);
        }

        private void CreateStatusBar()
        {
            statusStrip = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            ToolStripStatusLabel lblUser = new ToolStripStatusLabel($"User: {currentUser}");
            ToolStripStatusLabel lblRole = new ToolStripStatusLabel($"Role: {userRole}");
            ToolStripStatusLabel lblTime = new ToolStripStatusLabel();

            // Cấu hình các label
            lblStatus.Spring = true;
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            lblUser.TextAlign = ContentAlignment.MiddleRight;
            lblRole.TextAlign = ContentAlignment.MiddleRight;

            statusStrip.Items.AddRange(new ToolStripItem[] {
                lblStatus, lblUser, lblRole, lblTime
            });

            // Timer cập nhật thời gian
            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += (s, e) => {
                lblTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            };
            timer.Start();

            this.Controls.Add(statusStrip);
        }

        private void CreateContentPanel()
        {
            contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.BackColor = Color.White;
            contentPanel.Padding = new Padding(10);
            this.Controls.Add(contentPanel);

            // Đưa contentPanel lên trên cùng (sau menu và toolbar)
            contentPanel.BringToFront();
        }

        private void CreateSidebar()
        {
            Panel sidebar = new Panel();
            sidebar.Width = 200;
            sidebar.Dock = DockStyle.Left;
            sidebar.BackColor = Color.FromArgb(240, 240, 240);

            // Thêm các nút chức năng nhanh vào sidebar
            FlowLayoutPanel quickAccess = new FlowLayoutPanel();
            quickAccess.Dock = DockStyle.Fill;
            quickAccess.FlowDirection = FlowDirection.TopDown;
            quickAccess.WrapContents = false;

            string[] quickButtons = {
                "📊 Dashboard", "👥 Khách hàng", "📦 Sản phẩm",
                "🛒 Đơn hàng", "📈 Báo cáo", "⚙️ Cài đặt"
            };

            foreach (string text in quickButtons)
            {
                Button btn = new Button();
                btn.Text = text;
                btn.Width = 180;
                btn.Height = 40;
                btn.Margin = new Padding(5);
                btn.TextAlign = ContentAlignment.MiddleLeft;
                btn.Font = new Font("Segoe UI", 9);
                btn.Click += QuickButton_Click;
                quickAccess.Controls.Add(btn);
            }

            sidebar.Controls.Add(quickAccess);
            this.Controls.Add(sidebar);
        }

        private Bitmap CreateIcon(Color color, string text)
        {
            Bitmap bmp = new Bitmap(32, 32);
            using (Graphics g = Graphics.FromImage(bmp))
            using (Brush brush = new SolidBrush(color))
            using (Font font = new Font("Arial", 12, FontStyle.Bold))
            using (Brush textBrush = new SolidBrush(Color.White))
            {
                g.Clear(Color.Transparent);
                g.FillEllipse(brush, 0, 0, 31, 31);
                g.DrawString(text, font, textBrush, new PointF(8, 6));
            }
            return bmp;
        }

        private void UpdateStatus(string message)
        {
            lblStatus.Text = message;
        }

        private void ShowFormInPanel(Form form)
        {
            // Xóa form cũ trong panel
            foreach (Control control in contentPanel.Controls)
            {
                if (control is Form)
                {
                    ((Form)control).Close();
                }
            }
            contentPanel.Controls.Clear();

            // Hiển thị form mới
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(form);
            form.Show();
        }

        #region Event Handlers for Menu Items

        private void MenuChangePassword_Click(object sender, EventArgs e)
        {
            FormChangePassword form = new FormChangePassword(currentUser);
            form.ShowDialog();
        }

        private void MenuUserManager_Click(object sender, EventArgs e)
        {
            FormUserManager form = new FormUserManager();
            ShowFormInPanel(form);
            UpdateStatus("Quản lý người dùng");
        }

        private void MenuCustomer_Click(object sender, EventArgs e)
        {
            FormCustomer form = new FormCustomer();
            ShowFormInPanel(form);
            UpdateStatus("Quản lý khách hàng");
        }

        private void MenuOrder_Click(object sender, EventArgs e)
        {
            FormOrder form = new FormOrder();
            ShowFormInPanel(form);
            UpdateStatus("Quản lý đơn hàng");
        }

        private void MenuInvoice_Click(object sender, EventArgs e)
        {
            FormInvoice form = new FormInvoice();
            ShowFormInPanel(form);
            UpdateStatus("Quản lý hóa đơn");
        }

        private void MenuSupplier_Click(object sender, EventArgs e)
        {
            FormSupplier form = new FormSupplier();
            ShowFormInPanel(form);
            UpdateStatus("Quản lý nhà cung cấp");
        }

        private void MenuPurchaseOrder_Click(object sender, EventArgs e)
        {
            FormPurchaseOrder form = new FormPurchaseOrder();
            ShowFormInPanel(form);
            UpdateStatus("Quản lý đơn mua hàng");
        }

        private void MenuProduct_Click(object sender, EventArgs e)
        {
            FormProduct form = new FormProduct();
            ShowFormInPanel(form);
            UpdateStatus("Quản lý sản phẩm");
        }

        private void MenuStock_Click(object sender, EventArgs e)
        {
            FormStock form = new FormStock();
            ShowFormInPanel(form);
            UpdateStatus("Quản lý tồn kho");
        }

        private void MenuImport_Click(object sender, EventArgs e)
        {
            FormImport form = new FormImport();
            ShowFormInPanel(form);
            UpdateStatus("Nhập kho");
        }

        private void MenuExport_Click(object sender, EventArgs e)
        {
            FormExport form = new FormExport();
            ShowFormInPanel(form);
            UpdateStatus("Xuất kho");
        }

        private void MenuBOM_Click(object sender, EventArgs e)
        {
            FormBOM form = new FormBOM();
            ShowFormInPanel(form);
            UpdateStatus("Định mức nguyên liệu");
        }

        private void MenuPlan_Click(object sender, EventArgs e)
        {
            FormProductionPlan form = new FormProductionPlan();
            ShowFormInPanel(form);
            UpdateStatus("Kế hoạch sản xuất");
        }

        private void MenuAccounting_Click(object sender, EventArgs e)
        {
            FormAccounting form = new FormAccounting();
            ShowFormInPanel(form);
            UpdateStatus("Kế toán");
        }

        private void MenuReport_Click(object sender, EventArgs e)
        {
            FormReport form = new FormReport();
            ShowFormInPanel(form);
            UpdateStatus("Báo cáo");
        }

        private void MenuEmployee_Click(object sender, EventArgs e)
        {
            FormEmployee form = new FormEmployee();
            ShowFormInPanel(form);
            UpdateStatus("Quản lý nhân viên");
        }

        private void MenuSalary_Click(object sender, EventArgs e)
        {
            FormSalary form = new FormSalary();
            ShowFormInPanel(form);
            UpdateStatus("Tính lương");
        }

        private void MenuTimekeeping_Click(object sender, EventArgs e)
        {
            FormTimekeeping form = new FormTimekeeping();
            ShowFormInPanel(form);
            UpdateStatus("Chấm công");
        }

        private void MenuAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hệ thống ERP\nPhiên bản 1.0\n© 2024", "Giới thiệu");
        }

        private void MenuManual_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://help.erp-system.com");
        }

        private void MenuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Hide();
                Login loginForm = new Login();
                loginForm.Show();
            }
        }

        private void QuickButton_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                UpdateStatus($"Đã chọn: {btn.Text}");
                // Có thể thêm xử lý cho từng nút tại đây
            }
        }

        #endregion
    }
}

