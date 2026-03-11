using System.ComponentModel.DataAnnotations;

public class ProductCreateRequest
{
    [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
    [StringLength(200, MinimumLength = 2)]
    public string Name { get; set; } = "";

    [Required]
    [Range(1000, 999999999)]
    public decimal Price { get; set; }

    [Required]
    public long CategoryId { get; set; }

    [StringLength(5000)]
    public string? Description { get; set; }

    [Required]
    public string Status { get; set; } = "Pending";

    public List<IFormFile>? ImageFiles { get; set; }
}