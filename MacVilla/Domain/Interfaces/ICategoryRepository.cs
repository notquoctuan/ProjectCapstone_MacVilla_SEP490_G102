using Domain.Entities;

namespace Domain.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(long id);
    Task<Category> CreateAsync(Category category);
    Task<Category> UpdateAsync(Category category);
    Task<bool> DeleteAsync(long id);
    Task<bool> SetIsActiveAsync(long id, bool isActive);
    Task<(IEnumerable<Category> Categories, int TotalCount)> SearchAsync(string? name, bool? isActive, int pageNumber, int pageSize);
}
