namespace Application.DTOs;

public class OrderTrackingResponse
{
    public long OrderId { get; set; }
    public string? CurrentStatus { get; set; }
    public DateTime? CreatedAt { get; set; }
    public List<OrderStatusHistoryDto> StatusHistory { get; set; } = new();
    public OrderTimelineDto Timeline { get; set; } = new();
}

public class OrderStatusHistoryDto
{
    public string? Status { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? Notes { get; set; }
}

public class OrderTimelineDto
{
    public bool IsOrderPlaced { get; set; }
    public DateTime? OrderPlacedAt { get; set; }

    public bool IsPaymentReceived { get; set; }
    public DateTime? PaymentReceivedAt { get; set; }

    public bool IsProcessing { get; set; }
    public DateTime? ProcessingStartedAt { get; set; }

    public bool IsShipped { get; set; }
    public DateTime? ShippedAt { get; set; }

    public bool IsDelivered { get; set; }
    public DateTime? DeliveredAt { get; set; }

    public bool IsCancelled { get; set; }
    public DateTime? CancelledAt { get; set; }
}