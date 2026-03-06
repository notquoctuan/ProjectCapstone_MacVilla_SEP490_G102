using Application.DTOs;

namespace Application.Interfaces;

public interface ICartService
{
    Task<CartDto> GetCartAsync(long userId);
    Task<CartDto> AddToCartAsync(long userId, AddToCartRequest request);
    Task<CartDto> UpdateItemAsync(long userId, UpdateCartItemRequest request);
    Task<CartDto> RemoveItemAsync(long userId, long cartItemId);
    Task ClearCartAsync(long userId);
}