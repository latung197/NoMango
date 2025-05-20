using PlastMB.Domain.Interface;
using PlastMB.Infrastructure.Context;
using PlastMB.Domain.Entity;

namespace PlastMB.Infrastructure.Repositories
{
    public class TrnOperationResultRepositoryImpl : PlastMBBaseRepositoryImpl<TrnOperationResult>, ITrnOperationResultRepository
    {
        public TrnOperationResultRepositoryImpl(PlastMBContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}