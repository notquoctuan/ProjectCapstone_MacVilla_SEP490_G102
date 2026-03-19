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
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> _categoryRepoMock;
        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _categoryRepoMock = new Mock<ICategoryRepository>();
            _service = new CategoryService(_categoryRepoMock.Object);
        }

        #region TC22: GetCategoriesOrderedAsync
        [Fact]
        public async Task GetCategoriesOrderedAsync_ShouldReturnHierarchyOrdered()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { CategoryId = 1, CategoryName = "RootA", ParentCategoryId = null },
                new Category { CategoryId = 2, CategoryName = "RootB", ParentCategoryId = null },
                new Category { CategoryId = 3, CategoryName = "ChildA1", ParentCategoryId = 1 }
            };
            _categoryRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

            // Act
            var result = (await _service.GetCategoriesOrderedAsync()).ToList();

            // Assert
            result[0].CategoryId.Should().Be(1); // RootA
            result[1].CategoryId.Should().Be(3); // ChildA1 (nằm dưới RootA)
            result[2].CategoryId.Should().Be(2); // RootB
        }
        #endregion

        #region TC23: GetAllCategoriesAsync
        [Fact]
        public async Task GetAllCategoriesAsync_ShouldReturnAllFromRepository()
        {
            var categories = new List<Category> { new Category(), new Category() };
            _categoryRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

            var result = await _service.GetAllCategoriesAsync();

            result.Should().HaveCount(2);
        }
        #endregion

        #region TC24 & TC25: GetCategoryByIdAsync
        [Fact]
        public async Task GetCategoryByIdAsync_ShouldReturnCategory_WhenIdExists()
        {
            var cat = new Category { CategoryId = 10, CategoryName = "Test" };
            _categoryRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(cat);

            var result = await _service.GetCategoryByIdAsync(10);

            result.Should().NotBeNull();
            result!.CategoryName.Should().Be("Test");
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ShouldReturnNull_WhenIdDoesNotExist()
        {
            _categoryRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((Category?)null);

            var result = await _service.GetCategoryByIdAsync(99);

            result.Should().BeNull();
        }
        #endregion

        #region TC26: CreateCategoryAsync
        [Fact]
        public async Task CreateCategoryAsync_ShouldCreateWithIsActiveTrue_WhenValid()
        {
            // Arrange
            var name = "Gạch Men";
            _categoryRepoMock.Setup(r => r.ExistsByNameAsync(It.IsAny<string>(), It.IsAny<long?>())).ReturnsAsync(false);
            _categoryRepoMock.Setup(r => r.CreateAsync(It.IsAny<Category>()))
                             .ReturnsAsync((Category c) => { c.CategoryId = 1; return c; });

            // Act
            var result = await _service.CreateCategoryAsync(name, null);

            // Assert
            result.IsActive.Should().BeTrue();
            result.CategoryName.Should().Be(name);
            _categoryRepoMock.Verify(r => r.CreateAsync(It.Is<Category>(c => c.CategoryName == name)), Times.Once);
        }

        [Fact]
        public async Task CreateCategoryAsync_ShouldThrowException_WhenNameAlreadyExists()
        {
            _categoryRepoMock.Setup(r => r.ExistsByNameAsync("Duplicate", null)).ReturnsAsync(true);

            Func<Task> act = async () => await _service.CreateCategoryAsync("Duplicate", null);

            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*đã tồn tại*");
        }
        #endregion

        #region TC27 & TC28: UpdateCategoryAsync
        [Fact]
        public async Task UpdateCategoryAsync_ShouldReturnNull_WhenCategoryNotFound()
        {
            _categoryRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((Category?)null);

            var result = await _service.UpdateCategoryAsync(1, "New Name", null);

            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateCategoryAsync_ShouldUpdateSuccessfully_WhenExists()
        {
            // Arrange
            var existing = new Category { CategoryId = 1, CategoryName = "Old" };
            _categoryRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _categoryRepoMock.Setup(r => r.ExistsByNameAsync("New", 1)).ReturnsAsync(false);
            _categoryRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Category>())).ReturnsAsync((Category c) => c);

            // Act
            var result = await _service.UpdateCategoryAsync(1, "New", 2);

            // Assert
            result!.CategoryName.Should().Be("New");
            result.ParentCategoryId.Should().Be(2);
        }
        #endregion

        #region TC29: DeleteCategoryAsync
        [Fact]
        public async Task DeleteCategoryAsync_ShouldThrowException_WhenHasProducts()
        {
            _categoryRepoMock.Setup(r => r.HasProductsAsync(1)).ReturnsAsync(true);

            Func<Task> act = async () => await _service.DeleteCategoryAsync(1);

            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*không thể xóa*");
        }

        [Fact]
        public async Task DeleteCategoryAsync_ShouldReturnTrue_WhenNoProducts()
        {
            _categoryRepoMock.Setup(r => r.HasProductsAsync(1)).ReturnsAsync(false);
            _categoryRepoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _service.DeleteCategoryAsync(1);

            result.Should().BeTrue();
        }
        #endregion

        #region TC30 & TC31: Activate/Deactivate
        [Fact]
        public async Task ActivateCategoryAsync_ShouldCallRepositoryWithTrue()
        {
            _categoryRepoMock.Setup(r => r.SetIsActiveAsync(1, true)).ReturnsAsync(true);

            var result = await _service.ActivateCategoryAsync(1);

            result.Should().BeTrue();
            _categoryRepoMock.Verify(r => r.SetIsActiveAsync(1, true), Times.Once);
        }

        [Fact]
        public async Task DeactivateCategoryAsync_ShouldCallRepositoryWithFalse()
        {
            _categoryRepoMock.Setup(r => r.SetIsActiveAsync(1, false)).ReturnsAsync(true);

            var result = await _service.DeactivateCategoryAsync(1);

            result.Should().BeTrue();
            _categoryRepoMock.Verify(r => r.SetIsActiveAsync(1, false), Times.Once);
        }
        #endregion

        #region TC32: SearchCategoriesAsync
        [Fact]
        public async Task SearchCategoriesAsync_ShouldReturnPaginatedResponse()
        {
            // Arrange
            var list = new List<Category> { new Category { CategoryName = "A" } };
            _categoryRepoMock.Setup(r => r.SearchAsync("A", true, 1, 10))
                             .ReturnsAsync((list, 1));

            var request = new CategorySearchRequest { Name = "A", IsActive = true, PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _service.SearchCategoriesAsync(request);

            // Assert
            result.Data.Should().HaveCount(1);
            result.TotalCount.Should().Be(1);
            result.TotalPages.Should().Be(1);
        }
        #endregion
    }
}