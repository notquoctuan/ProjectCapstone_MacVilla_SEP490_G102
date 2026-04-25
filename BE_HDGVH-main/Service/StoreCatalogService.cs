using System.Globalization;
using BE_API.Domain;
using BE_API.Dto.Category;
using BE_API.Dto.Common;
using BE_API.Dto.Product;
using BE_API.Dto.Store;
using BE_API.Entities;
using BE_API.Repository;
using BE_API.Service.IService;
using Microsoft.EntityFrameworkCore;

namespace BE_API.Service;

public class StoreCatalogService(
    ICategoryService categoryService,
    IRepository<Product> productRepo,
    IRepository<Category> categoryRepo,
    IRepository<ProductVariant> variantRepo) : IStoreCatalogService
{
    public Task<List<CategoryTreeNodeDto>> GetCategoryTreeAsync(CancellationToken cancellationToken = default) =>
        categoryService.GetTreeAsync(cancellationToken);

    public async Task<PagedResultDto<StoreProductListItemDto>> GetProductsPagedAsync(
        int page,
        int pageSize,
        int? categoryId,
        bool includeSubcategories,
        string? search,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 200);

        var query = productRepo.Get()
            .AsNoTracking()
            .Where(p => p.Status == ProductStatus.Active);

        if (categoryId.HasValue)
        {
            if (includeSubcategories)
            {
                var flat = await categoryRepo.Get()
                    .AsNoTracking()
                    .Select(c => new { c.Id, c.ParentId })
                    .ToListAsync(cancellationToken);
                var tuples = flat.Select(x => (x.Id, x.ParentId)).ToList();
                var ids = CategorySubtreeIds(categoryId.Value, tuples);
                query = query.Where(p => ids.Contains(p.CategoryId));
            }
            else
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            var pattern = $"%{term}%";
            query = query.Where(p =>
                EF.Functions.Like(p.Name, pattern) ||
                EF.Functions.Like(p.Slug, pattern));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new StoreProductListItemDto
            {
                Id = p.Id,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                Name = p.Name,
                Slug = p.Slug,
                ImageUrl = p.ImageUrl ?? p.Variants
                    .Where(v => v.ImageUrl != null)
                    .OrderBy(v => v.Id)
                    .Select(v => v.ImageUrl)
                    .FirstOrDefault(),
                BasePrice = p.BasePrice,
                WarrantyPeriodMonths = p.WarrantyPeriodMonths,
                VariantCount = p.Variants.Count,
                AttributeCount = p.Attributes.Count
            })
            .ToListAsync(cancellationToken);

        return new PagedResultDto<StoreProductListItemDto>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<StoreProductDetailDto> GetProductBySlugOrIdAsync(
        string slugOrId,
        CancellationToken cancellationToken = default)
    {
        var key = slugOrId?.Trim() ?? throw new ArgumentException("Thiếu slug hoặc id.");
        if (key.Length == 0)
            throw new ArgumentException("Thiếu slug hoặc id.");

        Product? entity = null;

        if (int.TryParse(key, NumberStyles.Integer, CultureInfo.InvariantCulture, out var pid))
        {
            entity = await ActiveProductDetailQuery()
                .FirstOrDefaultAsync(p => p.Id == pid, cancellationToken);
        }

        if (entity == null)
        {
            var slugLower = key.ToLowerInvariant();
            entity = await ActiveProductDetailQuery()
                .FirstOrDefaultAsync(p => p.Slug.ToLower() == slugLower, cancellationToken);
        }

        if (entity == null)
            throw new KeyNotFoundException("Không tìm thấy sản phẩm");

        return MapDetail(entity);
    }

    public async Task<StoreProductDetailDto> GetProductDetailByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await ActiveProductDetailQuery()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (entity is null)
            throw new KeyNotFoundException("Không tìm thấy sản phẩm");

        return MapDetail(entity);
    }

    public async Task<StoreVariantSkuDto?> GetVariantBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        var key = sku.Trim();
        if (string.IsNullOrEmpty(key))
            return null;

        var row = await variantRepo.Get()
            .AsNoTracking()
            .Include(v => v.Product)
            .Include(v => v.Inventories)
            .FirstOrDefaultAsync(
                v => v.Sku.ToLower() == key.ToLower() && v.Product.Status == ProductStatus.Active,
                cancellationToken);

        if (row is null)
            return null;

        var inv = row.Inventories.FirstOrDefault();
        return new StoreVariantSkuDto
        {
            Id = row.Id,
            ProductId = row.ProductId,
            ProductName = row.Product.Name,
            ProductSlug = row.Product.Slug,
            Sku = row.Sku,
            VariantName = row.VariantName,
            RetailPrice = row.RetailPrice,
            Weight = row.Weight,
            Dimensions = row.Dimensions,
            ImageUrl = row.ImageUrl,
            QuantityOnHand = inv?.QuantityOnHand,
            QuantityReserved = inv?.QuantityReserved,
            QuantityAvailable = inv?.QuantityAvailable
        };
    }

    private IQueryable<Product> ActiveProductDetailQuery() =>
        productRepo.Get()
            .AsNoTracking()
            .AsSplitQuery()
            .Where(p => p.Status == ProductStatus.Active)
            .Include(p => p.Category)
            .Include(p => p.Attributes).ThenInclude(a => a.Values)
            .Include(p => p.Variants).ThenInclude(v => v.Inventories);

    private static StoreProductDetailDto MapDetail(Product entity) =>
        new()
        {
            Id = entity.Id,
            CategoryId = entity.CategoryId,
            CategoryName = entity.Category.Name,
            Name = entity.Name,
            Slug = entity.Slug,
            Description = entity.Description,
            ImageUrl = entity.ImageUrl ?? entity.Variants
                .Where(v => !string.IsNullOrWhiteSpace(v.ImageUrl))
                .OrderBy(v => v.Id)
                .Select(v => v.ImageUrl)
                .FirstOrDefault(),
            BasePrice = entity.BasePrice,
            WarrantyPeriodMonths = entity.WarrantyPeriodMonths,
            VariantCount = entity.Variants.Count,
            AttributeCount = entity.Attributes.Count,
            Attributes = entity.Attributes
                .OrderBy(a => a.Name)
                .Select(a => new ProductDetailAttributeDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Values = a.Values
                        .OrderBy(v => v.Value)
                        .Select(v => new ProductDetailAttributeValueDto { Id = v.Id, Value = v.Value })
                        .ToList()
                })
                .ToList(),
            Variants = entity.Variants
                .OrderBy(v => v.Sku)
                .Select(v =>
                {
                    var row = v.Inventories.FirstOrDefault();
                    return new StoreProductVariantDto
                    {
                        Id = v.Id,
                        Sku = v.Sku,
                        VariantName = v.VariantName,
                        RetailPrice = v.RetailPrice,
                        Weight = v.Weight,
                        Dimensions = v.Dimensions,
                        ImageUrl = v.ImageUrl,
                        QuantityOnHand = row?.QuantityOnHand,
                        QuantityReserved = row?.QuantityReserved,
                        QuantityAvailable = row?.QuantityAvailable
                    };
                })
                .ToList()
        };

    private static int[] CategorySubtreeIds(int rootId, IReadOnlyList<(int Id, int? ParentId)> flat)
    {
        var result = new List<int>();
        var seen = new HashSet<int>();
        var queue = new Queue<int>();
        queue.Enqueue(rootId);
        while (queue.Count > 0)
        {
            var id = queue.Dequeue();
            if (!seen.Add(id))
                continue;
            result.Add(id);
            foreach (var (cid, pid) in flat)
            {
                if (pid == id)
                    queue.Enqueue(cid);
            }
        }

        return result.ToArray();
    }
}
