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
