using MacVilla_Web.DTOs;

namespace MacVilla_Web.Models;

public class CartViewModel
{
    public CartDto? Cart { get; set; }

    public decimal TotalPrice => Cart?.TotalPrice ?? 0;

    public int TotalItems => Cart?.Items.Sum(i => i.Quantity) ?? 0;
}

