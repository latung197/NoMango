using System.Windows;
using System.IO;
using ERP_System.Utilities;
using ERP_System.Views;

namespace ERP_System
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Tạo thư mục cần thiết
            CreateRequiredDirectories();

            // Khởi tạo database connection
            DatabaseHelper.Initialize();

            // Hiển thị màn hình đăng nhập
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            // Cleanup resources
        }

        private void CreateRequiredDirectories()
        {
            string exportPath = System.Configuration.ConfigurationManager.AppSettings["ExportPath"];
            string backupPath = System.Configuration.ConfigurationManager.AppSettings["BackupPath"];
            string logPath = System.Configuration.ConfigurationManager.AppSettings["LogPath"];

            Directory.CreateDirectory(exportPath);
            Directory.CreateDirectory(backupPath);
            Directory.CreateDirectory(logPath);
        }
    }
}