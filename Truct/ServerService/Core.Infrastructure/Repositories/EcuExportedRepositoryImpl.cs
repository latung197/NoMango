using Core.Domain.Interface;
using Core.Infrastructure.Context;
using Core.Domain.Entity;

namespace Core.Infrastructure.Repositories
{
    public class EcuExportedRepositoryImpl : CoreBaseRepositoryImpl<EcuExported>, IEcuExportedRepository
    {
        public EcuExportedRepositoryImpl(CoreContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
