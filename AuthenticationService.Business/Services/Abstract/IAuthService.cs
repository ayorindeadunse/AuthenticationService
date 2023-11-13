using System;
using AuthenticationService.Business.Models;
using AuthenticationService.Business.Models.DTOs;

namespace AuthenticationService.Business.Services
{
	public interface IAuthService
	{
		Task<ResponseWrapper<string>> RegisterUserAsync(RegisterDTO registerDTO);
		Task<ResponseWrapper<string>> LoginUserAsync(LoginDTO loginDTO);
		ResponseWrapper<string> SocialLoginAsync(string provider, string providerToken);
	}
}

