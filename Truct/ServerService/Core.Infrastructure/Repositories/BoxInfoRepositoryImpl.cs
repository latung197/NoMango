using Core.Domain.Interface;
using Core.Infrastructure.Context;
using Core.Domain.Entity;

namespace Core.Infrastructure.Repositories
{
    public class BoxInfoRepositoryImpl : CoreBaseRepositoryImpl<BoxInfo>, IBoxInfoRepository
    {
        public BoxInfoRepositoryImpl(CoreContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
