namespace Domain.Entities;

public partial class ProductAttributeValue
{
    public long ProductId { get; set; }

    public long AttributeId { get; set; }

    public string? Value { get; set; }

    public virtual ProductAttribute Attribute { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
