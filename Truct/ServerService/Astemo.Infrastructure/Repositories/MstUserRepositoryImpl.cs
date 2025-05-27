using Core.Domain.Interface;
using Core.Infrastructure.Context;
using Core.Domain.Entity;

namespace Core.Infrastructure.Repositories
{
    public class MstUserRepositoryImpl : AstemoBaseRepositoryImpl<MstUser>, IMstUserRepository
    {
        public MstUserRepositoryImpl(AstemoContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
