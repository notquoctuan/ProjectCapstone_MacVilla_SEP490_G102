using System;
using System.Collections.Generic;

namespace Persistence.Entities;

public partial class ShippingMethod
{
    public long ShippingMethodId { get; set; }

    public string? MethodName { get; set; }

    public decimal? BaseFee { get; set; }

    public int? EstimatedDays { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Shipping> Shippings { get; set; } = new List<Shipping>();
}
