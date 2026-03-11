using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/products")]
    //[Authorize(Roles = "Admin,Employee")]
    public class ProductAdminController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _env;

        public ProductAdminController(IProductService productService, IWebHostEnvironment env)
        {
            _productService = productService;
            _env = env;
        }

        /// <summary>Lấy danh sách sản phẩm (lọc + phân trang)</summary>
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] ProductSearchRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(FormatErrors());
            try
            {
                var result = await _productService.GetPagedProductsAsync(request);
                return Ok(result);
            }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        /// <summary>Lấy chi tiết sản phẩm kèm toàn bộ ảnh</summary>
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetDetail(long id)
        {
            try
            {
                var product = await _productService.GetProductDetailAsync(id);
                if (product == null) return NotFound(new { message = "Không tìm thấy sản phẩm." });
                return Ok(product);
            }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        /// <summary>Tạo sản phẩm mới</summary>
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(FormatErrors());
            try
            {
                var result = await _productService.CreateProductAsync(request, _env.WebRootPath);
                return CreatedAtAction(nameof(GetDetail), new { id = result.ProductId }, result);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        /// <summary>
        /// Cập nhật sản phẩm.
        /// - NewImageFiles   : upload thêm ảnh mới (không xóa ảnh cũ)
        /// - DeleteImageIds  : danh sách ImageId muốn xóa
        /// - MainImageId     : ImageId muốn đặt làm ảnh chính
        /// </summary>
        [HttpPut("{id:long}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(long id, [FromForm] ProductUpdateRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(FormatErrors());
            try
            {
                await _productService.UpdateProductAsync(id, request, _env.WebRootPath);
                return Ok(new { message = "Cập nhật sản phẩm thành công." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        // ════════════════════════════════════════════════════════════════
        // QUẢN LÝ ẢNH RIÊNG LẺ
        // ════════════════════════════════════════════════════════════════

        /// <summary>Upload thêm ảnh cho sản phẩm (không xóa ảnh cũ)</summary>
        [HttpPost("{id:long}/images")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddImages(long id, [FromForm] List<IFormFile> files)
        {
            if (files == null || !files.Any())
                return BadRequest(new { message = "Vui lòng chọn ít nhất 1 ảnh." });
            try
            {
                var result = await _productService.AddImagesAsync(id, files, _env.WebRootPath);
                return Ok(new { message = $"Đã upload {result.Count} ảnh thành công.", images = result });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        /// <summary>Xóa 1 ảnh của sản phẩm theo ImageId</summary>
        [HttpDelete("{id:long}/images/{imageId:long}")]
        public async Task<IActionResult> DeleteImage(long id, long imageId)
        {
            try
            {
                await _productService.DeleteImageAsync(id, imageId, _env.WebRootPath);
                return Ok(new { message = "Xóa ảnh thành công." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        /// <summary>Đặt ảnh chính cho sản phẩm</summary>
        [HttpPatch("{id:long}/images/{imageId:long}/set-main")]
        public async Task<IActionResult> SetMainImage(long id, long imageId)
        {
            try
            {
                await _productService.SetMainImageAsync(id, imageId);
                return Ok(new { message = "Đã đặt ảnh chính thành công." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        }

        /// <summary>Cập nhật trạng thái sản phẩm</summary>
        [HttpPatch("{id:long}/status")]
        public async Task<IActionResult> ChangeStatus(long id, [FromBody] ChangeStatusRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(FormatErrors());
            try
            {
                await _productService.ChangeStatusAsync(id, request.Status);
                return Ok(new { message = $"Đã chuyển trạng thái sang '{request.Status}'." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        /// <summary>Xóa sản phẩm (Admin only)</summary>
        [HttpDelete("{id:long}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                await _productService.DeleteProductAsync(id, _env.WebRootPath);
                return Ok(new { message = "Xóa sản phẩm thành công." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        // ── Helper ──────────────────────────────────────────────────────
        private object FormatErrors() => new
        {
            message = "Dữ liệu không hợp lệ.",
            errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value!.Errors.Select(e => e.ErrorMessage).ToList()
                )
        };
    }

    public class ChangeStatusRequest
    {
        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        [RegularExpression("^(Enable|Disable|Pending)$",
            ErrorMessage = "Trạng thái chỉ được là: Enable, Disable hoặc Pending.")]
        public string Status { get; set; } = null!;
    }
}