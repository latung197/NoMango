using Astemo.Domain.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using EFCore.BulkExtensions;

namespace Astemo.Infrastructure.Repositories
{
    public abstract class BaseRepositoryImpl<T> : IBaseRepository<T> where T : class
    {
        protected DbContext Context { get; set; }
        public BaseRepositoryImpl(DbContext repositoryContext)
        {
            Context = repositoryContext;
            Context.Database.SetCommandTimeout(120);
        }

        #region Getting a list of entities

        public List<T> GetAllList()
        {
            return Context.Set<T>().ToList();
        }

        public async Task<List<T>> GetAllListAsync()
        {
            return await Context.Set<T>().ToListAsync();
        }

        public List<T> GetAllList(Expression<Func<T, bool>> expression)
        {
            return Context.Set<T>().Where(expression).ToList();
        }

        public async Task<List<T>> GetAllListAsync(Expression<Func<T, bool>> expression)
        {
            return await Context.Set<T>().Where(expression).ToListAsync();
        }

        public IQueryable<T> GetAll()
        {
            return Context.Set<T>();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate != null)
            {
                return GetAll().Where(predicate).AsEnumerable();
            }
            return GetAll().AsEnumerable();
        }


        public IQueryable<T> Get(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate = null,
            Expression<Func<T, bool>> predicate_orderBy = null)
        {
            var iQueryResult = GetAll();
            if (predicate != null)
            {
                iQueryResult = iQueryResult.Where(predicate);
            }
            if (predicate_orderBy != null)
            {
                iQueryResult = iQueryResult.OrderBy(predicate_orderBy);
            }
            return
                iQueryResult
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);

        }

        #endregion Getting a list of entities

        #region Getting single entity

        public async Task<T> GetAsync(object id)
        {
            return await Context.Set<T>().FindAsync(id);
        }

        public T Get(object id)
        {
            return Context.Set<T>().Find(id);
        }

        public T Single(Expression<Func<T, bool>> predicate)
        {
            return GetAll().Single(predicate);
        }

        public T SingleOrDefault(Expression<Func<T, bool>> predicate)
        {
            return GetAll().SingleOrDefault(predicate);
        }

        public async Task<T> SingleAsync(Expression<Func<T, bool>> predicate)
        {
            return await GetAll().SingleAsync(predicate);
        }

        public async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await GetAll().SingleOrDefaultAsync(predicate);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await GetAll().FirstOrDefaultAsync(predicate);
        }

        #endregion Getting single entity

        #region Insert

        public async Task<T> InsertAsync(T entity)
        {
            var x = await Context.Set<T>().AddAsync(entity);
            return x.Entity;
        }

        public async Task InsertAsync(IEnumerable<T> entities)
        {
            await Context.Set<T>().AddRangeAsync(entities);
        }
        public async Task BulkInsertAsync(IEnumerable<T> entities)
        {
            await Context.BulkInsertAsync(entities.ToList());
        }
        #endregion Insert

        #region Update

        public async Task UpdateAsync(T entity)
        {
            await Task.Run(() =>
            {
                Context.Set<T>().Update(entity);
            });
        }

        public async Task UpdateAsync(IEnumerable<T> entities)
        {
            await Task.Run(() =>
            {
                Context.Set<T>().UpdateRange(entities);
            });
        }
        public async Task BulkUpdateAsync(IEnumerable<T> entities)
        {
            await Context.BulkUpdateAsync<T>(entities.ToList());
        }
        public void BulkInsertOrUpdate(IEnumerable<T> entities)
        {
            Context.BulkInsertOrUpdate(entities.ToList());
        }
        public async Task BulkInsertOrUpdateAsync(IEnumerable<T> entities)
        {
            await Context.BulkInsertOrUpdateAsync<T>(entities.ToList());
        }
        #endregion Update

        #region Delete

        public void Delete(T entity)
        {
            //AttachIfNot(AttachIfNot);
            Context.Set<T>().Remove(entity);
        }

        public async Task DeleteAsync(T entity)
        {
            await Task.Run(() =>
            {
                Delete(entity);
            });
        }
        public async Task BulkDeleteAsync(IEnumerable<T> entities)
        {
            await Context.BulkDeleteAsync(entities.ToList());
        }
        public void Delete(IEnumerable<T> entities)
        {
            Context.Set<T>().RemoveRange(entities);
        }

        public async Task DeleteAsync(IEnumerable<T> entities)
        {
            await Task.Run(() =>
            {
                Delete(entities);
            });
        }

        public void Delete(Expression<Func<T, bool>> predicate)
        {
            var entities = GetAll().Where(predicate).ToList();
            Context.Set<T>().RemoveRange(entities);
        }

        public async Task DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            var entities = await GetAll().Where(predicate).ToListAsync();
            Context.Set<T>().RemoveRange(entities);

        }

        #endregion Delete

        #region Other

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            return await Context.Set<T>().AnyAsync(expression);
        }

        public int Count()
        {
            return GetAll().Count();
        }

        public async Task<int> CountAsync()
        {
            return await GetAll().CountAsync();
        }

        public int Count(Expression<Func<T, bool>> predicate)
        {
            return GetAll().Count(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await GetAll().CountAsync(predicate);
        }

        public long LongCount()
        {
            return GetAll().LongCount();
        }

        public async Task<long> LongCountAsync()
        {
            return await GetAll().LongCountAsync();
        }

        public long LongCount(Expression<Func<T, bool>> predicate)
        {
            return GetAll().LongCount(predicate);
        }

        public async Task<long> LongCountAsync(Expression<Func<T, bool>> predicate)
        {
            return await GetAll().LongCountAsync(predicate);
        }

        public IQueryable<T> ExecuteStoreProc(string procName, object[] prs)
        {
            return Context.Set<T>().FromSqlRaw(procName, prs).AsQueryable();
        }
        #endregion Other
    }
}
