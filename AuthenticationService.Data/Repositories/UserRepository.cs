using System;
using AuthenticationService.Data.Context;
using AuthenticationService.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.Data.Repositories
{
    public class UserRepository : IUserRepository //deprecate
    {
        private readonly UserManager<User> _userManager;
        // inject db context
        private readonly AuthDbContext _context;

        public UserRepository(UserManager<User> userManager,AuthDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IdentityResult> CreateUserAsync(User user, string password)
        {
          //  return await _userManager.CreateAsync(user, password);
          
          var result = await _userManager.CreateAsync(user, password);
          return result;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username); // review
        }

        public User FindByExternalProviderAsync(string provider, string providerKey)
        {
            return _userManager.Users.FirstOrDefault(u => u.ExternalLoginProvider == provider && u.ExternalLoginProviderKey == providerKey);
        }
    }
}

