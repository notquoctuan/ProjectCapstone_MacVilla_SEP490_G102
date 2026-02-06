using System;
using System.Collections.Generic;

namespace Application.DTOs
{
    public class ProductHomeResponse
    {
        public long ProductId { get; set; }
        public string? Name { get; set; }
        public string? CategoryName { get; set; }
        public decimal Price { get; set; }
        public string? Status { get; set; }
        public string? MainImageUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CategoryMenuDto
    {
        public long CategoryId { get; set;}
        public string? CategoryName { get; set; }
        public List<CategoryMenuDto> Children { get; set; } = new();
    }

    public class HomepageResponse
    {
        public IEnumerable<CategoryMenuDto> Categories { get; set; }
        public IEnumerable<ProductHomeResponse> FeaturedProducts { get; set; }
    }
}