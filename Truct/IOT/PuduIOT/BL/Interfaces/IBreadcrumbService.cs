using PuduIOT.Models;

namespace PuduIOT.BL.Interfaces
{
    public interface IBreadcrumbService
    {
        List<BreadcrumbItem> GetBreadcrumbItems();

        void AddBreadcrumbItem(string displayName, string url);
    }
}
