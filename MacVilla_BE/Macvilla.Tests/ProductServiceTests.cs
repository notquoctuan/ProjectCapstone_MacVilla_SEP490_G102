using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Http;

namespace MacVilla.Tests
{
    public class ProductServiceTests
    {
        #region EF Core Async Helpers
        internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;
            public TestAsyncQueryProvider(IQueryProvider inner) => _inner = inner;
            public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerable<TEntity>(expression);
            public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new TestAsyncEnumerable<TElement>(expression);
            public object? Execute(Expression expression) => _inner.Execute(expression);
            public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);
            public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            {
                var expectedResultType = typeof(TResult).GetGenericArguments()[0];

                var executionResult = typeof(IQueryProvider)
                    .GetMethod(
                        name: nameof(IQueryProvider.Execute),
                        genericParameterCount: 1,
                        types: new[] { typeof(Expression) })!
                    .MakeGenericMethod(expectedResultType)
                    .Invoke(_inner, new object[] { expression });

                return (TResult)typeof(Task)
                    .GetMethod(nameof(Task.FromResult))!
                    .MakeGenericMethod(expectedResultType)
                    .Invoke(null, new[] { executionResult })!;
            }
        }
            
        internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
            public TestAsyncEnumerable(Expression expression) : base(expression) { }
            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
        }

        internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;
            public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;
            public ValueTask DisposeAsync() { _inner.Dispose(); return default; }
            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());
            public T Current => _inner.Current;
        }
        #endregion

        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly Mock<ICategoryRepository> _categoryRepoMock;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _productRepoMock = new Mock<IProductRepository>();
            _categoryRepoMock = new Mock<ICategoryRepository>();
            _service = new ProductService(_productRepoMock.Object, _categoryRepoMock.Object);
        }

        // --- CREATE TESTS ---

        [Fact]
        public async Task CreateProductAsync_ShouldThrow_WhenCategoryNotExist()
        {
            // Arrange
            _categoryRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((Category?)null);

            var request = new ProductCreateRequest { CategoryId = 999, Name = "Test", Price = 100 };

            // Act
            Func<Task> act = async () => await _service.CreateProductAsync(request, "dummyPath");

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("*không tồn tại*");
        }

        [Fact]
        public async Task CreateProductAsync_ShouldReturnResponse_WhenValid()
        {
            // Arrange
            var category = new Category { CategoryId = 1, CategoryName = "Sanitary" };
            _categoryRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);
            _productRepoMock.Setup(r => r.ExistsByNameAsync(It.IsAny<string>(), null)).ReturnsAsync(false);

            _productRepoMock.Setup(r => r.AddAsync(It.IsAny<Product>()))
                .ReturnsAsync((Product p) => { p.ProductId = 500; return p; });

            var request = new ProductCreateRequest
            {
                Name = "Lavabo Luxury",
                CategoryId = 1,
                Price = 1500,
                Status = "Active"
            };

            // Act
            var result = await _service.CreateProductAsync(request, "tempPath");

            // Assert
            result.Should().NotBeNull();
            result.ProductId.Should().Be(500);
            result.Name.Should().Be("Lavabo Luxury");
            result.CategoryName.Should().Be("Sanitary");
        }

        // --- GET PAGED TESTS ---

        [Fact]
        public async Task GetPagedProductsAsync_ShouldReturnPagedResult()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { ProductId = 1, Name = "P1", Price = 10, CreatedAt = DateTime.UtcNow, Category = new Category{ CategoryName="C1"} },
                new Product { ProductId = 2, Name = "P2", Price = 20, CreatedAt = DateTime.UtcNow, Category = new Category{ CategoryName="C1"} }
            }.AsQueryable();

            var asyncQuery = new TestAsyncEnumerable<Product>(products);
            _productRepoMock.Setup(r => r.GetQueryable()).Returns(asyncQuery);

            var request = new ProductSearchRequest { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _service.GetPagedProductsAsync(request);

            // Assert
            result.Data.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
        }

        // --- UPDATE TESTS ---

        [Fact]
        public async Task UpdateProductAsync_ShouldThrow_WhenProductNotFound()
        {
            // Arrange
            _productRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((Product?)null);

            // Act
            Func<Task> act = async () => await _service.UpdateProductAsync(1, new ProductUpdateRequest(), "path");

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("*Không tìm thấy sản phẩm*");
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldCallUpdate_WhenValid()
        {
            // Arrange
            var existing = new Product { ProductId = 1, Name = "Old", ProductImages = new List<ProductImage>() };
            _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _categoryRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(new Category { CategoryName = "Cat" });

            var request = new ProductUpdateRequest { Name = "New Name", CategoryId = 2, Price = 200 };

            // Act
            await _service.UpdateProductAsync(1, request, "path");

            // Assert
            _productRepoMock.Verify(r => r.ExistsByNameAsync("New Name", 1), Times.Once);
        }

        // --- DETAIL TESTS ---

        [Fact]
        public async Task GetProductDetailAsync_ShouldReturnMappedDto_WhenFound()
        {
            // Arrange
            var product = new Product
            {
                ProductId = 10,
                Name = "Bồn cầu",
                Price = 5000,
                Category = new Category { CategoryName = "Thiết bị vệ sinh" },
                ProductImages = new List<ProductImage>
                {
                    new ProductImage { ImageId = 1, ImageUrl = "img.jpg", IsMain = true }
                }
            };
            _productRepoMock.Setup(r => r.GetByIdDetailAsync(10)).ReturnsAsync(product);

            // Act
            var result = await _service.GetProductDetailAsync(10);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Bồn cầu");
            result.CategoryName.Should().Be("Thiết bị vệ sinh");
            result.Images.Should().ContainSingle(i => i.IsMain);
        }
    }
}