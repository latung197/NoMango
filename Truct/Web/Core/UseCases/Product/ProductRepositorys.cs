using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces.IProductsRepository;
using Core.Entities;
using Public;
namespace Core.UseCases.Product
{
    public class ProductRepositorys : IProductsRepository<Products>
    {
       
        public ProductRepositorys()
        {
           
        }

        public Task AddAsync(Products entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Products>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<List<Products>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Products> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Products entity)
        {
            throw new NotImplementedException();
        }
    }
}
