using Application.DTOs;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MacVilla.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly Mock<ICategoryRepository> _categoryRepoMock;
        private readonly ProductService _service;
        private const string WebRootPath = "wwwroot";

        public ProductServiceTests()
        {
            _productRepoMock = new Mock<IProductRepository>();
            _categoryRepoMock = new Mock<ICategoryRepository>();
            _service = new ProductService(_productRepoMock.Object, _categoryRepoMock.Object);
        }

        #region TC5 & TC6: CreateProductAsync
        [Fact]
        public async Task CreateProductAsync_ShouldThrowException_WhenCategoryDoesNotExist()
        {
            // Arrange (TC5)
            _categoryRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((Category)null);
            var request = new ProductCreateRequest { CategoryId = 99, Name = "Test" };

            // Act
            Func<Task> act = async () => await _service.CreateProductAsync(request, WebRootPath);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("*không tồn tại*");
        }

        [Fact]
        public async Task CreateProductAsync_ShouldReturnResponse_WhenValid()
        {
            // Arrange (TC6)
            var category = new Category { CategoryId = 1, CategoryName = "Sơn" };
            var request = new ProductCreateRequest { CategoryId = 1, Name = "Sơn Dulux", Price = 100000 };

            _categoryRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);
            _productRepoMock.Setup(r => r.ExistsByNameAsync(It.IsAny<string>(), null)).ReturnsAsync(false);
            _productRepoMock.Setup(r => r.AddAsync(It.IsAny<Product>())).ReturnsAsync((Product p) => {
                p.ProductId = 10; // Giả lập DB sinh ID
                return p;
            });

            // Act
            var result = await _service.CreateProductAsync(request, WebRootPath);

            // Assert
            result.ProductId.Should().Be(10);
            result.Name.Should().Be("Sơn Dulux");
            result.CategoryName.Should().Be("Sơn");
        }
        #endregion

        #region TC9, TC10: UpdateProductAsync
        [Fact]
        public async Task UpdateProductAsync_ShouldThrowException_WhenProductNotFound()
        {
            // Arrange (TC9)
            _productRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((Product)null);
            var request = new ProductUpdateRequest { CategoryId = 1, Name = "Update" };
            // Act
            Func<Task> act = async () => await _service.UpdateProductAsync(1, request, WebRootPath);
            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>();
        }
        #endregion

        #region TC11, TC12: ChangeProductStatus
        [Fact]
        public async Task ChangeStatusAsync_ShouldThrowException_WhenStatusIsInvalid()
        {
            // Arrange (TC11)
            var invalidStatus = "WrongStatus";

            // Act
            Func<Task> act = async () => await _service.ChangeStatusAsync(1, invalidStatus);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Trạng thái không hợp lệ*");
        }

        [Fact]
        public async Task ChangeStatusAsync_ShouldReturnTrue_WhenValidStatus()
        {
            // Arrange (TC12)
            _productRepoMock.Setup(r => r.UpdateStatusAsync(1, "Enable")).ReturnsAsync(true);

            // Act
            await _service.ChangeStatusAsync(1, "Enable");

            // Assert
            _productRepoMock.Verify(r => r.UpdateStatusAsync(1, "Enable"), Times.Once);
        }
        #endregion

        #region TC13, TC14: GetProductDetail
        [Fact]
        public async Task GetProductDetailAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Arrange (TC13)
            _productRepoMock.Setup(r => r.GetByIdDetailAsync(99)).ReturnsAsync((Product)null);

            // Act
            var result = await _service.GetProductDetailAsync(99);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetProductDetailAsync_ShouldReturnCorrectData_IncludingImages()
        {
            // Arrange (TC14)
            var product = new Product
            {
                ProductId = 1,
                Name = "Sản phẩm 1",
                ProductImages = new List<ProductImage>
                {
                    new ProductImage { ImageId = 101, ImageUrl = "img1.jpg", IsMain = true },
                    new ProductImage { ImageId = 102, ImageUrl = "img2.jpg", IsMain = false }
                }
            };
            _productRepoMock.Setup(r => r.GetByIdDetailAsync(1)).ReturnsAsync(product);

            // Act
            var result = await _service.GetProductDetailAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Sản phẩm 1");
            result.Images.Should().HaveCount(2);
            result.Images.First(i => i.IsMain).ImageUrl.Should().Be("img1.jpg");
        }
        #endregion
    }
}