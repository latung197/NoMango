using Cody_v3.Repositories.Entities;
using Cody_v3.Repositories.Generics;
using Cody_v3.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;
using System.Reflection;
using System.Web;

namespace Cody_v3.Repositories.Contexts
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<RecloserData> RecloserDatas { get; set; }
        public DbSet<Recloser> Reclosers { get; set; }
        public DbSet<Department> Departments { get; set; }

        public System.Data.IDbConnection Connection => Database.GetDbConnection();

        protected override async void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (Type type in GetEntityTypes(typeof(BaseEntity)))
            {
                var method = SetGlobalQueryMethod.MakeGenericMethod(type);
                method.Invoke(this, new object[] { builder });
            }

            builder.Entity<Department>().HasIndex(i => i.DepartmentCode).IsUnique();
            builder.Entity<Department>().HasIndex(i => i.DepartmentName).IsUnique();
            builder.Entity<Department>().HasMany(r => r.Reclosers).WithOne(d => d.Department);

            builder.Entity<Recloser>().HasIndex(i => i.RecloserQualify).IsUnique();
            builder.Entity<Recloser>().HasMany(r => r.RecloserDatas).WithOne(d => d.Recloser);

            builder.Entity<RecloserData>().HasIndex(i => i.Date).IsClustered(false);
            builder.Entity<RecloserData>(r => r.HasIndex(i => new { i.SheetName, i.RecloserId, i.Date, i.Time }).IsUnique());


        }
        private static IList<Type> _entityTypeCache;
        private static IList<Type> GetEntityTypes(Type type)
        {
            if (_entityTypeCache != null && _entityTypeCache.First().BaseType == type)
            {
                return _entityTypeCache.ToList();
            }

            _entityTypeCache = (from a in GetReferencingAssemblies()
                                from t in a.DefinedTypes
                                where t.BaseType == type
                                select t.AsType()
                              ).ToList();
            return _entityTypeCache;
        }
        private static IEnumerable<Assembly> GetReferencingAssemblies()
        {
            var assemblies = new List<Assembly>();
            var dependencies = DependencyContext.Default.RuntimeLibraries;

            foreach (var library in dependencies)
            {
                try
                {
                    var assembly = Assembly.Load(new AssemblyName(library.Name));
                    assemblies.Add(assembly);
                }
                catch (FileNotFoundException)
                {
                }

            }
            return assemblies;
        }
        private static readonly MethodInfo SetGlobalQueryMethod = typeof(AppDbContext)
               .GetMethods(BindingFlags.Public | BindingFlags.Instance)
               .Single(t => t.IsGenericMethod && t.Name == "SetGlobalQuery");

        public void SetGlobalQuery<T>(ModelBuilder builder) where T : BaseEntity
        {
            builder.Entity<T>().Property(o => o.Id).HasDefaultValueSql("NEWID()");
            builder.Entity<T>().Property(o => o.CreatedDate).HasDefaultValueSql("GETDATE()");
            builder.Entity<T>().Property(o => o.UpdatedDate).HasDefaultValueSql("GETDATE()");
            builder.Entity<T>().Property(o => o.CreatedIPAddress).HasDefaultValueSql("''");
            builder.Entity<T>().Property(o => o.CreatedPCName).HasDefaultValueSql("''");
            builder.Entity<T>().Property(o => o.UpdatedIPAddress).HasDefaultValueSql("''");
            builder.Entity<T>().Property(o => o.UpdatedPCName).HasDefaultValueSql("''");
            builder.Entity<T>().Property(o => o.IsDeleted).HasDefaultValueSql("0");

        }

        public async Task<int> SaveChangesAsync()
        {
            var modifiedEnties = ChangeTracker.Entries().Where(e => e.State == EntityState.Modified);
            foreach (var item in modifiedEnties)
            {
                if (item.Entity is BaseEntity)
                {
                    var baseEntity = item.Entity as BaseEntity;
                    if (baseEntity != null)
                    {
                        baseEntity.UpdatedDate = DateTime.Now;
                    }
                }
            }

            var addedEnties = ChangeTracker.Entries().Where(e => e.State == EntityState.Added);
            foreach (var item in addedEnties)
            {
                if (item.Entity is BaseEntity)
                {
                    var baseEntity = item.Entity as BaseEntity;
                    if (baseEntity != null)
                    {
                        baseEntity.Id = Guid.NewGuid();
                        //baseEntity.CreatedIPAddress= HttpContext.Current.Request.UserHostAddress;
                        baseEntity.CreatedDate = DateTime.Now;
                        baseEntity.IsDeleted = false;

                        baseEntity.UpdatedDate = DateTime.Now;
                    }
                }
            }

            return await base.SaveChangesAsync();
        }
    }
}