using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Repositories;

namespace Presentation.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
//[Authorize(Roles = "Admin")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("getall")]
    public async Task<IActionResult> GetAll()
    {
        var categories = (await _categoryService.GetAllCategoriesAsync()).ToList();

        var result = new List<object>();

        void BuildHierarchy(long? parentId, int level)
        {
            var children = categories
                .Where(c => c.ParentCategoryId == parentId)
                .OrderBy(c => c.CategoryName);

            foreach (var item in children)
            {
                string prefix = new string('-', level * 2);

                result.Add(new
                {
                    CategoryId = item.CategoryId,
                    Name = level > 0 ? $"{prefix} {item.CategoryName}" : item.CategoryName
                });

                BuildHierarchy(item.CategoryId, level + 1);
            }
        }

        BuildHierarchy(null, 0);

        return Ok(result);
    }

    /// <summary>
    /// Get all categories with search, filter, and pagination
    /// </summary>
    /// <remarks>GET: api/category?name=electronics&amp;isActive=true&amp;pageNumber=1&amp;pageSize=10</remarks>
    [HttpGet]
    public async Task<ActionResult<PagedResponse<Category>>> GetAll(
        [FromQuery] string? name,
        [FromQuery] bool? isActive,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var request = new CategorySearchRequest
        {
            Name = name,
            IsActive = isActive,
            PageNumber = pageNumber > 0 ? pageNumber : 1,
            PageSize = pageSize > 0 && pageSize <= 100 ? pageSize : 10
        };

        var result = await _categoryService.SearchCategoriesAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    /// <remarks>GET: api/category/{id}</remarks>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<Category>> GetById(long id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    /// <summary>
    /// Add new category
    /// </summary>
    /// <remarks>POST: api/category</remarks>
    [HttpPost]
    public async Task<ActionResult<Category>> Create([FromBody] CreateCategoryRequest request)
    {
        var createdCategory = await _categoryService.CreateCategoryAsync(request.CategoryName, request.ParentCategoryId);
        return CreatedAtAction(nameof(GetById), new { id = createdCategory.CategoryId }, createdCategory);
    }

    /// <summary>
    /// Update category
    /// </summary>
    /// <remarks>PUT: api/category/{id}</remarks>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateCategoryRequest request)
    {
        var category = await _categoryService.UpdateCategoryAsync(id, request.CategoryName, request.ParentCategoryId);
        if (category == null)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Soft delete category
    /// </summary>
    /// <remarks>DELETE: api/category/{id}</remarks>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var deleted = await _categoryService.DeleteCategoryAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Activate category
    /// </summary>
    /// <remarks>PATCH: api/category/{id}/activate</remarks>
    [HttpPatch("{id:long}/activate")]
    public async Task<IActionResult> Activate(long id)
    {
        var success = await _categoryService.ActivateCategoryAsync(id);
        if (!success)
        {
            return NotFound(new { message = "Category not found." });
        }

        return Ok(new { message = "Category activated successfully." });
    }

    /// <summary>
    /// Deactivate category
    /// </summary>
    /// <remarks>PATCH: api/category/{id}/deactivate</remarks>
    [HttpPatch("{id:long}/deactivate")]
    public async Task<IActionResult> Deactivate(long id)
    {
        var success = await _categoryService.DeactivateCategoryAsync(id);
        if (!success)
        {
            return NotFound(new { message = "Category not found." });
        }

        return Ok(new { message = "Category deactivated successfully." });
    }
}
