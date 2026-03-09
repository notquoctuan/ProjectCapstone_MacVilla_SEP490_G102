using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Xunit;

namespace MacVilla.Tests
{
    public class UserServiceTests
    {
        #region EF Core Async Helpers
        internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;
            public TestAsyncQueryProvider(IQueryProvider inner) => _inner = inner;

            public IQueryable CreateQuery(Expression expression)
                => new TestAsyncEnumerable<TEntity>(expression);

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
                => new TestAsyncEnumerable<TElement>(expression);

            public object? Execute(Expression expression) => _inner.Execute(expression);

            public TResult Execute<TResult>(Expression expression)
                => _inner.Execute<TResult>(expression);

            // Fix: EF Core CountAsync/ToListAsync gọi ExecuteAsync với TResult = Task<int>/Task<List<T>>
            // phải wrap kết quả sync vào Task.FromResult
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

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
                => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

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

        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _service = new UserService(_userRepoMock.Object);
        }

        [Fact]
        public async Task GetPagedUsersAsync_ShouldReturnPagedData_WhenKeywordMatches()
        {
            // Fix: dùng TestAsyncEnumerable thay vì .AsQueryable() để hỗ trợ EF Core CountAsync
            var users = new TestAsyncEnumerable<User>(new List<User>
            {
                new User { UserId = 1, Email = "admin@macvilla.vn", FullName = "Nguyễn Admin" }
            });

            _userRepoMock.Setup(r => r.GetQueryable()).Returns(users);

            var request = new UserSearchRequest { Keyword = "Admin", PageNumber = 1, PageSize = 10 };
            var result = await _service.GetPagedUsersAsync(request);

            result.Data.Should().HaveCount(1);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldThrowException_WhenEmailAlreadyExists()
        {
            _userRepoMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<long?>()))
                         .ReturnsAsync(true);

            var request = new CreateUserRequest { Email = "test@mail.com", FullName = "Test", Password = "123" };

            Func<Task> act = async () => await _service.CreateUserAsync(request);
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task CreateUserAsync_ShouldReturnMappedResponse_WhenValid()
        {
            var request = new CreateUserRequest { Email = "new@macvilla.vn", FullName = "New User", Password = "password123" };
            var createdUser = new User { UserId = 50, Email = request.Email };

            _userRepoMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<long?>()))
                         .ReturnsAsync(false);

            _userRepoMock.Setup(r => r.CreateUserAsync(It.IsAny<User>(), It.IsAny<string>()))
                         .ReturnsAsync(createdUser);

            var result = await _service.CreateUserAsync(request);

            result.UserId.Should().Be(50);
        }

        [Fact]
        public async Task ResetPasswordAsync_ShouldCallRepo()
        {
            var userId = 1L;
            var user = new User { UserId = userId };
            var request = new ResetPasswordRequest { NewPassword = "NewPassword123" };

            _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            await _service.ResetPasswordAsync(userId, request);

            _userRepoMock.Verify(r => r.ResetPasswordAsync(userId, It.IsAny<string>()), Times.Once);
        }
    }
}