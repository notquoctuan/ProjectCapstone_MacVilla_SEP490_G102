        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] string? name, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] int? categoryId)
        {
            var result = await _productService.SearchProductsForAdmin(name, minPrice, maxPrice, categoryId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _productService.GetProductDetailsAsync(id);

            if (result == null)
            {
                return NotFound(new { message = $"Không tìm thấy sản phẩm có ID: {id}" });
            }

            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateProductRequest request)
