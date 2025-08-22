using PuduIOT.BL.Interfaces;
using PuduIOT.DA;
using PuduIOT.DA.Respository;
using PuduIOT.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Protocol.Core.Types;

namespace PuduIOT.BL.Services
{
    public class MstConfigurationService : IMstConfigurationService
    {
        private readonly MstConfigurationRespository _respository;

        public MstConfigurationService(PuduIotDbContext context)
        {
            _respository = new MstConfigurationRespository(context);
        }

        public string GetValueByKey(string key)
        {
            return _respository.GetValueByKey(key);
        }

        public List<MstConfiguration> GetListConfiguration()
        {
            return _respository.GetListConfiguration();
        }

        public void UpdateValueByKey(string key, string value)
        {
             _respository.UpdateValueByKey(key, value);
        }
    }
}
