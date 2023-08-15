using System;
using AuthenticationService.Business.Models.DTOs;

namespace AuthenticationService.Business.Services
{
	public interface IAuthService
	{
		Task<bool> RegisterUserAsync(RegisterDTO registerDTO);
		Task<bool> LoginUserAsync(LoginDTO loginDTO);
	}
}

