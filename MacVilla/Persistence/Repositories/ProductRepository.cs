        public async Task<IEnumerable<Product>> GetProductsForAdminAsync(string? name, decimal? minPrice, decimal? maxPrice, int? categoryId)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(p => p.Name.Contains(name));
            if (minPrice.HasValue) query = query.Where(p => p.Price >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(p => p.Price <= maxPrice.Value);
            if (categoryId.HasValue) query = query.Where(p => p.CategoryId == categoryId.Value);
