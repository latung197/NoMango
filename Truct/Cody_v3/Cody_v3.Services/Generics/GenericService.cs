using Cody_v3.Repositories.Interfaces;
using Cody_v3.Repositories.Paging;
using Cody_v3.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Linq.Expressions;

namespace Cody_v3.Services.Generics
{
    public class GenericService<T> : Hub, IGenericService<T> where T : class
    {
        public readonly IGenericRepository<T> repository;
        public GenericService(IGenericRepository<T> repository)
        {
            this.repository= repository;
        }
        public async Task<int> Delete(Guid Id)
        {
            return await repository.Delete(Id);
        }

        public async Task<int> Delete(string Id)
        {
            return await repository.Delete(Id);
        }

        public async Task<int> Delete(Expression<Func<T, bool>> filter = null)
        {
            return await repository.Delete(filter);
        }

        public async Task<int> ExecProcNonqueryAsync(string storedProcedureName, object parameter)
        {
            return await repository.ExecProcNonqueryAsync(storedProcedureName, parameter);
        }

        public async Task<List<T>> ExecProcDataAsync(string storedProcedureName, object parameter)
        {
            return await repository.ExecProcDataAsync(storedProcedureName, parameter);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await repository.GetAll();
        }

        public async Task<IEnumerable<T>> GetByCondition(Expression<Func<T, bool>> filter=null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            return await repository.GetByCondition(filter, orderBy, includeProperties);
        }

        public IQueryable<T> GetByConditionQueryable(Expression<Func<T, bool>> filter=null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            return repository.GetByConditionQueryable(filter, orderBy, includeProperties);
        }

        public async Task<T> GetById(Guid Id)
        {
            return await repository.GetById(Id);
        }

        public async Task<T> GetById(string Id)
        {
            return await repository.GetById(Id);
        }

        public async Task<PageResult<T>> GetWithPaging(int page, int pageSize, Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            return await repository.GetWithPaging(page, pageSize, filter, orderBy, includeProperties);
        }

        public async Task<int> Insert(T entity)
        {
            return await repository.Insert(entity);
        }

        public async Task<int> Insert(List<T> entities)
        {
            return await repository.Insert(entities);
        }

        public async Task<int> SoftDelete(Guid Id)
        {
            return await repository.SoftDelete(Id);
        }

        public async Task<int> SoftDelete(string Id)
        {
            return await repository.SoftDelete(Id);
        }

        public async Task<int> Update(T entity)
        {
            return await repository.Update(entity);
        }

        public async Task<int> Update(List<T> entities)
        {
            return await repository.Update(entities);
        }
    }
}
