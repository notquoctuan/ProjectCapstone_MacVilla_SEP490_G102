using System;
using System.Collections.Generic;

namespace Persistence.Entities;

public partial class Shipping
{
    public long ShippingId { get; set; }

    public long? OrderId { get; set; }

    public long? ShippingAddressId { get; set; }

    public long? ShippingMethodId { get; set; }

    public decimal? ShippingFee { get; set; }

    public string? Status { get; set; }

    public DateTime? DeliveredDate { get; set; }

    public virtual Order? Order { get; set; }

    public virtual ShippingAddress? ShippingAddress { get; set; }

    public virtual ShippingMethod? ShippingMethod { get; set; }
}
