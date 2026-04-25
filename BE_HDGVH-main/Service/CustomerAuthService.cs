using BE_API.Domain;
using BE_API.Dto.Store;
using BE_API.Entities;
using BE_API.ExceptionHandling;
using BE_API.Repository;
using BE_API.Service.IService;
using Microsoft.EntityFrameworkCore;

namespace BE_API.Service;

public class CustomerAuthService(IRepository<Customer> customerRepo, IJwtTokenService jwtTokenService) : ICustomerAuthService
{
    public async Task<StoreCustomerLoginResponseDto> RegisterAsync(
        StoreCustomerRegisterDto dto,
        CancellationToken cancellationToken = default)
    {
        var phone = dto.Phone.Trim();
        if (phone.Length == 0)
            throw new ArgumentException("Số điện thoại không hợp lệ.");

        var email = dto.Email.Trim();
        if (email.Length == 0)
            throw new ArgumentException("Email không hợp lệ.");

        if (await customerRepo.Get().AsNoTracking().AnyAsync(c => c.Phone.Trim() == phone, cancellationToken))
            throw new InvalidOperationException("Số điện thoại đã được đăng ký.");

        if (await customerRepo.Get().AsNoTracking()
                .AnyAsync(c => c.Email != null && c.Email.ToLower() == email.ToLowerInvariant(), cancellationToken))
            throw new InvalidOperationException("Email đã được đăng ký.");

        var entity = new Customer
        {
            CustomerType = CustomerTypes.B2C,
            FullName = dto.FullName.Trim(),
            Email = email.ToLowerInvariant(),
            Phone = phone,
            // PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            PasswordHash = dto.Password,
            DebtBalance = 0,
            CreatedAt = DateTime.UtcNow
        };

        await customerRepo.AddAsync(entity, cancellationToken);
        await customerRepo.SaveChangesAsync(cancellationToken);

        return BuildLoginResponse(entity);
    }

    public async Task<StoreCustomerLoginResponseDto> LoginAsync(
        StoreCustomerLoginDto dto,
        CancellationToken cancellationToken = default)
    {
        var email = dto.Email.Trim();
        if (email.Length == 0 || string.IsNullOrEmpty(dto.Password))
            throw new AuthenticationFailedException();

        var emailLower = email.ToLowerInvariant();

        var customer = await customerRepo.Get()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                c => c.CustomerType == CustomerTypes.B2C
                     && c.PasswordHash != null
                     && c.PasswordHash.Length > 0
                     && c.Email != null
                     && c.Email.ToLower() == emailLower,
                cancellationToken);
        //Tạm comment đổi thành  == vì đang test trên local
        // if (customer is null || !BCrypt.Net.BCrypt.Verify(dto.Password, customer.PasswordHash))
        if (customer is null || dto.Password != customer.PasswordHash)
        // if (customer is null || !BCrypt.Net.BCrypt.Verify(dto.Password, customer.PasswordHash))
            throw new AuthenticationFailedException();

        return BuildLoginResponse(customer);
    }

    public async Task<StoreCustomerProfileDto> GetProfileAsync(int customerId, CancellationToken cancellationToken = default)
    {
        var c = await customerRepo.Get()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == customerId && x.CustomerType == CustomerTypes.B2C, cancellationToken)
            ?? throw new KeyNotFoundException("Không tìm thấy tài khoản");

        return MapProfile(c);
    }

    public async Task<StoreCustomerProfileDto> UpdateProfileAsync(
        int customerId,
        StoreCustomerUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        var entity = await customerRepo.Get()
            .FirstOrDefaultAsync(x => x.Id == customerId && x.CustomerType == CustomerTypes.B2C, cancellationToken)
            ?? throw new KeyNotFoundException("Không tìm thấy tài khoản");

        var phone = dto.Phone.Trim();
        if (phone.Length == 0)
            throw new ArgumentException("Số điện thoại không hợp lệ.");

        var email = dto.Email.Trim();
        if (email.Length == 0)
            throw new ArgumentException("Email không hợp lệ.");

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

        customerRepo.Update(entity);
        await customerRepo.SaveChangesAsync(cancellationToken);

        return MapProfile(entity);
    }

    public async Task ChangePasswordAsync(
        int customerId,
        StoreCustomerChangePasswordDto dto,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.OldPassword) || string.IsNullOrWhiteSpace(dto.NewPassword))
            throw new ArgumentException("Vui lòng nhập mật khẩu hiện tại và mật khẩu mới.");

        if (dto.NewPassword.Length < 6)
            throw new ArgumentException("Mật khẩu mới tối thiểu 6 ký tự.");

        if (string.Equals(dto.OldPassword, dto.NewPassword, StringComparison.Ordinal))
            throw new InvalidOperationException("Mật khẩu mới phải khác mật khẩu hiện tại.");

        var entity = await customerRepo.Get()
            .FirstOrDefaultAsync(x => x.Id == customerId, cancellationToken)
            ?? throw new KeyNotFoundException("Không tìm thấy tài khoản");

        if (string.IsNullOrEmpty(entity.PasswordHash) || entity.PasswordHash != dto.OldPassword)
        {
            throw new AuthenticationFailedException();
        }

        entity.PasswordHash = dto.NewPassword;
        customerRepo.Update(entity);
        await customerRepo.SaveChangesAsync(cancellationToken);
    }

    private StoreCustomerLoginResponseDto BuildLoginResponse(Customer c)
    {
        var (token, expires) = jwtTokenService.CreateCustomerAccessToken(c);
        return new StoreCustomerLoginResponseDto
        {
            AccessToken = token,
            ExpiresAtUtc = expires,
            Customer = MapProfile(c)
        };
    }

    private static StoreCustomerProfileDto MapProfile(Customer c) =>
        new()
        {
            Id = c.Id,
            CustomerType = c.CustomerType,
            FullName = c.FullName,
            Email = c.Email,
            Phone = c.Phone
        };
}
