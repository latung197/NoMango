using System.Windows.Controls;
using ERP_System.ViewModels.Inventory;

namespace ERP_System.Views.Inventory
{
    public partial class MaterialView : UserControl
    {
        public MaterialView()
        {
            InitializeComponent();
            DataContext = new MaterialViewModel();
        }
    }
}