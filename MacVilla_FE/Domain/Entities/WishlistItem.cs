using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class WishlistItem
{
    public long WishlistItemId { get; set; }

    public long? WishlistId { get; set; }

    public long? ProductId { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Wishlist? Wishlist { get; set; }
}
