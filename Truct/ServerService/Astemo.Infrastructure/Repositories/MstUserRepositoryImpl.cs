using Astemo.Domain.Interface;
using Astemo.Infrastructure.Context;
using Astemo.Domain.Entity;

namespace Astemo.Infrastructure.Repositories
{
    public class MstUserRepositoryImpl : AstemoBaseRepositoryImpl<MstUser>, IMstUserRepository
    {
        public MstUserRepositoryImpl(AstemoContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
