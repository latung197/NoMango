using Astemo.Domain.Interface;
using Astemo.Infrastructure.Context;
using Astemo.Domain.Entity;

namespace Astemo.Infrastructure.Repositories
{
    public class EcuDataRepositoryImpl : AstemoBaseRepositoryImpl<EcuData>, IEcuDataRepository
    {
        public EcuDataRepositoryImpl(AstemoContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
