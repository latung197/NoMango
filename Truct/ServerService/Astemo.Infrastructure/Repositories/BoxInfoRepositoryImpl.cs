using Core.Domain.Interface;
using Core.Infrastructure.Context;
using Core.Domain.Entity;

namespace Core.Infrastructure.Repositories
{
    public class BoxInfoRepositoryImpl : AstemoBaseRepositoryImpl<BoxInfo>, IBoxInfoRepository
    {
        public BoxInfoRepositoryImpl(AstemoContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
