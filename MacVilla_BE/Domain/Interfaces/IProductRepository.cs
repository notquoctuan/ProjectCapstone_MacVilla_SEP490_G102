using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IProductRepository
    {
        IQueryable<Product> GetQueryable();
        Task<Product?> GetByIdAsync(long id);
        Task<Product?> GetByIdDetailAsync(long id);
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);
        Task<Product> AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
        Task<bool> UpdateStatusAsync(long id, string status);

        // ── Quản lý ảnh ───────────────────────────────────────────────
        Task<ProductImage?> GetImageAsync(long imageId);
        Task AddImageAsync(ProductImage image);
        Task DeleteImageAsync(ProductImage image);
        Task SaveAsync();
    }
}