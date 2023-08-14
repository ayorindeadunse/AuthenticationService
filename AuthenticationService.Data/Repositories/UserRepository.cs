using System;
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

        public async Task CreateUserAsync(User user, string password)
        {
            //might need to pass a DTO and not the full user object...
            await _userManager.CreateAsync(user, password);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username); // review
        }
    }
}

