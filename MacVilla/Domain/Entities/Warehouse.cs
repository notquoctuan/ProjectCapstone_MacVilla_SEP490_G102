namespace Domain.Entities;

public partial class Warehouse
{
    public long WarehouseId { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
}
