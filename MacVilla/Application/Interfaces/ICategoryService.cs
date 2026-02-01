using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(long id);
    Task<Category> CreateCategoryAsync(CreateCategoryRequest request);
    Task<Category?> UpdateCategoryAsync(long id, UpdateCategoryRequest request);
    Task<bool> DeleteCategoryAsync(long id);
    Task<bool> ActivateCategoryAsync(long id);
    Task<bool> DeactivateCategoryAsync(long id);
}
