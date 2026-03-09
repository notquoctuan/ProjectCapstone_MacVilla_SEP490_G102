using Application.Services;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace MacVilla.Tests
{
    public class TokenServiceTests
    {
        private readonly IConfiguration _config;
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            // Sử dụng helper TestConfig bạn đã viết để load cấu hình giả lập
            _config = TestConfig.InitConfiguration();
            _tokenService = new TokenService(_config);
        }

        [Fact]
        public void GenerateToken_ShouldReturnValidJwt_WithCorrectClaims()
        {
            // Arrange
            var user = new User
            {
                UserId = 123,
                Email = "dev@macvilla.vn",
                FullName = "Gemini Developer",
                Role = "Admin"
            };

            // Act
            var tokenString = _tokenService.GenerateToken(user);

            // Assert
            tokenString.Should().NotBeNullOrEmpty();

            // Giải mã token để kiểm tra nội dung bên trong
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(tokenString);

            // 1. Kiểm tra Issuer và Audience
            jwtToken.Issuer.Should().Be(_config["Jwt:Issuer"]);
            jwtToken.Audiences.Should().Contain(_config["Jwt:Audience"]);

            // 2. Kiểm tra các Claims quan trọng
            jwtToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value.Should().Be("123");
            jwtToken.Claims.First(c => c.Type == ClaimTypes.Name).Value.Should().Be(user.Email);
            jwtToken.Claims.First(c => c.Type == ClaimTypes.Role).Value.Should().Be("Admin");
            jwtToken.Claims.First(c => c.Type == "full_name").Value.Should().Be(user.FullName);

            // 3. Kiểm tra thời gian hết hạn (ExpireHours)
            // Mặc định trong TestConfig có thể chưa có ExpireHours, TokenService sẽ dùng 24h
            var expireHours = double.Parse(_config["Jwt:ExpireHours"] ?? "24");
            jwtToken.ValidTo.Should().BeAfter(DateTime.UtcNow.AddHours(expireHours - 0.5));
            jwtToken.ValidTo.Should().BeBefore(DateTime.UtcNow.AddHours(expireHours + 0.5));
        }

        [Fact]
        public void GenerateToken_ShouldAssignDefaultRole_WhenUserRoleIsNull()
        {
            // Arrange
            var user = new User
            {
                UserId = 1,
                Email = "customer@gmail.com",
                Role = null // Role null
            };

            // Act
            var tokenString = _tokenService.GenerateToken(user);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(tokenString);

            // Kiểm tra fallback về "Customer"
            jwtToken.Claims.First(c => c.Type == ClaimTypes.Role).Value.Should().Be("Customer");
        }

        [Fact]
        public void GenerateToken_ShouldBeVerifiable_WithSecretKey()
        {
            // Arrange
            var user = new User { UserId = 1, Email = "test@test.com" };
            var tokenString = _tokenService.GenerateToken(user);
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _config["Jwt:Audience"],
                ClockSkew = TimeSpan.Zero
            };

            // Act & Assert
            var handler = new JwtSecurityTokenHandler();
            // Nếu token không hợp lệ (sai key/issuer/...), phương thức này sẽ quăng Exception
            var principal = handler.ValidateToken(tokenString, validationParameters, out var validatedToken);

            principal.Should().NotBeNull();
            validatedToken.Should().NotBeNull();
        }
    }
}