using PuduIOT.Models;
using Microsoft.EntityFrameworkCore;

namespace PuduIOT.DA
{
    public class PuduIotDbContext : DbContext
    {
        public PuduIotDbContext(DbContextOptions<PuduIotDbContext> options)
          : base(options)
        {
        }
        public DbSet<MstLanguage> MstLanguages { get; set; }
        public DbSet<MstUnit> MstUnits { get; set; }
        public DbSet<MstConfiguration> MstConfigurations { get; set; }
        public DbSet<MstRobotStatus> MstRobotStatus { get; set; }
        public DbSet<MstRobotInfor> MstRobotInfo { get; set; }

    }
}
