using System;
using System.Collections.Generic;

namespace Persistence.Entities;

public partial class Wishlist
{
    public long WishlistId { get; set; }

    public long? UserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; }

    public virtual ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
}
