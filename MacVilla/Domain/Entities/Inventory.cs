namespace Domain.Entities;

public partial class Inventory
{
    public long InventoryId { get; set; }

    public long? ProductId { get; set; }

    public long? WarehouseId { get; set; }

    public int? Quantity { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Warehouse? Warehouse { get; set; }
}
