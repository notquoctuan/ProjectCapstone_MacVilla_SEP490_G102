using System;
using System.Collections.Generic;
namespace Domain.Entities;

public partial class Quotation
{
    public long QuotationId { get; set; }

    public long? RfqId { get; set; }

    public decimal? Price { get; set; }

    public DateOnly? ValidUntil { get; set; }

    public string? Notes { get; set; }

    public virtual Rfq? Rfq { get; set; }
}
