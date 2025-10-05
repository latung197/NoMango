using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ERP_System.Models;
using ERP_System.Utilities;
using ERP_System.Repositories;
using ERP_System.Views;

namespace ERP_System.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly UserRepository _userRepository;
        private readonly DatabaseHelper _databaseHelper;

        private string _userName = "admin";
        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        private string _password = "123456";
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private DatabaseConnection _selectedDatabase;
        public DatabaseConnection SelectedDatabase
        {
            get => _selectedDatabase;
            set => SetProperty(ref _selectedDatabase, value);
        }

        private ObservableCollection<DatabaseConnection> _databases;
        public ObservableCollection<DatabaseConnection> Databases
        {
            get => _databases;
            set => SetProperty(ref _databases, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand ExitCommand { get; }

        public LoginViewModel()
        {
            _userRepository = new UserRepository();
            _databaseHelper = new DatabaseHelper();

            LoadDatabases();
            LoginCommand = new RelayCommand(Login, CanLogin);
            ExitCommand = new RelayCommand(Exit);
        }

        private void LoadDatabases()
        {
            try
            {
                var connections = _databaseHelper.GetDatabaseConnections();
                Databases = new ObservableCollection<DatabaseConnection>(connections);
                if (Databases.Count > 0)
                    SelectedDatabase = Databases[0];
            }
            catch (Exception ex)
            {
                ShowError($"Không thể tải danh sách database: {ex.Message}");
            }
        }

        private bool CanLogin()
        {
            return !string.IsNullOrEmpty(UserName) &&
                   !string.IsNullOrEmpty(Password) &&
                   SelectedDatabase != null;
        }

        private void Login()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Đang đăng nhập...";

                var user = _userRepository.Authenticate(UserName, Password);
                if (user != null)
                {
                    // Đăng nhập thành công
                    Application.Current.Properties["CurrentUser"] = user;

                    var mainWindow = new MainWindow();
                    mainWindow.Show();

                    // Đóng window đăng nhập
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window is LoginWindow)
                            window.Close();
                    }
                }
                else
                {
                    ShowError("Tên đăng nhập hoặc mật khẩu không đúng!");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi đăng nhập: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                StatusMessage = "";
            }
        }

        private void Exit()
        {
            if (ShowConfirmation("Bạn có chắc chắn muốn thoát?"))
            {
                Application.Current.Shutdown();
            }
        }
    }
}