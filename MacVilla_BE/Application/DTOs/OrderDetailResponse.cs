namespace Application.DTOs;

public class OrderDetailResponse
{
    public long OrderId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? Status { get; set; }
    public decimal? TotalAmount { get; set; }

    // Customer Information
    public CustomerInfoDto? Customer { get; set; }

    // Order Items
    public List<OrderItemDetailDto> OrderItems { get; set; } = new();

    // Payment Information
    public List<PaymentInfoDto> Payments { get; set; } = new();

    // Shipping Information
    public List<ShippingInfoDto> Shippings { get; set; } = new();
}

public class CustomerInfoDto
{
    public long UserId { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}

public class OrderItemDetailDto
{
    public long OrderItemId { get; set; }
    public long ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductImageUrl { get; set; }
    public string? CategoryName { get; set; }
    public int? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? SubTotal => Quantity.HasValue && UnitPrice.HasValue
        ? Quantity.Value * UnitPrice.Value
        : 0;
}

public class PaymentInfoDto
{
    public long PaymentId { get; set; }
    public string? PaymentMethod { get; set; }
    public string? PaymentStatus { get; set; }
    public DateTime? PaidAt { get; set; }
}

public class ShippingInfoDto
{
    public long ShippingId { get; set; }
    public string? Status { get; set; }
    public decimal? ShippingFee { get; set; }
    public DateTime? DeliveredDate { get; set; }
    public ShippingAddressDto? ShippingAddress { get; set; }
    public ShippingMethodDto? ShippingMethod { get; set; }
}

public class ShippingAddressDto
{
    public string? ReceiverName { get; set; }
    public string? Phone { get; set; }
    public string? AddressLine { get; set; }
    public string? Ward { get; set; }
    public string? District { get; set; }
    public string? City { get; set; }
}

public class ShippingMethodDto
{
    public string? MethodName { get; set; }
    public decimal? BaseFee { get; set; }
    public int? EstimatedDays { get; set; }
}