using PlastMB.Domain.Interface;
using PlastMB.Infrastructure.Context;
using PlastMB.Domain.Entity;

namespace PlastMB.Infrastructure.Repositories
{
    public class MstUserRepositoryImpl : PlastMBBaseRepositoryImpl<MstUser>, IMstUserRepository
    {
        public MstUserRepositoryImpl(PlastMBContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
