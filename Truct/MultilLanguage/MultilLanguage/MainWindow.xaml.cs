using MultiLangWpfNet8.Helpers;
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

namespace MultilLanguage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string lang = Properties.Settings.Default.Language;
            if (string.IsNullOrEmpty(lang))
                lang = "vi";

            LocalizationManager.SetLanguage(lang);

            foreach (ComboBoxItem item in LanguageSelector.Items)
            {
                if (item.Tag.ToString() == lang)
                {
                    LanguageSelector.SelectedItem = item;
                    break;
                }
            }
        }

        private void LanguageChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LanguageSelector.SelectedItem is ComboBoxItem item)
            {
                string lang = item.Tag.ToString();
                LocalizationManager.SetLanguage(lang);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}