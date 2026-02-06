using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly ICategoryService _categoryService;

        public HomeController(ProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        /// </new>
        [HttpGet]
        public async Task<ActionResult<HomepageResponse>> GetHomepage()
        {
            var featuredProducts = await _productService.GetHomepageProductsAsync(8);

            var allCategories = (await _categoryService.GetAllCategoriesAsync())
                .Where(c => c.IsActive == true)
                .ToList();

            var categoryMenu = allCategories
                .Where(c => c.ParentCategoryId == null)
                .Select(parent => new CategoryMenuDto
                {
                    CategoryId = parent.CategoryId,
                    CategoryName = parent.CategoryName,
                    Children = allCategories
                        .Where(child => child.ParentCategoryId == parent.CategoryId)
                        .Select(child => new CategoryMenuDto
                        {
                            CategoryId = child.CategoryId,
                            CategoryName = child.CategoryName
                        }).ToList()
                }).ToList();

            return Ok(new HomepageResponse
            {
                Categories = categoryMenu,
                FeaturedProducts = featuredProducts
            });
        }
    }
}