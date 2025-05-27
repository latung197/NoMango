using Core.Domain.Interface;
using Core.Infrastructure.Context;
using Core.Domain.Entity;

namespace Core.Infrastructure.Repositories
{
    public class MstUserRepositoryImpl : CoreBaseRepositoryImpl<MstUser>, IMstUserRepository
    {
        public MstUserRepositoryImpl(CoreContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
