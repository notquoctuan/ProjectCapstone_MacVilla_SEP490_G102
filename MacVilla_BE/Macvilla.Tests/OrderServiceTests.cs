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
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _orderRepoMock;
        private readonly Mock<IInventoryRepository> _inventoryRepoMock;
        private readonly Mock<IOrderTransaction> _transactionMock;
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            _orderRepoMock = new Mock<IOrderRepository>();
            _inventoryRepoMock = new Mock<IInventoryRepository>();
            _transactionMock = new Mock<IOrderTransaction>();

            // Setup transaction mock — required for all UpdateOrderStatus and CancelOrder tests
            _transactionMock.Setup(t => t.CommitAsync()).Returns(Task.CompletedTask);
            _transactionMock.Setup(t => t.RollbackAsync()).Returns(Task.CompletedTask);
            _transactionMock.Setup(t => t.DisposeAsync()).Returns(ValueTask.CompletedTask);
            _orderRepoMock
                .Setup(r => r.BeginTransactionAsync())
                .ReturnsAsync(_transactionMock.Object);

            _service = new OrderService(_orderRepoMock.Object, _inventoryRepoMock.Object);
        }

        [Fact]
        public async Task GetOrderDetailAsync_ShouldReturnNull_WhenNotFound()
        {
            _orderRepoMock.Setup(r => r.GetOrderDetailByIdAsync(It.IsAny<long>())).ReturnsAsync((Order?)null);

            var result = await _service.GetOrderDetailAsync(99);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetOrderDetailAsync_ShouldMapOrderDetail_WhenExists()
        {
            var order = new Order
            {
                OrderId = 10,
                CreatedAt = DateTime.UtcNow,
                Status = "Processing",
                TotalAmount = 250m,
                User = new User { UserId = 1, FullName = "Alice", Email = "a@x.com", Phone = "0123" },
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        OrderItemId = 101,
                        ProductId = 5,
                        Product = new Product
                        {
                            ProductId = 5,
                            Name = "Prod5",
                            Category = new Category { CategoryName = "CatX" },
                            ProductImages = new List<ProductImage> { new ProductImage { ImageUrl = "/img.jpg", IsMain = true } }
                        },
                        Quantity = 2,
                        UnitPrice = 50m
                    }
                },
                Payments = new List<Payment>
                {
                    new Payment { PaymentId = 201, PaymentMethod = "Card", PaymentStatus = "Paid", PaidAt = DateTime.UtcNow }
                },
                Shippings = new List<Shipping>
                {
                    new Shipping
                    {
                        ShippingId = 301,
                        Status = "Shipped",
                        ShippingFee = 5m,
                        DeliveredDate = DateTime.UtcNow,
                        ShippingAddress = new ShippingAddress { ReceiverName = "Alice", Phone = "0123", AddressLine = "Addr" },
                        ShippingMethod = new ShippingMethod { MethodName = "Express", BaseFee = 5m, EstimatedDays = 1 }
                    }
                }
            };

            _orderRepoMock.Setup(r => r.GetOrderDetailByIdAsync(order.OrderId)).ReturnsAsync(order);

            var detail = await _service.GetOrderDetailAsync(order.OrderId);

            detail.Should().NotBeNull();
            detail!.OrderId.Should().Be(order.OrderId);
            detail.Customer.Should().NotBeNull();
            detail.Customer!.FullName.Should().Be("Alice");
            detail.OrderItems.Should().HaveCount(1);
            var item = detail.OrderItems.First();
            item.ProductName.Should().Be("Prod5");
            item.ProductImageUrl.Should().Be("/img.jpg");
            item.CategoryName.Should().Be("CatX");
            detail.Payments.Should().HaveCount(1);
            detail.Shippings.Should().HaveCount(1);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_ShouldReturnFalse_WhenOrderNotFound()
        {
            _orderRepoMock.Setup(r => r.GetOrderByIdAsync(It.IsAny<long>())).ReturnsAsync((Order?)null);

            var req = new UpdateOrderStatusRequest { Status = "Processing" };
            var result = await _service.UpdateOrderStatusAsync(999, req);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_ShouldThrow_WhenInvalidStatus()
        {
            var req = new UpdateOrderStatusRequest { Status = "NotAStatus" };

            Func<Task> act = async () => await _service.UpdateOrderStatusAsync(1, req);

            await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Invalid status*");
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_ShouldThrow_WhenInvalidTransition()
        {
            var order = new Order { OrderId = 2, Status = "Completed" };
            _orderRepoMock.Setup(r => r.GetOrderByIdAsync(order.OrderId)).ReturnsAsync(order);

            var req = new UpdateOrderStatusRequest { Status = "Processing" };

            Func<Task> act = async () => await _service.UpdateOrderStatusAsync(order.OrderId, req);

            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Cannot transition*");
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_ShouldReduceInventory_AndUpdateOrder_WhenTransitionToProcessing()
        {
            var order = new Order
            {
                OrderId = 3,
                Status = "Pending",
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = 100, Quantity = 2 },
                    new OrderItem { ProductId = 101, Quantity = 1 }
                }
            };

            _orderRepoMock.Setup(r => r.GetOrderByIdAsync(order.OrderId)).ReturnsAsync(order);
            _inventoryRepoMock.Setup(r => r.ReduceInventoryAsync(100, 2, It.IsAny<string>())).ReturnsAsync(true);
            _inventoryRepoMock.Setup(r => r.ReduceInventoryAsync(101, 1, It.IsAny<string>())).ReturnsAsync(true);
            _orderRepoMock.Setup(r => r.UpdateOrderAsync(It.IsAny<Order>())).ReturnsAsync((Order o) => o);

            var req = new UpdateOrderStatusRequest { Status = "Processing" };
            var result = await _service.UpdateOrderStatusAsync(order.OrderId, req);

            result.Should().BeTrue();
            _inventoryRepoMock.Verify(r => r.ReduceInventoryAsync(100, 2, It.IsAny<string>()), Times.Once);
            _inventoryRepoMock.Verify(r => r.ReduceInventoryAsync(101, 1, It.IsAny<string>()), Times.Once);
            _orderRepoMock.Verify(r => r.UpdateOrderAsync(It.Is<Order>(o => o.Status == "Processing")), Times.Once);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_ShouldThrow_WhenReduceInventoryFails()
        {
            var order = new Order
            {
                OrderId = 4,
                Status = "Pending",
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = 200, Quantity = 5 }
                }
            };

            _orderRepoMock.Setup(r => r.GetOrderByIdAsync(order.OrderId)).ReturnsAsync(order);
            _inventoryRepoMock.Setup(r => r.ReduceInventoryAsync(200, 5, It.IsAny<string>())).ReturnsAsync(false);

            var req = new UpdateOrderStatusRequest { Status = "Processing" };

            Func<Task> act = async () => await _service.UpdateOrderStatusAsync(order.OrderId, req);

            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Insufficient inventory*");
            _inventoryRepoMock.Verify(r => r.ReduceInventoryAsync(200, 5, It.IsAny<string>()), Times.Once);
            _orderRepoMock.Verify(r => r.UpdateOrderAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task CancelOrderAsync_ShouldReturnFalse_WhenOrderNotFound()
        {
            _orderRepoMock.Setup(r => r.GetOrderByIdAsync(It.IsAny<long>())).ReturnsAsync((Order?)null);

            var result = await _service.CancelOrderAsync(123, "no reason");

            result.Should().BeFalse();
        }

        [Fact]
        public async Task CancelOrderAsync_ShouldThrow_WhenOrderCompleted()
        {
            var order = new Order { OrderId = 5, Status = "Completed" };
            _orderRepoMock.Setup(r => r.GetOrderByIdAsync(order.OrderId)).ReturnsAsync(order);

            Func<Task> act = async () => await _service.CancelOrderAsync(order.OrderId, "change of mind");

            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Cannot cancel a completed order*");
        }

        [Fact]
        public async Task CancelOrderAsync_ShouldRestoreInventory_AndCancelOrder_WhenProcessing()
        {
            var order = new Order
            {
                OrderId = 6,
                Status = "Processing",
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = 300, Quantity = 2 }
                }
            };

            _orderRepoMock.Setup(r => r.GetOrderByIdAsync(order.OrderId)).ReturnsAsync(order);
            _inventoryRepoMock.Setup(r => r.RestoreInventoryAsync(300, 2, It.IsAny<string>())).ReturnsAsync(true);
            _orderRepoMock.Setup(r => r.UpdateOrderAsync(It.IsAny<Order>())).ReturnsAsync((Order o) => o);

            var result = await _service.CancelOrderAsync(order.OrderId, "customer cancel");

            result.Should().BeTrue();
            _inventoryRepoMock.Verify(r => r.RestoreInventoryAsync(300, 2, It.IsAny<string>()), Times.Once);
            _orderRepoMock.Verify(r => r.UpdateOrderAsync(It.Is<Order>(o => o.Status == "Cancelled")), Times.Once);
        }

        [Fact]
        public async Task GetOrderTrackingAsync_ShouldMapTrackingInfo_WhenExists()
        {
            var order = new Order
            {
                OrderId = 7,
                CreatedAt = DateTime.UtcNow.AddHours(-2),
                Status = "Shipped",
                Payments = new List<Payment> { new Payment { PaymentStatus = "Paid", PaidAt = DateTime.UtcNow.AddHours(-1) } },
                Shippings = new List<Shipping> { new Shipping { Status = "Delivered", DeliveredDate = DateTime.UtcNow } }
            };

            _orderRepoMock.Setup(r => r.GetOrderByIdAsync(order.OrderId)).ReturnsAsync(order);

            var track = await _service.GetOrderTrackingAsync(order.OrderId);

            track.Should().NotBeNull();
            track!.OrderId.Should().Be(order.OrderId);
            track.Timeline.IsOrderPlaced.Should().BeTrue();
            track.Timeline.IsShipped.Should().BeTrue();
            track.Timeline.IsDelivered.Should().BeTrue();
        }
    }
}