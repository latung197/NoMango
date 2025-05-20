using PlastMB.Domain.Interface;
using PlastMB.Infrastructure.Context;
using PlastMB.Domain.Entity;

namespace PlastMB.Infrastructure.Repositories
{
    public class TrnImportHistoryDetailRepositoryImpl : PlastMBBaseRepositoryImpl<TrnImportHistoryDetail>, ITrnImportHistoryDetailRepository
    {
        public TrnImportHistoryDetailRepositoryImpl(PlastMBContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
