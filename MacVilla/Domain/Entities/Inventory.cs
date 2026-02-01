using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Inventory
{
    public long InventoryId { get; set; }

    public long? ProductId { get; set; }

    public string? Sku { get; set; }

    public int? Quantity { get; set; }

    public string? WarehouseLocation { get; set; }

    public virtual ICollection<InventoryHistory> InventoryHistories { get; set; } = new List<InventoryHistory>();

    public virtual Product? Product { get; set; }
}
