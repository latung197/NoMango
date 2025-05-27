using Core.Domain.Interface;
using Core.Infrastructure.Context;
using Core.Domain.Entity;

namespace Core.Infrastructure.Repositories
{
    public class MstDataRepositoryImpl : AstemoBaseRepositoryImpl<MstData>, IMstDataRepository
    {
        public MstDataRepositoryImpl(AstemoContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}