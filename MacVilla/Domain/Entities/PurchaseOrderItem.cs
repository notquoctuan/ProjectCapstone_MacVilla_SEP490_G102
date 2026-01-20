namespace Domain.Entities;

public partial class PurchaseOrderItem
{
    public long PoItemId { get; set; }

    public long? PoId { get; set; }

    public long? ProductId { get; set; }

    public int? Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public virtual PurchaseOrder? Po { get; set; }

    public virtual Product? Product { get; set; }
}
