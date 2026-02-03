namespace Domain.Entities;

public partial class ProductAttribute
{
    public long AttributeId { get; set; }

    public virtual ICollection<ProductAttributeValue> ProductAttributeValues { get; set; } = new List<ProductAttributeValue>();
}
