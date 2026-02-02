using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(long id);
    Task<Category> CreateCategoryAsync(string categoryName, long? parentCategoryId);
    Task<Category?> UpdateCategoryAsync(long id, string categoryName, long? parentCategoryId);
    Task<bool> DeleteCategoryAsync(long id);
    Task<bool> ActivateCategoryAsync(long id);
    Task<bool> DeactivateCategoryAsync(long id);
    Task<PagedResponse<Category>> SearchCategoriesAsync(CategorySearchRequest request);
}
