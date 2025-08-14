using PuduIOT.BL.Interfaces;
using PuduIOT.Models;

namespace PuduIOT.BL.Services
{
    public class BreadcrumbService : IBreadcrumbService
    {
        private List<BreadcrumbItem> _breadcrumbItems;

        public BreadcrumbService()
        {
            _breadcrumbItems = new List<BreadcrumbItem>();
        }

        public void AddBreadcrumbItem(string displayName, string url)
        {
            _breadcrumbItems.Add(new BreadcrumbItem
            {
                DisplayName = displayName,
                Url = url
            });
        }

        public List<BreadcrumbItem> GetBreadcrumbItems()
        {
            return _breadcrumbItems;
        }
    }
}
