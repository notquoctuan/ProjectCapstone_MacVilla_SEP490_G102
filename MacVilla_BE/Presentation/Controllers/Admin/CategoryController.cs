using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    /// <summary>
    /// Get all categories with search, filter, and pagination
    /// </summary>
    /// <remarks>GET: api/category?name=electronics&amp;isActive=true&amp;pageNumber=1&amp;pageSize=10</remarks>
    [HttpGet]
    public async Task<ActionResult<PagedResponse<Category>>> GetAll([FromQuery] CategorySearchRequest request)
    {
        var result = await _categoryService.SearchCategoriesAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    /// <remarks>GET: api/category/{id}</remarks>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<Category>> GetById([FromRoute] long id)
    {
        if (id <= 0)
            return BadRequest(new { message = "ID danh mục phải lớn hơn 0." });

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
    public async Task<IActionResult> Update([FromRoute] long id, [FromBody] UpdateCategoryRequest request)
    {
        if (id <= 0)
            return BadRequest(new { message = "ID danh mục phải lớn hơn 0." });

        if (request.ParentCategoryId.HasValue && request.ParentCategoryId.Value == id)
            return BadRequest(new { message = "Danh mục không thể trở thành danh mục cha của chính nó." });

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
    public async Task<IActionResult> Delete([FromRoute] long id)
    {
        if (id <= 0)
            return BadRequest(new { message = "ID danh mục phải lớn hơn 0." });

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
    public async Task<IActionResult> Activate([FromRoute] long id)
    {
        if (id <= 0)
            return BadRequest(new { message = "ID danh mục phải lớn hơn 0." });

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
    public async Task<IActionResult> Deactivate([FromRoute] long id)
    {
        if (id <= 0)
            return BadRequest(new { message = "ID danh mục phải lớn hơn 0." });

        var success = await _categoryService.DeactivateCategoryAsync(id);
        if (!success)
        {
            return NotFound(new { message = "Category not found." });
        }

        return Ok(new { message = "Category deactivated successfully." });
    }
}
