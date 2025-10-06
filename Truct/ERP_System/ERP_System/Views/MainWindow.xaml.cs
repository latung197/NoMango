using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ERP_System.Models;
using ERP_System.ViewModels;

namespace ERP_System.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var currentUser = Application.Current.Properties["CurrentUser"] as User;
            if (currentUser != null)
            {
                DataContext = new MainViewModel(currentUser);
            }

            InitializeDateTime();
        }

        private void InitializeDateTime()
        {
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = System.TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) => txtDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            timer.Start();
        }

        private void BtnNotifications_Click(object sender, RoutedEventArgs e)
        {
            var contextMenu = new ContextMenu();

            if (DataContext is MainViewModel viewModel)
            {
                if (viewModel.Notifications.Count == 0)
                {
                    contextMenu.Items.Add(new MenuItem { Header = "Không có thông báo mới", IsEnabled = false });
                }
                else
                {
                    foreach (var notification in viewModel.Notifications.Take(10)) // Limit to 10 items
                    {
                        var menuItem = new MenuItem
                        {
                            Header = CreateNotificationHeader(notification),
                            Tag = notification
                        };
                        menuItem.Click += (s, args) => viewModel.NotificationCommand.Execute(notification);
                        contextMenu.Items.Add(menuItem);
                    }

                    if (viewModel.Notifications.Count > 10)
                    {
                        contextMenu.Items.Add(new Separator());
                        contextMenu.Items.Add(new MenuItem
                        {
                            Header = $"Xem thêm {viewModel.Notifications.Count - 10} thông báo...",
                            IsEnabled = false
                        });
                    }
                }
            }

            contextMenu.PlacementTarget = btnNotifications;
            contextMenu.IsOpen = true;
        }

        private object CreateNotificationHeader(Notification notification)
        {
            var stackPanel = new StackPanel();

            var titleText = new TextBlock
            {
                Text = notification.Title,
                FontWeight = FontWeights.SemiBold,
                TextWrapping = System.Windows.TextWrapping.Wrap
            };

            var timeText = new TextBlock
            {
                Text = notification.CreatedDate.ToString("HH:mm dd/MM"),
                FontSize = 11,
                Foreground = System.Windows.Media.Brushes.Gray
            };

            stackPanel.Children.Add(titleText);
            stackPanel.Children.Add(timeText);

            return stackPanel;
        }

        private void BtnUserMenu_Click(object sender, RoutedEventArgs e)
        {
            var contextMenu = new ContextMenu();

            var userInfoItem = new MenuItem
            {
                Header = CreateUserInfoHeader(),
                IsEnabled = false
            };

            var changePasswordItem = new MenuItem
            {
                Header = "Đổi mật khẩu",
                Icon = new TextBlock { Text = "🔒", FontSize = 12 }
            };
            changePasswordItem.Click += (s, args) =>
            {
                if (DataContext is MainViewModel viewModel)
                    viewModel.ChangePasswordCommand.Execute(null);
            };

            var settingsItem = new MenuItem
            {
                Header = "Cài đặt",
                Icon = new TextBlock { Text = "⚙️", FontSize = 12 }
            };

            var logoutItem = new MenuItem
            {
                Header = "Đăng xuất",
                Icon = new TextBlock { Text = "🚪", FontSize = 12 }
            };
            logoutItem.Click += (s, args) =>
            {
                if (DataContext is MainViewModel viewModel)
                    viewModel.LogoutCommand.Execute(null);
            };

            contextMenu.Items.Add(userInfoItem);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(changePasswordItem);
            contextMenu.Items.Add(settingsItem);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(logoutItem);

            contextMenu.PlacementTarget = btnUserMenu;
            contextMenu.IsOpen = true;
        }

        private object CreateUserInfoHeader()
        {
            if (DataContext is MainViewModel viewModel)
            {
                var stackPanel = new StackPanel();

                var nameText = new TextBlock
                {
                    Text = viewModel.CurrentUser.FullName,
                    FontWeight = FontWeights.SemiBold
                };

                var companyText = new TextBlock
                {
                    Text = viewModel.CurrentUser.CompanyName,
                    FontSize = 11,
                    Foreground = System.Windows.Media.Brushes.Gray
                };

                var usernameText = new TextBlock
                {
                    Text = $"Tài khoản: {viewModel.CurrentUser.UserName}",
                    FontSize = 11,
                    Foreground = System.Windows.Media.Brushes.Gray
                };

                stackPanel.Children.Add(nameText);
                stackPanel.Children.Add(companyText);
                stackPanel.Children.Add(usernameText);

                return stackPanel;
            }

            return "User Info";
        }

        private void MenuTextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock && textBlock.DataContext is MenuItem menuItem)
            {
                if (DataContext is MainViewModel viewModel)
                {
                    viewModel.LoadViewCommand.Execute(menuItem);
                }
            }
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (DataContext is MainViewModel viewModel)
                {
                    viewModel.SearchCommand.Execute(null);
                }
            }
        }

        private void BtnMenuToggle_Checked(object sender, RoutedEventArgs e)
        {
            sidebarColumn.Width = new GridLength(60);
            // Collapse menu text, show only icons
        }

        private void BtnMenuToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            sidebarColumn.Width = new GridLength(250);
            // Expand menu text
        }
    }
}