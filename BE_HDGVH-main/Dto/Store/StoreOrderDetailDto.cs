namespace BE_API.Dto.Store;

public class StoreOrderDetailDto
{
    public string OrderCode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string OrderStatus { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string? PaymentMethod { get; set; }
    public string? VoucherCode { get; set; }
    public StoreOrderShippingAddressDto? ShippingAddress { get; set; }
    public List<StoreOrderDetailLineDto> Lines { get; set; } = [];
    public decimal MerchandiseSubtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal PayableTotal { get; set; }
}

public class StoreOrderShippingAddressDto
{
    public int Id { get; set; }
    public string ReceiverName { get; set; } = string.Empty;
    public string ReceiverPhone { get; set; } = string.Empty;
    public string AddressLine { get; set; } = string.Empty;
}

public class StoreOrderDetailLineDto
{
    public int VariantId { get; set; }
    public string? SkuSnapshot { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal { get; set; }
}
