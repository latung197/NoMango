using Core.Domain.Interface;
using Core.Infrastructure.Context;
using Core.Domain.Entity;

namespace Core.Infrastructure.Repositories
{
    public class ExportHistoryListRepositoryImpl : CoreBaseRepositoryImpl<ExportHistoryList>, IExportHistoryListRepository
    {
        public ExportHistoryListRepositoryImpl(CoreContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
