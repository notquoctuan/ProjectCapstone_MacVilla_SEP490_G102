using Application.DTOs;
using Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserService
    {
        // Chức năng View List với Search/Paging
        Task<PagedResponse<UserResponse>> GetUsersAsync(UserSearchRequest request);

        // Chức năng Add New Account
        Task<bool> CreateAccountAsync(CreateUserRequest request);

        // Repository trả về Entity User
        Task<PagedResponse<User>> GetUsersPagedAsync(UserSearchRequest request);
    }
}
