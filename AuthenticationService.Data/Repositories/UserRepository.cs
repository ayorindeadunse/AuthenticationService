﻿using System;
using AuthenticationService.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;

        public UserRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> CreateUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
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

