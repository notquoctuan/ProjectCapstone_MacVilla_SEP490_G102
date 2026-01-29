using System;
using System.Collections.Generic;

namespace Persistence.Entities;

public partial class InventoryHistory
{
    public long HistoryId { get; set; }

    public long? InventoryId { get; set; }

    public int? ChangeQty { get; set; }

    public string? Reason { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Inventory? Inventory { get; set; }
}
