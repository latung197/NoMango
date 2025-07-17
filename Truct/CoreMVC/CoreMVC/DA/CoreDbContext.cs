using CoreMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreMVC.DA
{
    public class CoreDbContext: DbContext
    {
        public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options) { }
        public DbSet<MstLanguage>  mstLanguages {  get; set; }
        public DbSet<MstUnit> mstUnits { get; set; }
        public DbSet<MstConfiguration> mstConfigurations { get; set; } 
    }
}
