using Domain.Enums;

namespace Domain.Entities;

public class Order
{
    public long OrderId { get; set; }
    public long? UserId { get; set; }   // DB: DEFAULT NULL
    public long? ShippingAddressId { get; set; }
    public long? ShippingMethodId { get; set; }
    public decimal? TotalAmount { get; set; }   // DB: DEFAULT NULL
    public string? Status { get; set; }
    public string? Note { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual User? User { get; set; }
    public virtual ShippingAddress? ShippingAddress { get; set; }
    public virtual ShippingMethod? ShippingMethod { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public virtual ICollection<Shipping> Shippings { get; set; } = new List<Shipping>();
}