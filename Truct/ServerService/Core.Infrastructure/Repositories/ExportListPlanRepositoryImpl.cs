using Core.Domain.Interface;
using Core.Infrastructure.Context;
using Core.Domain.Entity;

namespace Core.Infrastructure.Repositories
{
    public class ExportListPlanRepositoryImpl : CoreBaseRepositoryImpl<ExportListPlan>, IExportListPlanRepository
    {
        public ExportListPlanRepositoryImpl(CoreContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
