namespace MacVilla_Web.Models
{
    // Cart response from API
    public class CartVM
    {
        public long CartId { get; set; }
        public long UserId { get; set; }
        public List<CartItemVM> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
    }

    public class CartItemVM
    {
        public long CartItemId { get; set; }
        public long ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ImageUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
    }

    // Request models
    public class AddToCartRequest
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateCartItemRequest
    {
        public long CartItemId { get; set; }
        public int Quantity { get; set; }
    }
}
