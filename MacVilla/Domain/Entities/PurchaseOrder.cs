namespace Domain.Entities;

public partial class PurchaseOrder
{
    public long PoId { get; set; }

    public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
}
