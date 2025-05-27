using Core.Domain.Interface;
using Core.Infrastructure.Context;
using Core.Domain.Entity;

namespace Core.Infrastructure.Repositories
{
    public class EcuExportedRepositoryImpl : AstemoBaseRepositoryImpl<EcuExported>, IEcuExportedRepository
    {
        public EcuExportedRepositoryImpl(AstemoContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
