using Domain.Interfaces;
using Application.DTOs;

namespace Application.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<IEnumerable<ProductAdminResponse>> GetAllProductsForAdmin()
        {
            var products = await _productRepository.GetProductsForAdminAsync();

            return products.Select(p => new ProductAdminResponse
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                CategoryName = p.Category?.CategoryName ?? "N/A",
                ImageUrl = p.ProductImages?
                            .OrderByDescending(img => img.IsMain)
                            .Select(img => img.ImageUrl)
                            .FirstOrDefault()
            });
        }

        public async Task<PagedResponse<ProductAdminResponse>> GetPagedProductsForAdminAsync(ProductSearchRequest request)
        {
            // Lấy dữ liệu thô từ Repository (giữ nguyên code repository cũ)
            var products = await _productRepository.GetProductsForAdminAsync();
            var query = products.AsQueryable();

            // 1. Lọc theo tên (đã qua validate Regex ở DTO)
            if (!string.IsNullOrEmpty(request.Name))
            {
                query = query.Where(p => p.Name != null && p.Name.Contains(request.Name, StringComparison.OrdinalIgnoreCase));
            }

            // 2. Lọc theo giá
            if (request.MinPrice.HasValue) query = query.Where(p => p.Price >= request.MinPrice.Value);
            if (request.MaxPrice.HasValue) query = query.Where(p => p.Price <= request.MaxPrice.Value);

            // 3. Mapping sang DTO (Sửa lỗi CS8072 bằng cách không dùng ?.)
            var mappedData = query.Select(p => new ProductAdminResponse
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                // Thay p.Category?.CategoryName bằng:
                CategoryName = p.Category != null ? p.Category.CategoryName : "N/A",
                // Thay p.ProductImages?. bằng kiểm tra null:
                ImageUrl = p.ProductImages != null
                    ? p.ProductImages.OrderByDescending(img => img.IsMain)
                                     .Select(img => img.ImageUrl)
                                     .FirstOrDefault()
                    : null
            });

            if (!string.IsNullOrEmpty(request.CategoryName))
            {
                mappedData = mappedData.Where(r => r.CategoryName != null &&
                    r.CategoryName.Contains(request.CategoryName, StringComparison.OrdinalIgnoreCase));
            }

            // 5. Thực hiện phân trang
            int totalCount = mappedData.Count();
            var items = mappedData
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new PagedResponse<ProductAdminResponse>
            {
                Data = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

    }
    
}