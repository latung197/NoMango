using Core.Domain.Interface;
using Core.Infrastructure.Context;
using Core.Domain.Entity;

namespace Core.Infrastructure.Repositories
{
    public class EcuDataRepositoryImpl : AstemoBaseRepositoryImpl<EcuData>, IEcuDataRepository
    {
        public EcuDataRepositoryImpl(AstemoContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
