using System;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.Data.Entities
{
	public class User : IdentityUser
	{
		// add other nullable properties(or not) of properties other services might require when integrating with the auth service.
		// Add some documentation after deploying this service so your users know what properties they can get from this implementation
		// or other properties that may or may not be needed in other services.

		public string? ExternalLoginProvider { get; set; } = null;
		public string? ExternalLoginProviderKey { get; set; }
	}
}

