using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    public async Task<IEnumerable<Category>> GetCategoriesOrderedAsync()
    {
        var allCategories = await _categoryRepository.GetAllAsync();
        var result = new List<Category>();

        var rootCategories = allCategories.Where(c => c.ParentCategoryId == null).OrderBy(c => c.CategoryName);

        foreach (var parent in rootCategories)
        {
            result.Add(parent);
            AddChildren(allCategories, parent.CategoryId, result);
        }

        return result;
    }
    private void AddChildren(IEnumerable<Category> all, long parentId, List<Category> result)
    {
        var children = all.Where(c => c.ParentCategoryId == parentId).OrderBy(c => c.CategoryName);
        foreach (var child in children)
        {
            result.Add(child);
            AddChildren(all, child.CategoryId, result);
        }
    }
    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await _categoryRepository.GetAllAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(long id)
    {
        return await _categoryRepository.GetByIdAsync(id);
    }

    public async Task<Category> CreateCategoryAsync(string categoryName, long? parentCategoryId)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            throw new ArgumentException("Tên danh mục không được để trống.", nameof(categoryName));
        }

        var normalizedName = categoryName.Trim();

        // Ngăn tạo trùng tên danh mục
        if (await _categoryRepository.ExistsByNameAsync(normalizedName))
        {
            throw new InvalidOperationException("Tên danh mục đã tồn tại. Vui lòng chọn tên khác.");
        }

        var category = new Category
        {
            CategoryName = normalizedName,
            ParentCategoryId = parentCategoryId,
            IsActive = true
        };
        return await _categoryRepository.CreateAsync(category);
    }

    public async Task<Category?> UpdateCategoryAsync(long id, string categoryName, long? parentCategoryId)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            throw new ArgumentException("Tên danh mục không được để trống.", nameof(categoryName));
        }

        var normalizedName = categoryName.Trim();

        var existingCategory = await _categoryRepository.GetByIdAsync(id);
        if (existingCategory == null)
        {
            return null;
        }

        // Ngăn trùng tên với danh mục khác
        if (await _categoryRepository.ExistsByNameAsync(normalizedName, id))
        {
            throw new InvalidOperationException("Tên danh mục đã tồn tại. Vui lòng chọn tên khác.");
        }

        existingCategory.CategoryName = normalizedName;
        existingCategory.ParentCategoryId = parentCategoryId;

        return await _categoryRepository.UpdateAsync(existingCategory);
    }

    public async Task<bool> DeleteCategoryAsync(long id)
    {
        // Nếu danh mục đang có sản phẩm thì không cho phép xóa
        if (await _categoryRepository.HasProductsAsync(id))
        {
            throw new InvalidOperationException("Danh mục đang được gắn với sản phẩm, không thể xóa.");
        }

        return await _categoryRepository.DeleteAsync(id);
    }

    public async Task<bool> ActivateCategoryAsync(long id)
    {
        return await _categoryRepository.SetIsActiveAsync(id, true);
    }

    public async Task<bool> DeactivateCategoryAsync(long id)
    {
        return await _categoryRepository.SetIsActiveAsync(id, false);
    }

    public async Task<PagedResponse<Category>> SearchCategoriesAsync(CategorySearchRequest request)
    {
        var (categories, totalCount) = await _categoryRepository.SearchAsync(
            request.Name,
            request.IsActive,
            request.PageNumber,
            request.PageSize
        );

        return new PagedResponse<Category>
        {
            Data = categories.ToList(),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }
}