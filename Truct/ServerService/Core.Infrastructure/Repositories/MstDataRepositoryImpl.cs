using Core.Domain.Interface;
using Core.Infrastructure.Context;
using Core.Domain.Entity;

namespace Core.Infrastructure.Repositories
{
    public class MstDataRepositoryImpl : CoreBaseRepositoryImpl<MstData>, IMstDataRepository
    {
        public MstDataRepositoryImpl(CoreContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}