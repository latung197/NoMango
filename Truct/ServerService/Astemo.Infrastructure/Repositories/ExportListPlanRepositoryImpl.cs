using Astemo.Domain.Interface;
using Astemo.Infrastructure.Context;
using Astemo.Domain.Entity;

namespace Astemo.Infrastructure.Repositories
{
    public class ExportListPlanRepositoryImpl : AstemoBaseRepositoryImpl<ExportListPlan>, IExportListPlanRepository
    {
        public ExportListPlanRepositoryImpl(AstemoContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
