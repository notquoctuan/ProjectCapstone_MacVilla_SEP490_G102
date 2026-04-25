using BE_API.Entities;
using Microsoft.EntityFrameworkCore;

namespace BE_API.Domain;

public static class StoreOrderCodes
{
    /// <summary>Sinh mã đơn unique (tiền tố B2C + ngày UTC + hậu tố ngẫu nhiên).</summary>
    public static async Task<string> GenerateUniqueAsync(
        IQueryable<CustomerOrder> orders,
        CancellationToken cancellationToken = default)
    {
        for (var attempt = 0; attempt < 30; attempt++)
        {
            var suffix = Random.Shared.Next(100_000, 999_999);
            var code = $"B2C{DateTime.UtcNow:yyyyMMdd}{suffix}";
            var exists = await orders.AsNoTracking().AnyAsync(o => o.OrderCode == code, cancellationToken);
            if (!exists)
                return code;
        }

        throw new InvalidOperationException("Không sinh được mã đơn hàng.");
    }
}
