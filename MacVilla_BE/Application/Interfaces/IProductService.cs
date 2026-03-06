using Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductAdminResponse> CreateProductAsync(ProductCreateRequest request, string webRootPath);
        Task<ProductDetailResponse?> GetProductDetailAsync(long id);
        Task<PagedResponse<ProductAdminResponse>> GetPagedProductsAsync(ProductSearchRequest request);
        Task UpdateProductAsync(long id, ProductUpdateRequest request, string webRootPath);
        Task ChangeStatusAsync(long id, string newStatus);
        Task DeleteProductAsync(long id, string webRootPath);

        // ── Quản lý ảnh riêng lẻ ──────────────────────────────────────
        Task<List<ProductImageResponse>> AddImagesAsync(long productId, List<IFormFile> files, string webRootPath);
        Task DeleteImageAsync(long productId, long imageId, string webRootPath);
        Task SetMainImageAsync(long productId, long imageId);
    }
}