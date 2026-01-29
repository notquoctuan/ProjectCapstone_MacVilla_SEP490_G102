using System;
using System.Collections.Generic;

namespace Persistence.Entities;

public partial class Payment
{
    public long PaymentId { get; set; }

    public long? OrderId { get; set; }

    public string? PaymentMethod { get; set; }

    public string? PaymentStatus { get; set; }

    public DateTime? PaidAt { get; set; }

    public virtual Order? Order { get; set; }
}
