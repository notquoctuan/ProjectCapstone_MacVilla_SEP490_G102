using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProductsForAdminAsync(string? name, decimal? minPrice, decimal? maxPrice, int? categoryId);

        Task<Product?> GetByIdAsync(long id);

