using Core.Domain.Entity.SystemEntities;
using Core.Domain.Interface;
using Core.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Repositories.SysRepositories
{
    public class SysUserCommandRepositorylmpl:CoreBaseRepositoryImpl<SysUserCommand>,ISysUserCommand
    {
        public SysUserCommandRepositorylmpl(CoreContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
