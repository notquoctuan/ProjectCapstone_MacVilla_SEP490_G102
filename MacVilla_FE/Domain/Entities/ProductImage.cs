using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class ProductImage
{
    public long ImageId { get; set; }

    public long? ProductId { get; set; }

    public string? ImageUrl { get; set; }

    public bool? IsMain { get; set; }

    public virtual Product? Product { get; set; }
}
