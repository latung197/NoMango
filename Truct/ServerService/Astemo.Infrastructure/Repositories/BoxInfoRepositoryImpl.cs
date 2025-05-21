using Astemo.Domain.Interface;
using Astemo.Infrastructure.Context;
using Astemo.Domain.Entity;

namespace Astemo.Infrastructure.Repositories
{
    public class BoxInfoRepositoryImpl : AstemoBaseRepositoryImpl<BoxInfo>, IBoxInfoRepository
    {
        public BoxInfoRepositoryImpl(AstemoContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
