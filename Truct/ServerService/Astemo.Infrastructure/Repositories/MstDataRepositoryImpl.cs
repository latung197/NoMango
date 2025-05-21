using Astemo.Domain.Interface;
using Astemo.Infrastructure.Context;
using Astemo.Domain.Entity;

namespace Astemo.Infrastructure.Repositories
{
    public class MstDataRepositoryImpl : AstemoBaseRepositoryImpl<MstData>, IMstDataRepository
    {
        public MstDataRepositoryImpl(AstemoContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}