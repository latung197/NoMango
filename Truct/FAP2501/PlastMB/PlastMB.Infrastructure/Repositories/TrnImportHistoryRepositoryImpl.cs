using PlastMB.Domain.Interface;
using PlastMB.Infrastructure.Context;
using PlastMB.Domain.Entity;

namespace PlastMB.Infrastructure.Repositories
{
    public class TrnImportHistoryRepositoryImpl : PlastMBBaseRepositoryImpl<TrnImportHistory>, ITrnImportHistoryRepository
    {
        public TrnImportHistoryRepositoryImpl(PlastMBContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
