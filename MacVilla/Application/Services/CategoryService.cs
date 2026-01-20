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

    public async Task<Category> CreateCategoryAsync(Category category)
    {
        return await _categoryRepository.CreateAsync(category);
    }

    public async Task<Category?> UpdateCategoryAsync(long id, Category category)
    {
        var existingCategory = await _categoryRepository.GetByIdAsync(id);
        if (existingCategory == null)
        {
            return null;
        }

        existingCategory.CategoryName = category.CategoryName;
        existingCategory.ParentCategoryId = category.ParentCategoryId;

        return await _categoryRepository.UpdateAsync(existingCategory);
    }

    public async Task<bool> DeleteCategoryAsync(long id)
    {
        return await _categoryRepository.DeleteAsync(id);
    }
}
