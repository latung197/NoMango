using PlastMB.Domain.Interface;
using PlastMB.Infrastructure.Context;
using PlastMB.Domain.Entity;

namespace PlastMB.Infrastructure.Repositories
{
    public class TrnOperationOeeRepositoryImpl : PlastMBBaseRepositoryImpl<TrnOperationOee>, ITrnOperationOeeRepository
    {
        public TrnOperationOeeRepositoryImpl(PlastMBContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}