using System;
using AuthenticationService.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.Data.Repositories
{
	public interface IUserRepository // deprecate
	{
		Task<IdentityResult> CreateUserAsync(User user, string password);
		Task<User> GetUserByUsernameAsync(string username);
		User FindByExternalProviderAsync(string provider, string providerKey);
		// Register user
	}
}

