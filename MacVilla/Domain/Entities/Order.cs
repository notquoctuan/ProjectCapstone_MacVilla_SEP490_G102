namespace Domain.Entities;

public partial class Order
{
    public long OrderId { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
