using System.Windows;
using System.Windows.Controls;
using ERP_System.ViewModels.Inventory;

namespace ERP_System.Views.Inventory
{
    public partial class StockRequestView : UserControl
    {
        public StockRequestView()
        {
            InitializeComponent();
            DataContext = new StockRequestViewModel();
        }

        private void RemoveDetail_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is StockRequestViewModel viewModel)
            {
                // The button's DataContext is the StockRequestDetail
                var button = sender as Button;
                var detail = button?.DataContext as Models.StockRequestDetail;
                if (detail != null)
                {
                    viewModel.CurrentRequest.Details.Remove(detail);
                }
            }
        }
    }
}