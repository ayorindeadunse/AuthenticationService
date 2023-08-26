using System;
using AuthenticationService.Data.Entities;

namespace AuthenticationService.Data.Repositories
{
	public interface IUserRepository
	{
		Task CreateUserAsync(User user, string password);
		Task<User> GetUserByUsernameAsync(string username);
		User FindByExternalProviderAsync(string provider, string providerKey);
		// Register user
	}
}

