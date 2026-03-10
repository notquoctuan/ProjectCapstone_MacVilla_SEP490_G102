using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepo;

    public CartService(ICartRepository cartRepo)
        => _cartRepo = cartRepo;

    public async Task<CartDto> GetCartAsync(long userId)
    {
        var cart = await _cartRepo.GetOrCreateAsync(userId);
        return MapToDto(cart);
    }

    public async Task<CartDto> AddToCartAsync(long userId, AddToCartRequest request)
    {
        if (request.Quantity <= 0)
            throw new ArgumentException("Số lượng phải lớn hơn 0.");

        var cart = await _cartRepo.GetOrCreateAsync(userId);
        var existing = await _cartRepo.GetCartItemAsync(cart.CartId, request.ProductId);

        if (existing is not null)
        {
            existing.Quantity += request.Quantity;
            await _cartRepo.UpdateItemAsync(existing);
        }
        else
        {
            await _cartRepo.AddItemAsync(new CartItem
            {
                CartId = cart.CartId,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                AddedAt = DateTime.UtcNow
            });
        }

        await _cartRepo.SaveAsync();
        return MapToDto(await _cartRepo.GetOrCreateAsync(userId));
    }

    public async Task<CartDto> UpdateItemAsync(long userId, UpdateCartItemRequest request)
    {
        var cart = await _cartRepo.GetOrCreateAsync(userId);
        var item = cart.CartItems.FirstOrDefault(i => i.CartItemId == request.CartItemId)
                   ?? throw new KeyNotFoundException("Không tìm thấy sản phẩm trong giỏ.");

        if (request.Quantity <= 0)
            await _cartRepo.RemoveItemAsync(item);
        else
        {
            item.Quantity = request.Quantity;
            await _cartRepo.UpdateItemAsync(item);
        }

        await _cartRepo.SaveAsync();
        return MapToDto(await _cartRepo.GetOrCreateAsync(userId));
    }

    public async Task<CartDto> RemoveItemAsync(long userId, long cartItemId)
    {
        var cart = await _cartRepo.GetOrCreateAsync(userId);
        var item = cart.CartItems.FirstOrDefault(i => i.CartItemId == cartItemId)
                   ?? throw new KeyNotFoundException("Không tìm thấy sản phẩm trong giỏ.");

        await _cartRepo.RemoveItemAsync(item);
        await _cartRepo.SaveAsync();
        return MapToDto(await _cartRepo.GetOrCreateAsync(userId));
    }

    public async Task ClearCartAsync(long userId)
    {
        var cart = await _cartRepo.GetOrCreateAsync(userId);
        await _cartRepo.ClearCartAsync(cart.CartId);
        await _cartRepo.SaveAsync();
    }

    // ── Helper ────────────────────────────────────────────────
    private static CartDto MapToDto(Cart cart)
    {
        var items = cart.CartItems.Select(i => new CartItemDto(
            i.CartItemId,
            i.ProductId,
            i.Product?.Name ?? "",
            i.Product?.ProductImages?.FirstOrDefault(x => x.IsMain == true)?.ImageUrl,
            i.Product?.Price ?? 0,
            i.Quantity,
            (i.Product?.Price ?? 0) * i.Quantity
        )).ToList();

        return new CartDto(cart.CartId, cart.UserId, items, items.Sum(x => x.SubTotal));
    }
}   