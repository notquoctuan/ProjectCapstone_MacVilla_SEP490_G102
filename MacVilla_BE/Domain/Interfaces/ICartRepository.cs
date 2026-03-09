using Domain.Entities;

namespace Domain.Interfaces;

public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(long userId);
    Task<Cart> GetOrCreateAsync(long userId);
    Task<CartItem?> GetCartItemAsync(long cartId, long productId);
    Task AddItemAsync(CartItem item);
    Task UpdateItemAsync(CartItem item);
    Task RemoveItemAsync(CartItem item);
    Task ClearCartAsync(long cartId);
    Task SaveAsync();
}