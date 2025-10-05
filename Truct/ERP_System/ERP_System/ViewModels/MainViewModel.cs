using System.Collections.ObjectModel;
using System.Windows.Input;
using ERP_System.Models;
using ERP_System.Utilities;
using ERP_System.Repositories;
using System.Windows;
using ERP_System.Views;

namespace ERP_System.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly MenuRepository _menuRepository;
        private readonly NotificationRepository _notificationRepository;

        private User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        private ObservableCollection<MenuItem> _menuItems;
        public ObservableCollection<MenuItem> MenuItems
        {
            get => _menuItems;
            set => SetProperty(ref _menuItems, value);
        }

        private ObservableCollection<Notification> _notifications;
        public ObservableCollection<Notification> Notifications
        {
            get => _notifications;
            set => SetProperty(ref _notifications, value);
        }

        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public ICommand LoadViewCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand NotificationCommand { get; }
        public ICommand ChangePasswordCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand MarkNotificationAsReadCommand { get; }

        public MainViewModel(User user)
        {
            CurrentUser = user;
            _menuRepository = new MenuRepository();
            _notificationRepository = new NotificationRepository();

            InitializeCommands();
            LoadData();
        }

        private void InitializeCommands()
        {
            LoadViewCommand = new RelayCommand<MenuItem>(LoadView);
            SearchCommand = new RelayCommand(Search);
            NotificationCommand = new RelayCommand<Notification>(OpenNotification);
            ChangePasswordCommand = new RelayCommand(ChangePassword);
            LogoutCommand = new RelayCommand(Logout);
            MarkNotificationAsReadCommand = new RelayCommand<Notification>(MarkNotificationAsRead);
        }

        private async void LoadData()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Đang tải dữ liệu...";

                // Load menu và notifications song song
                var menuTask = _menuRepository.GetUserMenu(CurrentUser.UserID);
                var notificationTask = _notificationRepository.GetUserNotifications(CurrentUser.UserID);

                await Task.WhenAll(menuTask, notificationTask);

                MenuItems = new ObservableCollection<MenuItem>(menuTask.Result);
                Notifications = new ObservableCollection<Notification>(notificationTask.Result);

                // Load view mặc định
                LoadDefaultView();

                StatusMessage = "Tải dữ liệu thành công";
            }
            catch (Exception ex)
            {
                ShowError($"Không thể tải dữ liệu: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void LoadDefaultView()
        {
            CurrentView = new Views.DashboardView();
        }

        private void LoadView(MenuItem menuItem)
        {
            if (menuItem == null || string.IsNullOrEmpty(menuItem.ClassName)) return;

            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var type = assembly.GetType(menuItem.ClassName);
                if (type != null)
                {
                    CurrentView = Activator.CreateInstance(type);
                }
                else
                {
                    ShowError($"Không tìm thấy class: {menuItem.ClassName}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Không thể tải view: {ex.Message}");
            }
        }

        private void Search()
        {
            if (!string.IsNullOrEmpty(SearchText))
            {
                // Tìm kiếm trong menu items
                var foundMenu = FindMenuInTree(MenuItems, SearchText);
                if (foundMenu != null)
                {
                    LoadView(foundMenu);
                }
                else
                {
                    ShowMessage($"Không tìm thấy chức năng: {SearchText}");
                }
            }
        }

        private MenuItem FindMenuInTree(ObservableCollection<MenuItem> menus, string searchText)
        {
            foreach (var menu in menus)
            {
                if (menu.MenuName.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                    return menu;

                var foundInChildren = FindMenuInTree(menu.Children, searchText);
                if (foundInChildren != null)
                    return foundInChildren;
            }
            return null;
        }

        private void OpenNotification(Notification notification)
        {
            if (notification != null)
            {
                MarkNotificationAsRead(notification);

                switch (notification.ReferenceType)
                {
                    case "StockRequest":
                        OpenStockRequest(notification.ReferenceID);
                        break;
                    case "PurchaseRequest":
                        OpenPurchaseRequest(notification.ReferenceID);
                        break;
                    case "SalesOrder":
                        OpenSalesOrder(notification.ReferenceID);
                        break;
                    default:
                        ShowMessage($"Mở {notification.ReferenceType} ID: {notification.ReferenceID}");
                        break;
                }
            }
        }

        private void OpenStockRequest(int requestID)
        {
            // Load StockRequestView với ID cụ thể
            var stockRequestView = new Views.Inventory.StockRequestView();
            if (stockRequestView.DataContext is ViewModels.Inventory.StockRequestViewModel viewModel)
            {
                viewModel.LoadRequest(requestID);
            }
            CurrentView = stockRequestView;
        }

        private void OpenPurchaseRequest(int requestID)
        {
            ShowMessage($"Mở phiếu yêu cầu mua hàng ID: {requestID}");
        }

        private void OpenSalesOrder(int orderID)
        {
            ShowMessage($"Mở đơn hàng bán ID: {orderID}");
        }

        private async void MarkNotificationAsRead(Notification notification)
        {
            try
            {
                await _notificationRepository.MarkAsRead(notification.NotificationID);
                notification.IsRead = true;
                Notifications.Remove(notification);
                OnPropertyChanged(nameof(Notifications));
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi khi đánh dấu thông báo đã đọc: {ex.Message}");
            }
        }

        private void ChangePassword()
        {
            var dialog = new Views.ChangePasswordDialog(CurrentUser);
            dialog.Owner = Application.Current.MainWindow;
            dialog.ShowDialog();
        }

        private void Logout()
        {
            if (ShowConfirmation("Bạn có chắc chắn muốn đăng xuất?"))
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is MainWindow)
                    {
                        window.Close();
                        break;
                    }
                }

                new LoginWindow().Show();
            }
        }
    }
}