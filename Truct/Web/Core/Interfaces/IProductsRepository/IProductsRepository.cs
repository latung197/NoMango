using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces.IRepositoryBase;

namespace Core.Interfaces.IProductsRepository
{
    public interface IProductsRepository<T> :IRepositoryBase<Products>
    {
        Task<IEnumerable<T>> GetAll();
    }
}
