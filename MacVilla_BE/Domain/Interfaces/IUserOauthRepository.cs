using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUserOauthRepository
    {
        Task<UserOauth?> GetByProviderAndProviderId(string provider, string providerUserId);
        Task CreateAsync(UserOauth oauth);
    }
}
