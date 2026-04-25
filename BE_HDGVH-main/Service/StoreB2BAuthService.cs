using BE_API.Domain;
using BE_API.Dto.Store;
using BE_API.Entities;
using BE_API.ExceptionHandling;
using BE_API.Repository;
using BE_API.Service.IService;
using Microsoft.EntityFrameworkCore;

namespace BE_API.Service;

public class StoreB2BAuthService(IRepository<Customer> customerRepo, IJwtTokenService jwtTokenService) : IStoreB2BAuthService
{
    public async Task<StoreB2BLoginResponseDto> RegisterAsync(
        StoreB2BRegisterDto dto,
        CancellationToken cancellationToken = default)
    {
        var phone = dto.Phone.Trim();
        if (phone.Length == 0)
            throw new ArgumentException("Số điện thoại không hợp lệ.");

        var email = dto.Email.Trim();
        if (email.Length == 0)
            throw new ArgumentException("Email không hợp lệ.");

        var companyName = dto.CompanyName.Trim();
        if (companyName.Length == 0)
            throw new ArgumentException("Tên công ty là bắt buộc.");

        if (await customerRepo.Get().AsNoTracking().AnyAsync(c => c.Phone.Trim() == phone, cancellationToken))
            throw new InvalidOperationException("Số điện thoại đã được đăng ký.");

        if (await customerRepo.Get().AsNoTracking()
                .AnyAsync(c => c.Email != null && c.Email.ToLower() == email.ToLowerInvariant(), cancellationToken))
            throw new InvalidOperationException("Email đã được đăng ký.");

        var entity = new Customer
        {
            CustomerType = CustomerTypes.B2B,
            FullName = dto.FullName.Trim(),
            Email = email.ToLowerInvariant(),
            Phone = phone,
            // PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            PasswordHash = dto.Password,
            CompanyName = companyName,
            TaxCode = NormalizeNullable(dto.TaxCode),
            CompanyAddress = NormalizeNullable(dto.CompanyAddress),
            DebtBalance = 0,
            CreatedAt = DateTime.UtcNow
        };

        await customerRepo.AddAsync(entity, cancellationToken);
        await customerRepo.SaveChangesAsync(cancellationToken);

        return BuildLoginResponse(entity);
    }

    public async Task<StoreB2BLoginResponseDto> LoginAsync(
        StoreB2BLoginDto dto,
        CancellationToken cancellationToken = default)
    {
        var email = dto.Email.Trim();
        if (email.Length == 0 || string.IsNullOrEmpty(dto.Password))
            throw new AuthenticationFailedException();

        var emailLower = email.ToLowerInvariant();

        var customer = await customerRepo.Get()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                c => c.CustomerType == CustomerTypes.B2B
                     && c.PasswordHash != null
                     && c.PasswordHash.Length > 0
                     && c.Email != null
                     && c.Email.ToLower() == emailLower,
                cancellationToken);
        //Tạm comment đổi thành == vì đang test trên local
        // if (customer is null || !BCrypt.Net.BCrypt.Verify(dto.Password, customer.PasswordHash))
        if (customer is null || dto.Password != customer.PasswordHash)
            throw new AuthenticationFailedException();

        return BuildLoginResponse(customer);
    }

    public async Task<StoreB2BProfileDto> GetProfileAsync(int customerId, CancellationToken cancellationToken = default)
    {
        var c = await customerRepo.Get()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == customerId && x.CustomerType == CustomerTypes.B2B, cancellationToken)
            ?? throw new KeyNotFoundException("Không tìm thấy tài khoản doanh nghiệp");

        return MapProfile(c);
    }

    public async Task<StoreB2BProfileDto> UpdateProfileAsync(
        int customerId,
        StoreB2BUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        var entity = await customerRepo.Get()
            .FirstOrDefaultAsync(x => x.Id == customerId && x.CustomerType == CustomerTypes.B2B, cancellationToken)
            ?? throw new KeyNotFoundException("Không tìm thấy tài khoản doanh nghiệp");

        var phone = dto.Phone.Trim();
        if (phone.Length == 0)
            throw new ArgumentException("Số điện thoại không hợp lệ.");

        var email = dto.Email.Trim();
        if (email.Length == 0)
            throw new ArgumentException("Email không hợp lệ.");

        var companyName = dto.CompanyName.Trim();
        if (companyName.Length == 0)
            throw new ArgumentException("Tên công ty là bắt buộc.");

        if (await customerRepo.Get().AsNoTracking()
                .AnyAsync(c => c.Id != customerId && c.Phone.Trim() == phone, cancellationToken))
            throw new InvalidOperationException("Số điện thoại đã được sử dụng.");

        if (await customerRepo.Get().AsNoTracking()
                .AnyAsync(c => c.Id != customerId && c.Email != null && c.Email.ToLower() == email.ToLowerInvariant(),
                    cancellationToken))
            throw new InvalidOperationException("Email đã được sử dụng.");

        entity.FullName = dto.FullName.Trim();
        entity.Email = email.ToLowerInvariant();
        entity.Phone = phone;
        entity.CompanyName = companyName;
        entity.TaxCode = NormalizeNullable(dto.TaxCode);
        entity.CompanyAddress = NormalizeNullable(dto.CompanyAddress);

        customerRepo.Update(entity);
        await customerRepo.SaveChangesAsync(cancellationToken);

        return MapProfile(entity);
    }

    private StoreB2BLoginResponseDto BuildLoginResponse(Customer c)
    {
        var (token, expires) = jwtTokenService.CreateCustomerAccessToken(c);
        return new StoreB2BLoginResponseDto
        {
            AccessToken = token,
            ExpiresAtUtc = expires,
            Customer = MapProfile(c)
        };
    }

    private static StoreB2BProfileDto MapProfile(Customer c) =>
        new()
        {
            Id = c.Id,
            CustomerType = c.CustomerType,
            FullName = c.FullName,
            Email = c.Email,
            Phone = c.Phone,
            CompanyName = c.CompanyName ?? string.Empty,
            TaxCode = c.TaxCode,
            CompanyAddress = c.CompanyAddress,
            DebtBalance = c.DebtBalance
        };

    private static string? NormalizeNullable(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
