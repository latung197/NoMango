using CoreMVC.Models;

namespace CoreMVC.BL.Interfaces
{
    public interface IMstConfigurationService
    {
        string GetValueByKey(string key);
        List<MstConfiguration> GetListConfiguration();
        void UpdateValueByKey(string key, string valuey);
    }
}
