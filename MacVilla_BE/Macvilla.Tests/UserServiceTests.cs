using Application.DTOs;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
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
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _service = new UserService(_userRepoMock.Object);
        }

        #region TC15 & TC21: GetPagedUsersAsync (Mapping & Filtering)
        [Fact]
        public async Task GetPagedUsersAsync_ShouldReturnCorrectMapping_AndFilterByRole()
        {
            var usersList = new List<User>
    {
        new User { UserId = 1, Email = "admin@test.com", FullName = "Admin User", Role = "Admin", CreatedAt = DateTime.UtcNow },
        new User { UserId = 2, Email = "cust@test.com", FullName = "Customer User", Role = "Customer", CreatedAt = DateTime.UtcNow }
    };

            // SỬA TẠI ĐÂY: Gọi BuildMock trực tiếp trên List
            var mockQueryable = usersList.BuildMock();

            // Repository trả về mockQueryable này (nó đã bao gồm các Provider cho ToListAsync, CountAsync)
            _userRepoMock.Setup(r => r.GetQueryable()).Returns(mockQueryable);

            var request = new UserSearchRequest
            {
                Role = "Admin",
                PageNumber = 1,
                PageSize = 10,
                SortOrder = "latest"
            };

            // Act
            var result = await _service.GetPagedUsersAsync(request);

            // Assert
            result.Data.Should().HaveCount(1);
            result.Data.First().Role.Should().Be("Admin");
        }
        #endregion

        #region TC16 & TC17: CreateUserAsync (AddInternalUser)
        [Fact]
        public async Task CreateUserAsync_ShouldThrowException_WhenEmailExists()
        {
            // Arrange (TC16)
            _userRepoMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<long?>()))
                         .ReturnsAsync(true);

            var request = new CreateUserRequest { Email = "exist@test.com", FullName = "Test", Password = "123" };

            // Act
            Func<Task> act = async () => await _service.CreateUserAsync(request);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*đã được sử dụng*");
        }

        [Fact]
        public async Task CreateUserAsync_ShouldReturnResponse_WhenValid()
        {
            // Arrange (TC17)
            var request = new CreateUserRequest { Email = "new@test.com", FullName = "New User", Password = "Password123", Role = "Admin" };

            _userRepoMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<long?>())).ReturnsAsync(false);
            _userRepoMock.Setup(r => r.CreateUserAsync(It.IsAny<User>(), It.IsAny<string>()))
                         .ReturnsAsync((User u, string h) => u);

            // Act
            var result = await _service.CreateUserAsync(request);

            // Assert
            result.Email.Should().Be("new@test.com");
            _userRepoMock.Verify(r => r.CreateUserAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        }
        #endregion

        #region TC18, TC19, TC20: ChangeStatusAsync (ToggleStatus)
        [Fact]
        public async Task ChangeStatusAsync_ShouldThrowException_WhenIdInvalid()
        {
            // Arrange (TC18)
            var invalidId = -1L;
            var request = new ChangeUserStatusRequest { Status = "Disabled" };

            // Act
            Func<Task> act = async () => await _service.ChangeStatusAsync(invalidId, request);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task ChangeStatusAsync_ShouldUpdateSuccessfully_WhenUserExists()
        {
            // Arrange (TC19)
            var userId = 10L;
            var request = new ChangeUserStatusRequest { Status = "Disabled" };
            _userRepoMock.Setup(r => r.UpdateStatusAsync(userId, "Disabled")).ReturnsAsync(true);

            // Act
            await _service.ChangeStatusAsync(userId, request);

            // Assert
            _userRepoMock.Verify(r => r.UpdateStatusAsync(userId, "Disabled"), Times.Once);
        }

        [Fact]
        public async Task ChangeStatusAsync_ShouldThrowKeyNotFound_WhenUpdateReturnsFalse()
        {
            // Arrange (TC20)
            var userId = 99L;
            var request = new ChangeUserStatusRequest { Status = "Active" };
            _userRepoMock.Setup(r => r.UpdateStatusAsync(userId, It.IsAny<string>())).ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _service.ChangeStatusAsync(userId, request);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>();
        }
        #endregion

        #region Additional: DeleteUserAsync
        [Fact]
        public async Task DeleteUserAsync_ShouldThrowException_WhenUserHasOrders()
        {
            // Mock user có đơn hàng
            var user = new User { UserId = 1, Orders = new List<Order> { new Order() } };
            _userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

            Func<Task> act = async () => await _service.DeleteUserAsync(1);

            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*đã có đơn hàng*");
        }
        #endregion
    }
}