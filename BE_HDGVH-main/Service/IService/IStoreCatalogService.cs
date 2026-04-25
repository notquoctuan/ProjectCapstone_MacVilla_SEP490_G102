using BE_API.Dto.Category;
using BE_API.Dto.Common;
using BE_API.Dto.Store;

namespace BE_API.Service.IService;

public interface IStoreCatalogService
{
    Task<List<CategoryTreeNodeDto>> GetCategoryTreeAsync(CancellationToken cancellationToken = default);

    Task<PagedResultDto<StoreProductListItemDto>> GetProductsPagedAsync(
        int page,
        int pageSize,
        int? categoryId,
        bool includeSubcategories,
        string? search,
        CancellationToken cancellationToken = default);

    Task<StoreProductDetailDto> GetProductBySlugOrIdAsync(string slugOrId, CancellationToken cancellationToken = default);

    /// <summary>Chi tiết theo id số — chỉ sản phẩm <c>Active</c> (cùng payload với chi tiết theo slug).</summary>
    Task<StoreProductDetailDto> GetProductDetailByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<StoreVariantSkuDto?> GetVariantBySkuAsync(string sku, CancellationToken cancellationToken = default);
}
