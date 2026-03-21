using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MacVilla.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly AuthService _authService;
        private readonly Mock<IUserOauthRepository> _userOauthRepoMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly IMemoryCache _memoryCache;

        public AuthServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _tokenServiceMock = new Mock<ITokenService>();
            _userOauthRepoMock = new Mock<IUserOauthRepository>();
            _emailServiceMock = new Mock<IEmailService>();

            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            _authService = new AuthService(
                _userRepoMock.Object,
                _tokenServiceMock.Object,
                _memoryCache,
                _userOauthRepoMock.Object,
                _emailServiceMock.Object
            );
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnResponse_WhenCredentialsAreValid()
        {
            // Arrange
            var password = "CorrectPassword123";
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            var email = "test@example.com";

            var user = new User
            {
                UserId = 1,
                Email = email,
                Status = "Active",
                FullName = "Test User",
                Role = "Admin"
            };

            var credential = new UserCredential { UserId = 1, PasswordHash = passwordHash };

            _userRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.GetCredentialAsync(user.UserId)).ReturnsAsync(credential);
            _tokenServiceMock.Setup(s => s.GenerateToken(user)).Returns("valid-jwt-token");

            var request = new LoginRequest { Email = email, Password = password };

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.AccessToken.Should().Be("valid-jwt-token");
            result.Email.Should().Be(email);
            result.Role.Should().Be("Admin");
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowUnauthorized_WhenEmailNotFound()
        {
            // Arrange
            _userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            var request = new LoginRequest { Email = "wrong@example.com", Password = "any" };

            // Act
            Func<Task> act = async () => await _authService.LoginAsync(request);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Email hoặc mật khẩu không đúng.");
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowUnauthorized_WhenAccountIsDisabled()
        {
            // Arrange
            var user = new User { UserId = 1, Email = "test@example.com", Status = "Disabled" };
            _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

            var request = new LoginRequest { Email = user.Email, Password = "any" };

            // Act
            Func<Task> act = async () => await _authService.LoginAsync(request);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("*vô hiệu hóa*");
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowUnauthorized_WhenPasswordIsIncorrect()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User { UserId = 1, Email = email, Status = "Active" };
            var credential = new UserCredential { UserId = 1, PasswordHash = BCrypt.Net.BCrypt.HashPassword("RealPassword") };

            _userRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.GetCredentialAsync(user.UserId)).ReturnsAsync(credential);

            var request = new LoginRequest { Email = email, Password = "WrongPassword" };

            // Act
            Func<Task> act = async () => await _authService.LoginAsync(request);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Email hoặc mật khẩu không đúng.");
        }

        [Fact]
        public async Task CreateAdminAsync_ShouldReturnResponse_WhenEmailIsNew()
        {
            // Arrange
            var request = new CreateAdminRequest
            {
                Email = "newadmin@example.com",
                FullName = "Admin",
                Password = "password",
                Role = "Admin"
            };

            _userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            _userRepoMock.Setup(r => r.CreateUserAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync((User u, string p) => { u.UserId = 99; return u; });
            _tokenServiceMock.Setup(s => s.GenerateToken(It.IsAny<User>())).Returns("new-token");

            // Act
            var result = await _authService.CreateAdminAsync(request);

            // Assert
            result.UserId.Should().Be(99);
            result.AccessToken.Should().Be("new-token");
            _userRepoMock.Verify(r => r.CreateUserAsync(It.Is<User>(u => u.Email == request.Email.ToLower()), It.IsAny<string>()), Times.Once);
        }

        //Case 1: Send OTP thành công
        [Fact]
        public async Task SendOtpAsync_ShouldSendEmail_WhenEmailExists()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User { UserId = 1, Email = email };

            _userRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

            // Act
            await _authService.SendOtpAsync(email);

            // Assert
            _emailServiceMock.Verify(
                x => x.SendOtpEmailAsync(email, It.IsAny<string>()),
                Times.Once
            );
        }

        //Case 2: Email không tồn tại
        [Fact]
        public async Task SendOtpAsync_ShouldThrowException_WhenEmailNotFound()
        {
            // Arrange
            var email = "notfound@example.com";

            _userRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync((User?)null);

            // Act
            Func<Task> act = async () => await _authService.SendOtpAsync(email);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Email chưa đăng ký");
        }

        //Case 3: Resend trước 60s (cooldown)
        [Fact]
        public async Task SendOtpAsync_ShouldThrowException_WhenCooldownNotExpired()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User { UserId = 1, Email = email };

            _userRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

            // Set cooldown trước
            _memoryCache.Set($"otp_cd_{email}", true, TimeSpan.FromSeconds(60));

            // Act
            Func<Task> act = async () => await _authService.SendOtpAsync(email);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Vui lòng đợi 60 giây trước khi yêu cầu mã mới.");
        }

        //Case 4: Resend OTP (gọi lại SendOtp)
        [Fact]
        public async Task ResendOtpAsync_ShouldCallSendOtp()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User { UserId = 1, Email = email };

            _userRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

            // Act
            await _authService.ResendOtpAsync(email);

            // Assert
            _emailServiceMock.Verify(
                x => x.SendOtpEmailAsync(email, It.IsAny<string>()),
                Times.Once
            );
        }
    }
}