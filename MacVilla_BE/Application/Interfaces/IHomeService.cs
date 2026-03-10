using Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces;

public interface IHomeService
{
    Task<IEnumerable<BannerResponse>> GetActiveBannersAsync();
    Task<PagedResponse<ProductAdminResponse>> SearchProductsAsync(ProductSearchPublicRequest request);
    Task<IEnumerable<ProductAdminResponse>> GetFeaturedProductsAsync(int limit = 8);
    Task<IEnumerable<CategoryTreeResponse>> GetCategoryTreeAsync();
}