using System;
using System.Collections.Generic;

namespace Persistence.Entities;

public partial class Rfq
{
    public long RfqId { get; set; }

    public long? UserId { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Quotation> Quotations { get; set; } = new List<Quotation>();

    public virtual User? User { get; set; }
}
