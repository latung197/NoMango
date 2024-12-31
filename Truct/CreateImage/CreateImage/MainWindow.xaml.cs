using Microsoft.Win32;
using System.Configuration;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CreateImage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string excelFilePath;
        private string imagesDirectory;
        private string outputDirectory;
        public MainWindow()
        {
            InitializeComponent();
        }


        private void LoadSettings()
        {
            excelFilePath = ConfigurationManager.AppSettings["ExcelFilePath"];
            imagesDirectory = ConfigurationManager.AppSettings["ImagesDirectory"];
            outputDirectory = ConfigurationManager.AppSettings["OutputDirectory"];

            textBoxExcelFile.Text = excelFilePath;
            textBoxImagesDirectory.Text = imagesDirectory;
            textBoxOutputDirectory.Text = outputDirectory;
        }

        private void SaveSettings()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["ExcelFilePath"].Value = excelFilePath;
            config.AppSettings.Settings["ImagesDirectory"].Value = imagesDirectory;
            config.AppSettings.Settings["OutputDirectory"].Value = outputDirectory;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void btnBrowseExcel_Click(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
            //if (openFileDialog.ShowDialog() == true)
            //{
            //    excelFilePath = openFileDialog.FileName;
            //    textBoxExcelFile.Text = excelFilePath;
            //    SaveSettings();
            //}
        }

        private void btnBrowseImages_Click(object sender, RoutedEventArgs e)
        {

            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                imagesDirectory = folderBrowserDialog.SelectedPath;
                textBoxImagesDirectory.Text = imagesDirectory;
                SaveSettings();
            }
        }

        private void btnButtonBrowseOutput_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                outputDirectory = folderBrowserDialog.SelectedPath;
                textBoxOutputDirectory.Text = outputDirectory;
                SaveSettings();
            }
        }
    }
}