using PuduIOT.Models;
using Microsoft.EntityFrameworkCore;

namespace PuduIOT.DA
{
    public class HamadenDbContext : DbContext
    {
        public HamadenDbContext(DbContextOptions<HamadenDbContext> options)
          : base(options)
        {
        }
        public DbSet<MstLanguage> MstLanguages { get; set; }
        public DbSet<MstUnit> MstUnits { get; set; }
        public DbSet<MstConfiguration> MstConfigurations { get; set; }
        public DbSet<MstRobotStatus> MstRobotInfo { get; set; }

    }
}
