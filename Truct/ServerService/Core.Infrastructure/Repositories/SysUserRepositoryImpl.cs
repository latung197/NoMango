using Core.Domain.Interface;
using Core.Infrastructure.Context;
using Core.Domain.Entity;
using Core.Domain.Entity.SystemEntities;

namespace Core.Infrastructure.Repositories
{
    public class SysUserRepositoryImpl : CoreBaseRepositoryImpl<SysUser>, ISysUserRepository
    {
        public SysUserRepositoryImpl(CoreContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
