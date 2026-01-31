using Domain.Interfaces;
using Application.DTOs;

namespace Application.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepo;
        public ProductService(IProductRepository productRepo) => _productRepo = productRepo;

        public async Task<IEnumerable<ProductAdminResponse>> SearchProductsForAdmin(string? name, decimal? min, decimal? max, int? catId)
        {
            var products = await _productRepo.GetProductsForAdminAsync(name, min, max, catId);

            return products.Select(p => new ProductAdminResponse
            {
                ProductId = p.ProductId,
                Name = p.Name,
                CategoryName = p.Category?.CategoryName,
                Price = p.Price ?? 0,
                Status = p.Status,  
                CreatedAt = p.CreatedAt
            });
        }
        public async Task<ProductAdminResponse?> GetProductDetailsAsync(long id)
        {
            var product = await _productRepo.GetByIdAsync(id);

            if (product == null) return null;

            return new ProductAdminResponse
            {
                ProductId = product.ProductId,
                Name = product.Name,
                CategoryName = product.Category?.CategoryName,
                Price = product.Price ?? 0,
                Status = product.Status,
                CreatedAt = product.CreatedAt
            };
        }
