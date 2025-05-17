using Astemo.Domain.Interface;
using Astemo.Infrastructure.Context;
using Astemo.Domain.Entity;

namespace Astemo.Infrastructure.Repositories
{
    public class EcuExportedRepositoryImpl : AstemoBaseRepositoryImpl<EcuExported>, IEcuExportedRepository
    {
        public EcuExportedRepositoryImpl(AstemoContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
