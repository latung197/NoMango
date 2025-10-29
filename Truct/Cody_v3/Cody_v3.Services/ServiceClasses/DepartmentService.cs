using Cody_v3.Repositories.Entities;
using Cody_v3.Repositories.Interfaces;
using Cody_v3.Services.Generics;
using Cody_v3.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cody_v3.Services.ServiceClasses
{
    public  class DepartmentService : GenericService<Department>, IDepartmentService
    {
        public DepartmentService(IGenericRepository<Department> repository) : base(repository)
        {
        }
    }
}
