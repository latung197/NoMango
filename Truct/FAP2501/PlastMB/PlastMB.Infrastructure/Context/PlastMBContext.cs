using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using PlastMB.Domain.Entity;
using PlastMB.Domain;
using PlastMB.Utils;
using System.Reflection;
using PlastMB.Infrastructure.ContextAccessors;

namespace PlastMB.Infrastructure.Context
{
    public class PlastMBContext : DbContext
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
        public PlastMBContext(DbContextOptions<PlastMBContext> options
            , IConfiguration configuration
            , IUserPrincipalService userPrincipalService) : base(options)
        {
            _configuration = configuration;
            _userPrincipalService = userPrincipalService;
        }

        public PlastMBContext() : base()
        {

        }

        public PlastMBContext(DbContextOptions<PlastMBContext> options) : base(options)
        {

        }
        public PlastMBContext(IUserPrincipalService userPrincipalService) : base()
        {
            _userPrincipalService = userPrincipalService;
        }
        #endregion
        #region Method
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            string DbType = _configuration.GetConnectionString("DatabaseType");
            switch (DbType)
            {
                case "1"://MSSQL
                    optionsBuilder.UseLoggerFactory(loggerFactory).UseSqlServer(Environment.GetEnvironmentVariable("PlastMBContext"));
                    break;
                case "2"://Postgre
                    optionsBuilder.UseLoggerFactory(loggerFactory).UseNpgsql(_configuration.GetConnectionString("PlastMBContext"));
                    break;
                default://MSSQL
                    optionsBuilder.UseLoggerFactory(loggerFactory).UseSqlServer(Environment.GetEnvironmentVariable("PlastMBContext"));
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

                            entity.CreateTime = DateTime.Now.ToString("yyyyMMddHHmm");
                        }
                        else
                        {
                            //Update UpdateId, UpdateDt here
                            if (_userPrincipalService.IsAuthenticated)
                                entity.UpdateId = _userPrincipalService.UserId.ToString();

                            entity.UpdateTime = DateTime.Now.ToString("yyyyMMddHHmm");
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
        public DbSet<MstUser> MstUser { get; set; }
        public DbSet<TrnImportHistory> importHistories { get; set; }
        public DbSet<MstMachine> MstMachine { get; set; }
        public DbSet<MstFactory> MstFactory { get; set; }
        public DbSet<TrnOperationOee> trnOperationOees { get; set; }
        public DbSet<TrnOperationResult> trnOperationResults { get; set; }
        public DbSet<TrnImportHistoryDetail> trnImportHistoryDetails { get; set; }



        #endregion
    }
}
