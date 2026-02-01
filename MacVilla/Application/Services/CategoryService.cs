using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await _categoryRepository.GetAllAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(long id)
    {
        return await _categoryRepository.GetByIdAsync(id);
    }

    public async Task<Category> CreateCategoryAsync(CreateCategoryRequest request)
    {
        var category = new Category
        {
            CategoryName = request.CategoryName,
            ParentCategoryId = request.ParentCategoryId,
            IsActive = true
        };
        return await _categoryRepository.CreateAsync(category);
    }

    public async Task<Category?> UpdateCategoryAsync(long id, UpdateCategoryRequest request)
    {
        var existingCategory = await _categoryRepository.GetByIdAsync(id);
        if (existingCategory == null)
        {
            return null;
        }

        existingCategory.CategoryName = request.CategoryName;
        existingCategory.ParentCategoryId = request.ParentCategoryId;

        return await _categoryRepository.UpdateAsync(existingCategory);
    }

    public async Task<bool> DeleteCategoryAsync(long id)
    {
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
}
