using System.Linq.Expressions;

namespace Core.Domain.Interface
{
    public interface IBaseRepository<T> where T : class
    {
        #region Getting a list of entities

        List<T> GetAllList();

        Task<List<T>> GetAllListAsync();

        List<T> GetAllList(Expression<Func<T, bool>> expression);

        Task<List<T>> GetAllListAsync(Expression<Func<T, bool>> expression);

        IQueryable<T> GetAll();

        IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate = null);

        IQueryable<T> Get(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate = null,
             Expression<Func<T, bool>> predicate_orderBy = null);

        #endregion Getting a list of entities

        #region Getting single entity

        Task<T> GetAsync(object id);

        T Get(object id);

        T Single(Expression<Func<T, bool>> predicate);

        T SingleOrDefault(Expression<Func<T, bool>> predicate);

        Task<T> SingleAsync(Expression<Func<T, bool>> predicate);

        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);

        T FirstOrDefault(Expression<Func<T, bool>> predicate);

        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        #endregion Getting single entity

        #region Insert

        Task<T> InsertAsync(T entity);

        Task InsertAsync(IEnumerable<T> entities);
        Task BulkInsertAsync(IEnumerable<T> entities);
        #endregion Insert

        #region Update

        Task UpdateAsync(T entity);

        Task UpdateAsync(IEnumerable<T> entities);
        Task BulkUpdateAsync(IEnumerable<T> entities);
        void BulkInsertOrUpdate(IEnumerable<T> entities);
        Task BulkInsertOrUpdateAsync(IEnumerable<T> entities);
        #endregion Update

        #region Delete

        void Delete(T entity);

        Task DeleteAsync(T entity);

        void Delete(IEnumerable<T> entity);

        Task DeleteAsync(IEnumerable<T> entity);

        void Delete(Expression<Func<T, bool>> predicate);

        Task DeleteAsync(Expression<Func<T, bool>> predicate);
        Task BulkDeleteAsync(IEnumerable<T> entities);
        #endregion Delete

        #region Other

        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);

        int Count();

        Task<int> CountAsync();

        int Count(Expression<Func<T, bool>> predicate);

        Task<int> CountAsync(Expression<Func<T, bool>> predicate);

        long LongCount();

        Task<long> LongCountAsync();

        long LongCount(Expression<Func<T, bool>> predicate);

        Task<long> LongCountAsync(Expression<Func<T, bool>> predicate);

        IQueryable<T> ExecuteStoreProc(string procName, object[] prs);
        #endregion Other
    }
}
