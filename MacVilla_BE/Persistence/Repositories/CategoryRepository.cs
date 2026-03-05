using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly MacvilladbContext _dbContext;

    public CategoryRepository(MacvilladbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(long id)
    {
        return await _dbContext.Categories
            .FirstOrDefaultAsync(c => c.CategoryId == id);
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

        // Soft delete by setting IsActive to false
        category.IsActive = false;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetIsActiveAsync(long id, bool isActive)
    {
        var category = await _dbContext.Categories.FindAsync(id);
        if (category == null)
        {
            return false;
        }

        category.IsActive = isActive;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<(IEnumerable<Category> Categories, int TotalCount)> SearchAsync(string? name, bool? isActive, int pageNumber, int pageSize)
    {
        var query = _dbContext.Categories.AsQueryable();

        // Filter by name (case-insensitive partial match)
        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(c => c.CategoryName != null && c.CategoryName.Contains(name));
        }

        // Filter by status
        if (isActive.HasValue)
        {
            query = query.Where(c => c.IsActive == isActive.Value);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply pagination
        var categories = await query
            .OrderBy(c => c.CategoryName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return (categories, totalCount);
    }

    public async Task<bool> ExistsByNameAsync(string categoryName, long? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            return false;
        }

        var normalized = categoryName.Trim().ToLower();

        return await _dbContext.Categories
            .AnyAsync(c =>
                c.CategoryName != null &&
                c.CategoryName.ToLower() == normalized &&
                (!excludeId.HasValue || c.CategoryId != excludeId.Value));
    }

    public async Task<bool> HasProductsAsync(long categoryId)
    {
        return await _dbContext.Products.AnyAsync(p => p.CategoryId == categoryId);
    }
}
