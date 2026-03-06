using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Persistence.Context;

namespace Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<MacvilladbContext>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<MacvilladbContext>>();

        var email = config["SeedAdmin:Email"];
        var password = config["SeedAdmin:Password"];
        var fullName = config["SeedAdmin:FullName"];
        var role = config["SeedAdmin:Role"] ?? "Admin";

        // Không seed nếu thiếu config (tránh crash khi env var chưa set)
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("[Seeder] SeedAdmin config không đầy đủ — bỏ qua seed. " +
                              "Hãy set env var SEEDADMIN__EMAIL và SEEDADMIN__PASSWORD.");
            return;
        }

        var normalizedEmail = email.Trim().ToLower();

        var exists = await ctx.Users.AnyAsync(u => u.Email == normalizedEmail);
        if (exists)
        {
            logger.LogInformation("[Seeder] Tài khoản {Email} đã tồn tại — bỏ qua.", normalizedEmail);
            return;
        }

        var user = new User
        {
            Email = normalizedEmail,
            FullName = fullName?.Trim() ?? "Admin",
            Role = role,
            Status = "Active",
            CreatedAt = DateTime.UtcNow
        };
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        ctx.UserCredentials.Add(new UserCredential
        {
            UserId = user.UserId,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            CreatedAt = DateTime.UtcNow
        });
        await ctx.SaveChangesAsync();

        // KHÔNG log password ra console dù là development
        logger.LogInformation("[Seeder] ✅ Đã tạo tài khoản Admin: {Email}", normalizedEmail);
    }
}