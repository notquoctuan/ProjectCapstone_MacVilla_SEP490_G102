using System;
using System.Collections.Generic;

namespace Persistence.Entities;

public partial class ShippingAddress
{
    public long ShippingAddressId { get; set; }

    public long UserId { get; set; }

    public string? ReceiverName { get; set; }

    public string? Phone { get; set; }

    public string? AddressLine { get; set; }

    public string? Ward { get; set; }

    public string? District { get; set; }

    public string? City { get; set; }

    public bool? IsDefault { get; set; }

    public virtual ICollection<Shipping> Shippings { get; set; } = new List<Shipping>();

    public virtual User User { get; set; } = null!;
}
