using Core.Infrastructure.Context;
using Core.Domain.Entity;
using Core.Domain.Entity.SystemEntities;
using Core.Domain.Interface.SystemInterface;

namespace Core.Infrastructure.Repositories.SysRepositories
{
    public class SysUserRepositoryImpl : CoreBaseRepositoryImpl<SysUser>, ISysUserRepository
    {
        public SysUserRepositoryImpl(CoreContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
