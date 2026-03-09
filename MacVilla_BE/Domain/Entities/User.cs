using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class User
{
    public long UserId { get; set; }

    public string Email { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Avatar { get; set; }

    public string? Phone { get; set; }

    public string? Role { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Rfq> Rfqs { get; set; } = new List<Rfq>();

    public virtual ICollection<ShippingAddress> ShippingAddresses { get; set; } = new List<ShippingAddress>();

    public virtual ICollection<UserCredential> UserCredentials { get; set; } = new List<UserCredential>();

    public virtual ICollection<UserOauth> UserOauths { get; set; } = new List<UserOauth>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
}
