using Core.Domain;
using Core.Domain.Entity;
using Core.Domain.Entity.SystemEntities;
using Core.Infrastructure.ContextAccessors;
using Core.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Reflection;

namespace Core.Infrastructure.Context
{
    public class CoreContext : DbContext
    {
        #region Properties
        private readonly IConfiguration _configuration;
        private readonly IUserPrincipalService _userPrincipalService;
        public static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
#if DEBUG
            builder
                .AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Warning)
                .AddFilter(DbLoggerCategory.Query.Name, LogLevel.Debug)
                .AddConsole();
#endif

        }
        );

        #endregion
        #region Constructor
        public CoreContext(DbContextOptions<CoreContext> options
            , IConfiguration configuration
            , IUserPrincipalService userPrincipalService) : base(options)
        {
            _configuration = configuration;
            _userPrincipalService = userPrincipalService;
        }

        public CoreContext() : base()
        {

        }

        public CoreContext(DbContextOptions<CoreContext> options) : base(options)
        {

        }
        public CoreContext(IUserPrincipalService userPrincipalService) : base()
        {
            _userPrincipalService = userPrincipalService;
        }
        #endregion
        #region Method
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            string DbType = _configuration.GetConnectionString("DatabaseType") ?? "2";

            // Lấy chuỗi kết nối
            string? connectionString = DbType == "2"
                ? _configuration?.GetConnectionString("CoreContext")
                : Environment.GetEnvironmentVariable("CoreContext");

            switch (DbType)
            {
                case "1"://MSSQL
                    optionsBuilder.UseLoggerFactory(loggerFactory).UseSqlServer(connectionString);
                    break;
                case "2"://Postgre
                    optionsBuilder.UseLoggerFactory(loggerFactory).UseNpgsql(connectionString);
                    break;
                default://Postgre
                    optionsBuilder.UseLoggerFactory(loggerFactory).UseNpgsql(connectionString);
                    break;
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        private object CreateWithValues(EntityEntry values)
        {
            object entity = Activator.CreateInstance(values.Entity.GetType());
            foreach (PropertyInfo property in values.Entity.GetType().GetProperties())
            {
                //var property = type.GetProperty(propname.);
                property.SetValue(entity, values.Property(property.Name).OriginalValue);
            }

            return entity;
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            //Do something here
            try
            {
                List<EntityEntry> modifiedEntries = ChangeTracker.Entries()
                    .Where(x => x.Entity is IAuditable
                                && (x.State == EntityState.Added || x.State == EntityState.Modified)).ToList();

                if (modifiedEntries.Any())
                {
                    foreach (EntityEntry entry in modifiedEntries)
                    {
                        IAuditable? entity = entry.Entity as IAuditable;
                        if (entity is null)
                        {
                            continue;
                        }

                        if (entry.State == EntityState.Added)
                        {
                            //Acc CreateID, CreateDt here
                            if (_userPrincipalService.IsAuthenticated)
                                entity.CreateId = _userPrincipalService.UserId.ToString();

                            entity.CreateTime = DateTime.Now;
                        }
                        else
                        {
                            //Update UpdateId, UpdateDt here
                            if (_userPrincipalService.IsAuthenticated)
                                entity.UpdateId = _userPrincipalService.UserId.ToString();

                            entity.UpdateTime = DateTime.Now;
                        }
                    }
                }
            }
            catch
            {
                //Ignore
            }
            return await base.SaveChangesAsync(true, cancellationToken);
        }
        #endregion
        #region Declare entity here
        public DbSet<SysUser> SysUser { get; set; }
        public DbSet<SysUserCommand> SysUserCommand { get; set; }
        public DbSet<ExportListPlan> ExportListPlan { get; set; }
        public DbSet<ExportHistoryList> ExportHistoryList { get; set; }
        public DbSet<MstData> MstData { get; set; }
        public DbSet<EcuData> EcuData { get; set; }
        public DbSet<EcuExported> EcuExported { get; set; }
        public DbSet<BoxInfo> BoxInfo { get; set; }
        #endregion
    }
}
