public class OrderSearchRequest
{
    public string? Status { get; set; }
    public long? UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 15;
}

public class OrderListResponse
{
    public long OrderId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? Status { get; set; }
    public decimal? TotalAmount { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public int ItemCount { get; set; }
    public string? PaymentStatus { get; set; }
    public string? ShippingStatus { get; set; }
}

public class OrderDetailResponse
{
    public long OrderId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? Status { get; set; }
    public decimal? TotalAmount { get; set; }
    public CustomerInfoDto? Customer { get; set; }
    public List<OrderItemDetailDto> OrderItems { get; set; } = new();
    public List<PaymentInfoDto> Payments { get; set; } = new();
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
    public decimal? SubTotal { get; set; }
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

public class UpdateOrderStatusRequest
{
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
