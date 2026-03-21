using Domain.Entities;
using Domain.Interfaces;
using Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class UserOauthRepository : IUserOauthRepository
    {
        private readonly MacvilladbContext _context;

        public UserOauthRepository(MacvilladbContext context)
        {
            _context = context;
        }

        public async Task<UserOauth?> GetByProviderAndProviderId(string provider, string providerUserId)
        {
            return await _context.UserOauths
                .Include(x => x.User)
                .FirstOrDefaultAsync(x =>
                    x.Provider == provider &&
                    x.ProviderUserId == providerUserId);
        }

        public async Task CreateAsync(UserOauth oauth)
        {
            _context.UserOauths.Add(oauth);
            await _context.SaveChangesAsync();
        }
    }
}
