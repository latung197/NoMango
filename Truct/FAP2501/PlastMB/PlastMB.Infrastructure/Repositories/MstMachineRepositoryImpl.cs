using PlastMB.Domain.Interface;
using PlastMB.Infrastructure.Context;
using PlastMB.Domain.Entity;

namespace PlastMB.Infrastructure.Repositories
{
    public class MstMachineRepositoryImpl : PlastMBBaseRepositoryImpl<MstMachine>, IMstMachineRepository
    {
        public MstMachineRepositoryImpl(PlastMBContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}