using Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProductsForAdminAsync(string? name, decimal? minPrice, decimal? maxPrice, int? categoryId);

        Task<Product?> GetByIdAsync(long id);

        Task UpdateAsync(Product product);
        Task SaveChangesAsync();

        //new
        async Task<IEnumerable<Product>> GetFeaturedProductsAsync(int limit)
        {
            var all = await GetProductsForAdminAsync(null, null, null, null);
            if (all == null) return Enumerable.Empty<Product>();

            return all
                .OrderByDescending(p => p.CreatedAt)
                .Take(limit);
        }
    }
}