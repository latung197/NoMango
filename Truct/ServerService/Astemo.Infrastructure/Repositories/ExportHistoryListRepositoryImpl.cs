using Astemo.Domain.Interface;
using Astemo.Infrastructure.Context;
using Astemo.Domain.Entity;

namespace Astemo.Infrastructure.Repositories
{
    public class ExportHistoryListRepositoryImpl : AstemoBaseRepositoryImpl<ExportHistoryList>, IExportHistoryListRepository
    {
        public ExportHistoryListRepositoryImpl(AstemoContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
