using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace MacVilla.Tests
{
    public class InventoryServiceTests
    {
        private readonly Mock<IInventoryRepository> _inventoryRepoMock;
        private readonly InventoryService _service;

        public InventoryServiceTests()
        {
            _inventoryRepoMock = new Mock<IInventoryRepository>();
            _service = new InventoryService(_inventoryRepoMock.Object);
        }

        #region TC36: SearchInventoriesAsync
        [Fact]
        public async Task SearchInventoriesAsync_ShouldReturnPagedData_WithCorrectMapping()
        {
            // Arrange
            var request = new InventorySearchRequest { Keyword = "Sơn", PageNumber = 1, PageSize = 10 };
            var mockData = new List<Inventory>
            {
                new Inventory { InventoryId = 1, ProductId = 101, Quantity = 50, Product = new Product { Name = "Sơn Dulux" } }
            };

            _inventoryRepoMock.Setup(r => r.SearchAsync("Sơn", 1, 10))
                .ReturnsAsync((mockData, 1));

            // Act
            var result = await _service.SearchInventoriesAsync(request);

            // Assert
            result.Data.Should().HaveCount(1);
            result.TotalCount.Should().Be(1);
            result.Data.First().ProductName.Should().Be("Sơn Dulux");
        }
        #endregion

        #region TC40: GetStatisticsAsync
        [Fact]
        public async Task GetStatisticsAsync_ShouldCalculateLowStockAndOutOfStockCorrectly()
        {
            // Arrange
            var inventories = new List<Inventory>
            {
                new Inventory { Quantity = 0 }, // Out of stock
                new Inventory { Quantity = -1 }, // Out of stock
                new Inventory { Quantity = 3 }, // Low stock (<= 5)
                new Inventory { Quantity = 10 } // Normal
            };

            _inventoryRepoMock.Setup(r => r.SearchAsync(null, 1, int.MaxValue))
                .ReturnsAsync((inventories, 4));

            // Act
            var result = await _service.GetStatisticsAsync();

            // Assert
            result.OutOfStockCount.Should().Be(2);
            result.LowStockCount.Should().Be(1);
            result.TotalProducts.Should().Be(4);
        }
        #endregion

        #region TC41, TC42, TC43: UpdateInventoryAsync
        [Fact]
        public async Task UpdateInventoryAsync_ShouldThrowException_WhenQuantityIsNegative()
        {
            // Arrange (TC43)
            var request = new UpdateInventoryRequest { Quantity = -5 };

            // Act
            Func<Task> act = async () => await _service.UpdateInventoryAsync(1, request);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*không được âm*");
        }
        #endregion

        #region TC44, TC46, TC47: AdjustInventoryAsync
        [Fact]
        public async Task AdjustInventoryAsync_ShouldThrowException_WhenChangeAmountIsZero()
        {
            // Arrange (TC44)
            var request = new AdjustInventoryRequest { QuantityChange = 0, Reason = "No change" };

            // Act
            Func<Task> act = async () => await _service.AdjustInventoryAsync(1, request);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task AdjustInventoryAsync_ShouldBlockDecrease_WhenNoStockRecordExists()
        {
            // Arrange (TC46)
            var request = new AdjustInventoryRequest { QuantityChange = -10, Reason = "Reduce" };
            _inventoryRepoMock.Setup(r => r.GetInventoryByProductIdAsync(1))
                .ReturnsAsync((Inventory)null);

            // Act
            Func<Task> act = async () => await _service.AdjustInventoryAsync(1, request);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*Không thể giảm tồn kho*");
        }

        [Fact]
        public async Task AdjustInventoryAsync_ShouldLogHistory_WhenAdjustmentIsSuccessful()
        {
            // Arrange (TC47)
            var existingInventory = new Inventory { InventoryId = 1, Quantity = 50 };
            var request = new AdjustInventoryRequest { QuantityChange = 20, Reason = "Import more" };

            _inventoryRepoMock.Setup(r => r.GetInventoryByProductIdAsync(1))
                .ReturnsAsync(existingInventory);
            _inventoryRepoMock.Setup(r => r.GetHistoryAsync(1))
                .ReturnsAsync(new List<InventoryHistory>());

            // Act
            await _service.AdjustInventoryAsync(1, request);

            // Assert
            existingInventory.Quantity.Should().Be(70);
            _inventoryRepoMock.Verify(r => r.AddInventoryHistoryAsync(1, 20, "Import more"), Times.Once);
        }
        #endregion
    }
}