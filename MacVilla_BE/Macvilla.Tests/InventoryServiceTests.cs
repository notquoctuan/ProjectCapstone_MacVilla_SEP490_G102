using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace MacVilla.Tests;

public class InventoryServiceTests
{
    private readonly Mock<IInventoryRepository> _inventoryRepoMock = new();
    private readonly InventoryService _service;

    public InventoryServiceTests()
    {
        _service = new InventoryService(_inventoryRepoMock.Object);
    }

    [Fact]
    public async Task GetStatisticsAsync_ShouldComputeCountsCorrectly()
    {
        var list = new List<Inventory>
        {
            new() { InventoryId = 1, Quantity = 10 },
            new() { InventoryId = 2, Quantity = 3 },
            new() { InventoryId = 3, Quantity = 0 },
            new() { InventoryId = 4, Quantity = null }
        };

        _inventoryRepoMock
            .Setup(r => r.SearchAsync(null, 1, int.MaxValue))
            .ReturnsAsync(((IEnumerable<Inventory>)list, list.Count));

        var stats = await _service.GetStatisticsAsync();

        stats.TotalProducts.Should().Be(4);
        stats.TotalQuantity.Should().Be(13); // 10+3+0+0 (null treated as 0)
        stats.LowStockCount.Should().Be(1); // quantity 3
        stats.OutOfStockCount.Should().Be(2); // 0 and null treated as <= 0
    }
}

