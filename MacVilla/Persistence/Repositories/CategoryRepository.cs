using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly DatabaseConfig _dbContext;

    public CategoryRepository(DatabaseConfig dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _dbContext.Categories
            .Where(c => c.DeletedAt == null)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(long id)
    {
        return await _dbContext.Categories
            .FirstOrDefaultAsync(c => c.CategoryId == id && c.DeletedAt == null);
    }

    public async Task<Category> CreateAsync(Category category)
    {
        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateAsync(Category category)
    {
        _dbContext.Categories.Update(category);
        await _dbContext.SaveChangesAsync();
        return category;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var category = await _dbContext.Categories.FindAsync(id);
        if (category == null)
        {
            return false;
        }

        category.DeletedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _dbContext.Categories
            .AnyAsync(c => c.CategoryId == id && c.DeletedAt == null);
    }
    
    public async Task<bool> IsNameUniqueAsync(string categoryName, long? excludeId = null)
    {
        return !await _dbContext.Categories
            .AnyAsync(c => c.CategoryName == categoryName 
                           && c.DeletedAt == null 
                           && (excludeId == null || c.CategoryId != excludeId));
    }

    public async Task<IEnumerable<Category>> GetByParentIdAsync(long parentId)
    {
        return await _dbContext.Categories
            .Where(c => c.ParentCategoryId == parentId && c.DeletedAt == null)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _dbContext.Categories
            .CountAsync(c => c.DeletedAt == null);
    }

    public async Task<bool> SetIsActiveAsync(long id, bool isActive)
    {
        var category = await _dbContext.Categories
            .FirstOrDefaultAsync(c => c.CategoryId == id && c.DeletedAt == null);
        if (category == null)
        {
            return false;
        }

        category.IsActive = isActive;
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
