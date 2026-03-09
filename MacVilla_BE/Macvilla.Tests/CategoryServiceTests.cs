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

        [Fact]
        public async Task GetCategoriesOrderedAsync_ShouldReturnHierarchyOrdered()
        {
            var categories = new List<Category>
            {
                new Category { CategoryId = 3, CategoryName = "ChildB1", ParentCategoryId = 2 },
                new Category { CategoryId = 1, CategoryName = "RootA", ParentCategoryId = null },
                new Category { CategoryId = 2, CategoryName = "RootB", ParentCategoryId = null },
                new Category { CategoryId = 4, CategoryName = "ChildA1", ParentCategoryId = 1 },
                new Category { CategoryId = 5, CategoryName = "GrandChildA1", ParentCategoryId = 4 }
            };
            _categoryRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

            var ordered = (await _service.GetCategoriesOrderedAsync()).ToList();

            ordered.Select(c => c.CategoryId).Should().ContainInOrder(1, 4, 5, 2, 3);
        }

        [Fact]
        public async Task CreateCategoryAsync_ShouldCreateWithIsActiveTrue()
        {
            var name = "NewCat";
            // Dùng It.IsAny để tránh lỗi tham số tùy chọn/Expression Tree
            _categoryRepoMock.Setup(r => r.CreateAsync(It.IsAny<Category>()))
                .ReturnsAsync((Category c) => { c.CategoryId = 55; return c; });

            var created = await _service.CreateCategoryAsync(name, null);

            created.Should().NotBeNull();
            created.CategoryId.Should().Be(55);
            created.IsActive.Should().BeTrue();
            _categoryRepoMock.Verify(r => r.CreateAsync(It.Is<Category>(x => x.CategoryName == name)), Times.Once);
        }

        [Fact]
        public async Task UpdateCategoryAsync_ShouldUpdate_WhenExists()
        {
            var existing = new Category { CategoryId = 7, CategoryName = "Old", ParentCategoryId = null };
            _categoryRepoMock.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(existing);

            // Setup Update trả về chính object đó
            _categoryRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Category>()))
                .ReturnsAsync((Category c) => c);

            var updated = await _service.UpdateCategoryAsync(7, "NewName", 1);

            updated.Should().NotBeNull();
            updated!.CategoryName.Should().Be("NewName");
            updated.ParentCategoryId.Should().Be(1);
        }

        [Fact]
        public async Task DeleteCategoryAsync_ShouldReturnTrue_WhenSuccess()
        {
            // CategoryRepository của bạn có gọi DeleteAsync(id)
            _categoryRepoMock.Setup(r => r.DeleteAsync(5)).ReturnsAsync(true);

            var result = await _service.DeleteCategoryAsync(5);

            result.Should().BeTrue();
            _categoryRepoMock.Verify(r => r.DeleteAsync(5), Times.Once);
        }

        [Fact]
        public async Task SearchCategoriesAsync_ShouldReturnPagedResponse()
        {
            var page = new List<Category>
            {
                new Category { CategoryId = 1, CategoryName = "A" },
                new Category { CategoryId = 2, CategoryName = "B" }
            };

            // Setup dùng It.IsAny cho các tham số để tránh lỗi biên dịch Expression Tree
            _categoryRepoMock
                .Setup(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<bool?>(), 1, 10))
                .ReturnsAsync(((IEnumerable<Category>)page, 2));

            var request = new CategorySearchRequest
            {
                Name = "A",
                IsActive = true,
                PageNumber = 1,
                PageSize = 10
            };

            var paged = await _service.SearchCategoriesAsync(request);

            paged.TotalCount.Should().Be(2);
            paged.Data.Should().HaveCount(2);
            paged.TotalPages.Should().Be(1); // 2 / 10 = 0.2 -> Ceil = 1
        }
    }
}