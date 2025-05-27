using Core.Domain.Interface;
using Core.Infrastructure.Context;
using Core.Domain.Entity;

namespace Core.Infrastructure.Repositories
{
    public class EcuDataRepositoryImpl : CoreBaseRepositoryImpl<EcuData>, IEcuDataRepository
    {
        public EcuDataRepositoryImpl(CoreContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
