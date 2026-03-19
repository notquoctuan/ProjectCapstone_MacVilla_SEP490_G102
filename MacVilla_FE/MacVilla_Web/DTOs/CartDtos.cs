namespace MacVilla_Web.DTOs;

public record CartDto(
    long CartId,
    long UserId,
    List<CartItemDto> Items,
    decimal TotalPrice
);

public record CartItemDto(
    long CartItemId,
    long ProductId,
    string ProductName,
    string? ImageUrl,
    decimal UnitPrice,
    int Quantity,
    decimal SubTotal
);

public record AddToCartRequest(long ProductId, int Quantity);
public record UpdateCartItemRequest(long CartItemId, int Quantity);

