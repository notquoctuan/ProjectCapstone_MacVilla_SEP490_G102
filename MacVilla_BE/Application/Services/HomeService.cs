using System;
using System.Linq;
using System.Collections.Generic;
using Application.DTOs;
using Application.Interfaces;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class HomeService : IHomeService
{
    private readonly IBannerRepository _bannerRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IOrderRepository _orderRepository;

    public HomeService(
        IBannerRepository bannerRepository,
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IOrderRepository orderRepository)
    {
        _bannerRepository = bannerRepository;
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _orderRepository = orderRepository;
    }

    public async Task<IEnumerable<BannerResponse>> GetActiveBannersAsync()
    {
        var banners = await _bannerRepository.GetActiveBannersAsync();
        return banners.Select(b => new BannerResponse
        {
            BannerId = b.BannerId,
            Title = b.Title,
            ImageUrl = b.ImageUrl,
            LinkUrl = b.LinkUrl,
            DisplayOrder = b.DisplayOrder ?? 0
        });
    }

    public async Task<PagedResponse<ProductAdminResponse>> SearchProductsAsync(ProductSearchPublicRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        var keyword = (request.Keyword ?? string.Empty).Trim();

        if (string.IsNullOrEmpty(keyword))
            throw new ArgumentException("Keyword cannot be empty");

        if (keyword.Length > 100)
            throw new ArgumentException("Keyword is too long");

        // Basic sanitization: remove wildcard characters
        keyword = keyword.Replace("%", string.Empty).Replace("_", string.Empty);

        var query = _productRepository.GetQueryable();

        query = query.Where(p => (p.Name != null && EF.Functions.Like(p.Name, $"%{keyword}%"))
                                 || (p.Description != null && EF.Functions.Like(p.Description, $"%{keyword}%")));

        int total = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProductAdminResponse
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price ?? 0,
                CategoryName = p.Category != null ? p.Category.CategoryName : "N/A",
                Status = p.Status,
                CreatedAt = p.CreatedAt ?? DateTime.Now,
                ImageUrl = p.ProductImages.OrderByDescending(img => img.IsMain).Select(img => img.ImageUrl).FirstOrDefault()
            })
            .ToListAsync();

        return new PagedResponse<ProductAdminResponse>
        {
            Data = items,
            TotalCount = total,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<IEnumerable<ProductAdminResponse>> GetFeaturedProductsAsync(int limit = 8)
    {
        var productsQuery = _productRepository.GetQueryable();

        // compute sales from orders
        var orders = await _orderRepository.GetAllOrdersAsync();
        var salesByProduct = orders
            .SelectMany(o => o.OrderItems)
            .Where(oi => oi.ProductId.HasValue)
            .GroupBy(oi => oi.ProductId!.Value)
            .ToDictionary(g => g.Key, g => g.Sum(oi => oi.Quantity));

        var products = await productsQuery
            .Select(p => new
            {
                Product = p,
                Sales = salesByProduct.ContainsKey(p.ProductId) ? salesByProduct[p.ProductId] : 0
            })
            .ToListAsync();

        var ordered = products
            .OrderByDescending(x => x.Sales)
            .ThenByDescending(x => x.Product.CreatedAt)
            .Take(limit)
            .Select(x => new ProductAdminResponse
            {
                ProductId = x.Product.ProductId,
                Name = x.Product.Name,
                Price = x.Product.Price ?? 0,
                CategoryName = x.Product.Category != null ? x.Product.Category.CategoryName : "N/A",
                Status = x.Product.Status,
                CreatedAt = x.Product.CreatedAt ?? DateTime.Now,
                ImageUrl = x.Product.ProductImages.OrderByDescending(img => img.IsMain).Select(img => img.ImageUrl).FirstOrDefault()
            });

        return ordered;
    }

    public async Task<IEnumerable<CategoryTreeResponse>> GetCategoryTreeAsync()
    {
        var all = (await _categoryRepository.GetAllAsync()).Where(c => c.IsActive == true).ToList();

        var lookup = all.ToDictionary(c => c.CategoryId, c => new CategoryTreeResponse
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName
        });

        var roots = new List<CategoryTreeResponse>();

        foreach (var cat in all)
        {
            if (cat.ParentCategoryId.HasValue && lookup.ContainsKey(cat.ParentCategoryId.Value))
            {
                lookup[cat.ParentCategoryId.Value].Children.Add(lookup[cat.CategoryId]);
            }
            else
            {
                roots.Add(lookup[cat.CategoryId]);
            }
        }

        return roots.OrderBy(c => c.CategoryName).ToList();
    }
}