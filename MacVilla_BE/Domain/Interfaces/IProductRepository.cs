using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProductsForAdminAsync();
        Task<Product?> GetByIdDetailAsync(long id);
        Task<Product> AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task<Product?> GetByIdAsync(long id);
        Task<bool> UpdateStatusAsync(long id, string status);
        IQueryable<Product> GetQueryable();

    }
}