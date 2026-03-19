namespace MacVilla_Web.Models
{
    public class CartDto
    {
        public long CartId { get; set; }
        public long UserId { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
    }

    public class CartItemDto
    {
        public long CartItemId { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
    }

    public class AddToCartRequest
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class UpdateCartItemRequest
    {
        public long CartItemId { get; set; }
        public int Quantity { get; set; }
    }
}
