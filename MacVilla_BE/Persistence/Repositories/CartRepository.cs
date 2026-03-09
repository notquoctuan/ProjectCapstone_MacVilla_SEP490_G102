using System;
using Domain.Entities;
using Domain.Interfaces;
using Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class CartRepository : ICartRepository
{
    private readonly MacvilladbContext _ctx;
    public CartRepository(MacvilladbContext ctx) => _ctx = ctx;

    private IQueryable<Cart> FullQuery()
        => _ctx.Carts
            .Include(c => c.CartItems)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p!.ProductImages);

    public async Task<Cart?> GetByUserIdAsync(long userId)
        => await FullQuery().FirstOrDefaultAsync(c => c.UserId == userId);

    public async Task<Cart> GetOrCreateAsync(long userId)
    {
        var cart = await GetByUserIdAsync(userId);
        if (cart is not null) return cart;

        cart = new Cart { UserId = userId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        _ctx.Carts.Add(cart);
        await _ctx.SaveChangesAsync();
        return await GetByUserIdAsync(userId) ?? cart;
    }

    public async Task<CartItem?> GetCartItemAsync(long cartId, long productId)
        => await _ctx.CartItems.FirstOrDefaultAsync(i => i.CartId == cartId && i.ProductId == productId);

    public async Task AddItemAsync(CartItem item) => await _ctx.CartItems.AddAsync(item);
    public Task UpdateItemAsync(CartItem item) { _ctx.CartItems.Update(item); return Task.CompletedTask; }
    public Task RemoveItemAsync(CartItem item) { _ctx.CartItems.Remove(item); return Task.CompletedTask; }

    public async Task ClearCartAsync(long cartId)
    {
        var items = await _ctx.CartItems.Where(i => i.CartId == cartId).ToListAsync();
        _ctx.CartItems.RemoveRange(items);
    }

    public async Task SaveAsync() => await _ctx.SaveChangesAsync();
}