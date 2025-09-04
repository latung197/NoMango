using CoreMVC.Models;

namespace CoreMVC.DA.Respository
{
    public class MstConfigurationRespository
    {
        public readonly CoreDbContext _context;
        public MstConfigurationRespository(CoreDbContext context)
        {
            _context = context;
        }
        public List<MstConfiguration> GetListConfiguration()
        {
            var existingConfig = _context.mstConfigurations.ToList() ?? new List<MstConfiguration>();
            return existingConfig;
        }
    }
}
