using Core.Domain.Interface;
using Core.Infrastructure.Context;
using Core.Domain.Entity;

namespace Core.Infrastructure.Repositories
{
    public class ExportHistoryListRepositoryImpl : AstemoBaseRepositoryImpl<ExportHistoryList>, IExportHistoryListRepository
    {
        public ExportHistoryListRepositoryImpl(AstemoContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
