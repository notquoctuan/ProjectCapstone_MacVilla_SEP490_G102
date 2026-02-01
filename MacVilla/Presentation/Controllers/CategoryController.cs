using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Get all categories (view category list)
    /// </summary>
    /// <remarks>GET: api/category</remarks>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetAll()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
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
        var createdCategory = await _categoryService.CreateCategoryAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = createdCategory.CategoryId }, createdCategory);
    }

    /// <summary>
    /// Update category
    /// </summary>
    /// <remarks>PUT: api/category/{id}</remarks>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateCategoryRequest request)
    {
        var category = await _categoryService.UpdateCategoryAsync(id, request);
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
