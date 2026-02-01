using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class ProductSpecification
{
    public long SpecId { get; set; }

    public long? ProductId { get; set; }

    public string? SpecName { get; set; }

    public string? SpecValue { get; set; }

    public virtual Product? Product { get; set; }
}
