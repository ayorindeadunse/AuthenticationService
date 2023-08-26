using System;
using AuthenticationService.Data.Entities;

namespace AuthenticationService.Business.Services.Abstract
{
	public interface IJwtService
	{
		string GenerateJwtToken(User user); //Consider using a DTO instead to sign the token TODO
	}
}

