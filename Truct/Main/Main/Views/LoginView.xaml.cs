using System.Windows;
using System.Windows.Input;

namespace Main.Views
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            // Xử lý login
        }

        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            // Xử lý mở cài đặt
        }

        private void BindablePasswordBox_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}