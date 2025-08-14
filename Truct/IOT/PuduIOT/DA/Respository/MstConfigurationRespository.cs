using PuduIOT.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace PuduIOT.DA.Respository
{
    public class MstConfigurationRespository
    {
        private readonly HamadenDbContext _context;

        public MstConfigurationRespository(HamadenDbContext context)
        {
            _context = context;
        }

        public List<MstConfiguration> GetListConfiguration()
        {
            var existingConfig = _context.MstConfigurations.ToList() ?? new List<MstConfiguration>();
            return existingConfig;


        }
        public string GetValueByKey(string key)
        {
            var existingConfig = _context.MstConfigurations.FirstOrDefault(e => e.Key == key);
            if (existingConfig == null)
            {
                return string.Empty;
            }
            return existingConfig.Value;
        }

        public void UpdateValueByKey(string key, string value)
        {
            
            var existingUnit = _context.MstConfigurations.FirstOrDefault(e => e.Key == key);
            if (existingUnit != null)
            {
                existingUnit.Value =  value ;
                existingUnit.update_time = DateTime.UtcNow;
                existingUnit.update_id = "admin";
                _context.SaveChanges();
            }
        }
    }
}
