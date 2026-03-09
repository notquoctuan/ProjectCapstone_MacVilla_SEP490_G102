using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace MacVilla.Tests
{
    public class TestConfig
    {
        public static IConfiguration InitConfiguration()
        {
            var builder = new ConfigurationBuilder()
                // Ưu tiên đọc từ file appsettings.json thực tế nếu có trong thư mục output của test
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);

            // Fallback: Cung cấp các giá trị mặc định khớp với appsettings.json bạn vừa gửi
            // để đảm bảo các Unit Test luôn chạy được trên môi trường CI/CD
            var inMemory = new Dictionary<string, string?>
            {
                ["SeedAdmin:Email"] = "admin@macvilla.vn",
                ["SeedAdmin:Password"] = "Admin@123",
                ["SeedAdmin:FullName"] = "Nguyễn Quản Trị",
                ["SeedAdmin:Role"] = "Admin",

                ["Jwt:Key"] = "MacVilla@SuperSecretKey2024!MinLength32Chars",
                ["Jwt:Issuer"] = "MacVillaAPI",
                ["Jwt:Audience"] = "MacVillaClient",
                ["Jwt:ExpireHours"] = "24"
            };

            builder.AddInMemoryCollection(inMemory);

            return builder.Build();
        }
    }
}