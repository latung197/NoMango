using Cody_v3.Repositories.Contexts;
using Cody_v3.Repositories.Entities;
using Cody_v3.Repositories.Interfaces;
using Cody_v3.Repositories.Paging;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace Cody_v3.Repositories.Generics
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        internal AppDbContext dbContext;
        internal DbSet<T> dbSet;

        public GenericRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<T>();
        }

        public async Task<int> Delete(T entityToDelete)
        {
            if (dbContext.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
            return await dbContext.SaveChangesAsync();
        }

        public async Task<int> Delete(Guid Id)
        {
            T entityToDelete = await dbSet.FindAsync(Id);
            if(entityToDelete!=null)
                return await this.Delete(entityToDelete);
            else 
                return -1;
        }

        public async Task<int> Delete(string Id)
        {
            T entityToDelete = await dbSet.FindAsync(Id);
            if (entityToDelete != null)
                return await this.Delete(entityToDelete);
            else
                return -1;
        }

        public async Task<int> Delete(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            dbSet.RemoveRange(query);
            return await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await dbSet.ToListAsync();
        } 

        public async Task<IEnumerable<T>> GetByCondition(Expression<Func<T, bool>> filter=null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            return await GetByConditionQueryable(filter, orderBy, includeProperties).ToListAsync();
        }

        public IQueryable<T> GetByConditionQueryable(Expression<Func<T, bool>> filter=null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split( new char[] { ','}, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query);
            }

            return query;
        }

        public async Task<T> GetById(Guid Id)
        {
            return await dbSet.FindAsync(Id);
        }

        public async Task<T> GetById(string Id)
        {
            return await dbSet.FindAsync(Id);
        }

        public async Task<PageResult<T>> GetWithPaging(int page, int pageSize, Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            IQueryable<T> query = GetByConditionQueryable(filter, orderBy, includeProperties);
            var result = new PageResult<T>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = query.Count();

            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;
            result.Results = await query.Skip(skip).Take(pageSize).ToListAsync();

            return result;
        }

        public async Task<int> Insert(T entity)
        {
            dbSet.Add(entity);
            return await dbContext.SaveChangesAsync();
        }

        public async Task<int> Insert(List<T> entities)
        {
            await dbSet.AddRangeAsync(entities);
            return await dbContext.SaveChangesAsync();
        }

        public async Task<int> SoftDelete(Guid Id)
        {
            var entity = await this.GetById(Id);
            if (entity != null)
            {
                var baseEntity = entity as BaseEntity;
                if (baseEntity != null)
                {
                    baseEntity.IsDeleted = true;
                    return await Update(entity);
                }
                else return -1;
            }
            else
                return -1;
        }

        public async Task<int> SoftDelete(string Id)
        {
            var entity = await this.GetById(Id);
            if (entity != null)
            {
                var baseEntity = entity as BaseEntity;
                if (baseEntity != null)
                {
                    baseEntity.IsDeleted = true;
                    return await Update(entity);
                }
                else return -1;
            }
            else
                return -1;
        }

        public async Task<int> Update(T entity)
        {
            dbSet.Attach(entity);
            dbContext.Entry(entity).State = EntityState.Modified;
            return await dbContext.SaveChangesAsync() ;
        }

        public async Task<int> Update(List<T> entities)
        {
            foreach (var entity in entities)
            {
                dbSet.Attach(entity);
                dbContext.Entry(entity).State = EntityState.Modified;
            }
            return await dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Mapping a object params to SqlParameter[]
        /// </summary>
        /// <param name="storedProcedureName">Stored procedure name</param>
        /// <param name="parameter">Object parameter. Ex for @fromDate, @toDate => parameter: new { fromDate = DateTime.Now, toDate = DateTime.Now}</param>
        /// <returns>Returns list of T otherwise returns null</returns>
        public async Task<List<T>> ExecProcDataAsync(string storedProcedureName, object parameter)
        {
            var parameters = MappingParameter(ref storedProcedureName, parameter);
            var results = dbSet.FromSqlRaw(storedProcedureName, parameters);
            return await results.ToListAsync();
        }

        /// <summary>
        /// Mapping a object params to SqlParameter[]
        /// </summary>
        /// <param name="storedProcedureName">Stored procedure name</param>
        /// <param name="parameter">Object parameter. Ex for @fromDate, @toDate => parameter: new { fromDate = DateTime.Now, toDate = DateTime.Now}</param>
        /// <returns>Returns rows affected otherwise returns -1</returns>
        public async Task<int> ExecProcNonqueryAsync(string storedProcedureName, object parameter)
        {
            var parameters = MappingParameter(ref storedProcedureName, parameter);
            return await dbContext.Database.ExecuteSqlRawAsync(storedProcedureName, parameters);
        }

        /// <summary>
        /// Mapping a object params to SqlParameter[]
        /// </summary>
        /// <param name="storedProcedureName">Stored procedure name</param>
        /// <param name="parameter">Object parameter. Ex for @fromDate, @toDate => parameter: new { fromDate = DateTime.Now, toDate = DateTime.Now}</param>
        /// <returns>Returns SqlParameter[] otherwise returns null</returns>
        private SqlParameter[]? MappingParameter(ref string storedProcedureName, object parameter)
        {
            SqlParameter[] parameters = null;
            string joinParam = "";
            if (parameter != null)
            {
                int i = 0;
                Type type= parameter.GetType();
                IList<PropertyInfo> props = new List<PropertyInfo>(type.GetProperties());
                parameters = new SqlParameter[props.Count];
                foreach (PropertyInfo prop in props)
                {
                    SqlParameter param = new SqlParameter($"@{prop.Name}", prop.GetValue(parameter));
                    parameters[i] = param;
                    i++;
                    joinParam += joinParam == string.Empty ? $" @{prop.Name}" : $", @{prop.Name}";
                }
            }
            storedProcedureName += joinParam;
            return parameters;
        }
    }
}
