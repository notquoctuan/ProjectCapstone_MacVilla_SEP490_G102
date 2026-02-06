using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly MacvilladbContext _context;
        public ProductRepository(MacvilladbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetProductsForAdminAsync(string? name, decimal? minPrice, decimal? maxPrice, int? categoryId)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(p => p.Name.Contains(name));
            if (minPrice.HasValue) query = query.Where(p => p.Price >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(p => p.Price <= maxPrice.Value);
            if (categoryId.HasValue) query = query.Where(p => p.CategoryId == categoryId.Value);

            return await query.OrderByDescending(p => p.ProductId).ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(long id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task UpdateAsync(Product product) => _context.Products.Update(product);

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        // New: get featured/recent products; includes images and category
        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync(int limit)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .AsQueryable();

                query = query.Where(p => p.Status == "Active" || p.Status == null)
                         .OrderByDescending(p => p.CreatedAt);

            return await query.Take(limit).ToListAsync();
        }

    }
}