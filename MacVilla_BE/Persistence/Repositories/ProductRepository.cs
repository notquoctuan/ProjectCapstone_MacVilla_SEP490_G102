using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly MacvilladbContext _ctx;

        public ProductRepository(MacvilladbContext ctx) => _ctx = ctx;

        public IQueryable<Product> GetQueryable()
            => _ctx.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .AsNoTracking();

        public async Task<Product?> GetByIdAsync(long id)
            => await _ctx.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ProductId == id);

        public async Task<Product?> GetByIdDetailAsync(long id)
            => await _ctx.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductSpecifications)
                .FirstOrDefaultAsync(p => p.ProductId == id);

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            var query = _ctx.Products.Where(p => p.Name == name.Trim());
            if (excludeId.HasValue)
                query = query.Where(p => p.ProductId != excludeId.Value);
            return await query.AnyAsync();
        }

        public async Task<Product> AddAsync(Product product)
        {
            await _ctx.Products.AddAsync(product);
            await _ctx.SaveChangesAsync();
            return product;
        }

        public async Task UpdateAsync(Product product)
        {
            _ctx.Products.Update(product);
            await _ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            _ctx.Products.Remove(product);
            await _ctx.SaveChangesAsync();
        }

        public async Task<bool> UpdateStatusAsync(long id, string status)
        {
            var product = await _ctx.Products.FindAsync(id);
            if (product == null) return false;
            product.Status = status;
            await _ctx.SaveChangesAsync();
            return true;
        }

        // ── Quản lý ảnh ───────────────────────────────────────────────
        public async Task<ProductImage?> GetImageAsync(long imageId)
            => await _ctx.ProductImages.FirstOrDefaultAsync(i => i.ImageId == imageId);

        public async Task AddImageAsync(ProductImage image)
            => await _ctx.ProductImages.AddAsync(image);

        public Task DeleteImageAsync(ProductImage image)
        {
            _ctx.ProductImages.Remove(image);
            return Task.CompletedTask;
        }

        public async Task SaveAsync()
            => await _ctx.SaveChangesAsync();
    }
}