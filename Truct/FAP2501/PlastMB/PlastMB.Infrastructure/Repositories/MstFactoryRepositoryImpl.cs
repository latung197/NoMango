using PlastMB.Domain.Interface;
using PlastMB.Infrastructure.Context;
using PlastMB.Domain.Entity;

namespace PlastMB.Infrastructure.Repositories
{
    public class MstFactoryRepositoryImpl : PlastMBBaseRepositoryImpl<MstFactory>, IMstFactoryRepository
    {
        public MstFactoryRepositoryImpl(PlastMBContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}