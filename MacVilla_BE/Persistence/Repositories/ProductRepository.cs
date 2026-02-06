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

        public async Task<IEnumerable<Product>> GetProductsForAdminAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .ToListAsync();
        }

    }
}