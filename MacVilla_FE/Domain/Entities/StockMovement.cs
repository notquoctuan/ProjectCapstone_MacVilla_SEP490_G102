namespace Domain.Entities;

public partial class StockMovement
{
    public long MovementId { get; set; }

    public long? ProductId { get; set; }

    public int? Quantity { get; set; }

    public string? MovementType { get; set; }

    public long? ReferenceId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Product? Product { get; set; }
}
