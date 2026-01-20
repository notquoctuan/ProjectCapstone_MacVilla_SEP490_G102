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

    // GET: api/category
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetAll()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    // GET: api/category/{id}
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

    // POST: api/category
    [HttpPost]
    public async Task<ActionResult<Category>> Create(Category category)
    {
        var createdCategory = await _categoryService.CreateCategoryAsync(category);
        return CreatedAtAction(nameof(GetById), new { id = createdCategory.CategoryId }, createdCategory);
    }

    // PUT: api/category/{id}
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, Category updatedCategory)
    {
        var category = await _categoryService.UpdateCategoryAsync(id, updatedCategory);
        if (category == null)
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE: api/category/{id}
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
}
