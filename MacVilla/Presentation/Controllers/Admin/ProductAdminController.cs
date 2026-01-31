        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] string? name, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] int? categoryId)
        {
            var result = await _productService.SearchProductsForAdmin(name, minPrice, maxPrice, categoryId);
            return Ok(result);
        }

