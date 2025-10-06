using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Animation;

namespace Main.Views
{
    public partial class MainView : Window
    {
        private Button currentActiveButton;
        private Dictionary<string, TabInfo> openTabs = new Dictionary<string, TabInfo>();
        private string currentActiveTab = "DashboardView";
        private bool isMenuExpanded = true;
        private List<Button> allMenuButtons = new List<Button>();

        public class TabInfo
        {
            public string ViewName { get; set; }
            public string Title { get; set; }
            public UserControl Content { get; set; }
            public Button TabButton { get; set; }
        }

        public MainView()
        {
            InitializeComponent();
            InitializeApplication();
        }

        private void InitializeApplication()
        {
            // Set user info
            txtUserName.Text = "Quản trị viên";

            // Collect all menu buttons for search functionality
            CollectMenuButtons();

            // Load dashboard as default
            LoadView("DashboardView");
            SetActiveButton(btnDashboard);
        }

        private void CollectMenuButtons()
        {
            allMenuButtons.Clear();
            allMenuButtons.AddRange(new[]
            {
                btnDashboard, btnBanHang, btnHoaDon, btnKhachHang,
                btnSanPham, btnNhapKho, btnXuatKho, btnTonKho,
                btnNhanVien, btnChamCong, btnLuong,
                btnBaoCaoDoanhThu, btnBaoCaoBanHang, btnBaoCaoKho,
                btnNguoiDung, btnPhanQuyen, btnSaoLuu
            });
        }

        private void btnToggleMenu_Click(object sender, RoutedEventArgs e)
        {
            ToggleMenu();
        }

        private void ToggleMenu()
        {
            if (isMenuExpanded)
            {
                // Ẩn menu với animation
                var animation = new DoubleAnimation(250, 0, TimeSpan.FromSeconds(0.2));
                sidebarMenu.BeginAnimation(Border.WidthProperty, animation);

                sidebarMenu.Visibility = Visibility.Collapsed;
                btnToggleMenuHidden.Visibility = Visibility.Visible;
                btnToggleMenu.Content = "►";
            }
            else
            {
                // Hiện menu
                sidebarMenu.Visibility = Visibility.Visible;
                var animation = new DoubleAnimation(0, 250, TimeSpan.FromSeconds(0.2));
                sidebarMenu.BeginAnimation(Border.WidthProperty, animation);

                btnToggleMenuHidden.Visibility = Visibility.Collapsed;
                btnToggleMenu.Content = "◀";
            }

            isMenuExpanded = !isMenuExpanded;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.ToLower();

            // Ẩn watermark khi có text
            txtSearchWatermark.Visibility = string.IsNullOrEmpty(searchText) ? Visibility.Visible : Visibility.Hidden;

            // Tìm kiếm và lọc menu items
            FilterMenuItems(searchText);
        }

        private void FilterMenuItems(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Hiện tất cả menu items
                foreach (var button in allMenuButtons)
                {
                    button.Visibility = Visibility.Visible;
                }

                // Mở tất cả expanders
                expBanHang.IsExpanded = true;
                expKho.IsExpanded = true;
                expNhanSu.IsExpanded = true;
                expBaoCao.IsExpanded = true;
                expHeThong.IsExpanded = true;
                return;
            }

            // Ẩn tất cả trước
            foreach (var button in allMenuButtons)
            {
                button.Visibility = Visibility.Collapsed;
            }

            // Tìm và hiện các items phù hợp
            var matchedButtons = allMenuButtons.Where(btn =>
                btn.Content.ToString().ToLower().Contains(searchText)).ToList();

            foreach (var button in matchedButtons)
            {
                button.Visibility = Visibility.Visible;
            }

            // Mở expanders có items được tìm thấy
            expBanHang.IsExpanded = matchedButtons.Any(btn =>
                btn == btnBanHang || btn == btnHoaDon || btn == btnKhachHang);

            expKho.IsExpanded = matchedButtons.Any(btn =>
                btn == btnSanPham || btn == btnNhapKho || btn == btnXuatKho || btn == btnTonKho);

            expNhanSu.IsExpanded = matchedButtons.Any(btn =>
                btn == btnNhanVien || btn == btnChamCong || btn == btnLuong);

            expBaoCao.IsExpanded = matchedButtons.Any(btn =>
                btn == btnBaoCaoDoanhThu || btn == btnBaoCaoBanHang || btn == btnBaoCaoKho);

            expHeThong.IsExpanded = matchedButtons.Any(btn =>
                btn == btnNguoiDung || btn == btnPhanQuyen || btn == btnSaoLuu);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag != null)
            {
                string viewName = button.Tag.ToString();
                LoadView(viewName);
                SetActiveButton(button);

                // Clear search after selecting an item
                txtSearch.Text = "";
            }
        }

        private void LoadView(string viewName)
        {
            try
            {
                // Update current view title
                txtCurrentView.Text = GetViewTitle(viewName);
                currentActiveTab = viewName;

                // Create or activate tab
                if (openTabs.ContainsKey(viewName))
                {
                    // Tab already exists, activate it
                    ActivateTab(viewName);
                }
                else
                {
                    // Create new tab
                    CreateTab(viewName);
                }

                // Load the corresponding view
                UserControl content = GetViewContent(viewName);
                if (content != null)
                {
                    MainContentFrame.Content = content;
                    openTabs[viewName].Content = content;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải view: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateTab(string viewName)
        {
            var tabInfo = new TabInfo
            {
                ViewName = viewName,
                Title = GetViewTitle(viewName)
            };

            // Create tab button với độ rộng tự động
            Button tabButton = new Button();
            tabButton.Content = tabInfo.Title;
            tabButton.Style = (Style)FindResource("TabItemStyle");
            tabButton.Tag = viewName;
            tabButton.Click += TabButton_Click;

            // Tính toán độ rộng dựa trên nội dung
            MeasureTabWidth(tabButton);

            tabInfo.TabButton = tabButton;
            openTabs[viewName] = tabInfo;

            // Add to tab container
            tabContainer.Items.Add(tabButton);

            // Activate the new tab
            ActivateTab(viewName);
        }

        private void MeasureTabWidth(Button tabButton)
        {
            // Tính toán độ rộng dựa trên nội dung text
            var textBlock = new TextBlock()
            {
                Text = tabButton.Content.ToString(),
                FontSize = 11,
                FontWeight = FontWeights.Normal
            };

            textBlock.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
            double textWidth = textBlock.DesiredSize.Width;

            // Độ rộng tab = độ rộng text + padding + close button
            double tabWidth = textWidth + 30 + 20; // 30 padding, 20 close button
            tabWidth = System.Math.Max(tabWidth, 80); // Tối thiểu 80
            tabWidth = System.Math.Min(tabWidth, 150); // Tối đa 150

            tabButton.Width = tabWidth;
        }

        private void ActivateTab(string viewName)
        {
            // Deactivate all tabs
            foreach (var tab in openTabs.Values)
            {
                tab.TabButton.Style = (Style)FindResource("TabItemStyle");
            }

            // Activate selected tab
            if (openTabs.ContainsKey(viewName))
            {
                openTabs[viewName].TabButton.Style = (Style)FindResource("ActiveTabItemStyle");
                currentActiveTab = viewName;

                // Update main content
                if (openTabs[viewName].Content != null)
                {
                    MainContentFrame.Content = openTabs[viewName].Content;
                }
            }
        }

        private void TabButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag != null)
            {
                string viewName = button.Tag.ToString();
                ActivateTab(viewName);
                txtCurrentView.Text = GetViewTitle(viewName);
            }
        }

        private void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            var closeButton = sender as Button;
            if (closeButton?.Tag is Button tabButton && tabButton.Tag != null)
            {
                string viewName = tabButton.Tag.ToString();
                CloseTab(viewName);
            }
            e.Handled = true;
        }

        private void CloseTab(string viewName)
        {
            if (openTabs.ContainsKey(viewName))
            {
                // Don't close the last tab or dashboard
                if (openTabs.Count <= 1 || viewName == "DashboardView")
                {
                    MessageBox.Show("Không thể đóng tab Dashboard hoặc tab cuối cùng.", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Remove tab from UI
                tabContainer.Items.Remove(openTabs[viewName].TabButton);
                openTabs.Remove(viewName);

                // Activate another tab
                if (currentActiveTab == viewName)
                {
                    var remainingTab = openTabs.FirstOrDefault();
                    if (remainingTab.Value != null)
                    {
                        ActivateTab(remainingTab.Key);
                        txtCurrentView.Text = GetViewTitle(remainingTab.Key);
                    }
                }
            }
        }

        private void btnCloseAllTabs_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn đóng tất cả các tab?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Giữ lại tab Dashboard, đóng các tab khác
                var tabsToClose = openTabs.Where(tab => tab.Key != "DashboardView").ToList();

                foreach (var tab in tabsToClose)
                {
                    tabContainer.Items.Remove(tab.Value.TabButton);
                    openTabs.Remove(tab.Key);
                }

                // Kích hoạt lại tab Dashboard
                if (openTabs.ContainsKey("DashboardView"))
                {
                    ActivateTab("DashboardView");
                    txtCurrentView.Text = GetViewTitle("DashboardView");
                }
            }
        }

        private UserControl GetViewContent(string viewName)
        {
            switch (viewName)
            {
                case "DashboardView": return new DashboardView();
                case "BanHangView": return new BanHangView();
                //case "HoaDonView": return new HoaDonView();
                //case "KhachHangView": return new KhachHangView();
                //case "SanPhamView": return new SanPhamView();
                //case "NhapKhoView": return new NhapKhoView();
                //case "XuatKhoView": return new XuatKhoView();
                //case "TonKhoView": return new TonKhoView();
                //case "NhanVienView": return new NhanVienView();
                //case "ChamCongView": return new ChamCongView();
                //case "LuongView": return new LuongView();
                //case "BaoCaoDoanhThuView": return new BaoCaoDoanhThuView();
                //case "BaoCaoBanHangView": return new BaoCaoBanHangView();
                //case "BaoCaoKhoView": return new BaoCaoKhoView();
                //case "NguoiDungView": return new NguoiDungView();
                //case "PhanQuyenView": return new PhanQuyenView();
                //case "SaoLuuView": return new SaoLuuView();
                default: return new DashboardView();
            }
        }

        private void SetActiveButton(Button activeButton)
        {
            // Reset previous active button
            if (currentActiveButton != null)
            {
                currentActiveButton.ClearValue(Button.BackgroundProperty);
                currentActiveButton.Foreground = new SolidColorBrush(Color.FromRgb(232, 232, 232));
            }

            // Set new active button
            currentActiveButton = activeButton;
            currentActiveButton.Background = new SolidColorBrush(Color.FromRgb(66, 101, 156));
            currentActiveButton.Foreground = Brushes.White;
        }

        private string GetViewTitle(string viewName)
        {
            var titles = new Dictionary<string, string>
            {
                {"DashboardView", "Dashboard"},
                {"BanHangView", "Bán Hàng"},
                {"HoaDonView", "Hóa Đơn"},
                {"KhachHangView", "Khách Hàng"},
                {"SanPhamView", "Sản Phẩm"},
                {"NhapKhoView", "Nhập Kho"},
                {"XuatKhoView", "Xuất Kho"},
                {"TonKhoView", "Tồn Kho"},
                {"NhanVienView", "Nhân Viên"},
                {"ChamCongView", "Chấm Công"},
                {"LuongView", "Tính Lương"},
                {"BaoCaoDoanhThuView", "Doanh Thu"},
                {"BaoCaoBanHangView", "Bán Hàng"},
                {"BaoCaoKhoView", "Tồn Kho"},
                {"NguoiDungView", "Người Dùng"},
                {"PhanQuyenView", "Phân Quyền"},
                {"SaoLuuView", "Sao Lưu"}
            };

            return titles.ContainsKey(viewName) ? titles[viewName] : "Hệ Thống";
        }

        private void btnDangXuat_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận đăng xuất",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                LoginView loginView = new LoginView();
                loginView.Show();
                this.Close();
            }
        }

        private void btnThongTin_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Thông tin tài khoản\nTên: Quản trị viên\nVai trò: Administrator",
                "Thông Tin Tài Khoản", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        protected override void OnClosed(System.EventArgs e)
        {
            openTabs.Clear();
            tabContainer.Items.Clear();
            base.OnClosed(e);
        }
    }
}