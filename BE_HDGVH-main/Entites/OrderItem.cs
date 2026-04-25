namespace BE_API.Entities;

public class OrderItem : IEntity
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int VariantId { get; set; }
    public string? SkuSnapshot { get; set; }
    public decimal PriceSnapshot { get; set; }
    public int Quantity { get; set; }
    public decimal SubTotal { get; set; }

    public CustomerOrder Order { get; set; } = null!;
    public ProductVariant Variant { get; set; } = null!;
}
