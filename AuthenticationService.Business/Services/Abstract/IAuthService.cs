using System;
using AuthenticationService.Business.Models;
using AuthenticationService.Business.Models.DTOs;
using FluentResults;

namespace AuthenticationService.Business.Services
{
	public interface IAuthService
	{
		Task<ResponseWrapper<string>> RegisterUserAsync(RegisterDTO registerDTO);
		Task<ResponseWrapper<string>> LoginUserAsync(LoginDTO loginDTO);
		
		// updated abstract methods for new Register and Login implementation
		Task<string> Register(RegisterDTO request);
		Task<string> Login(LoginDTO request);
		
		// different Social Login Version
		Task<Result<string>> SocialLogin(SocialLoginRequest request);
	}
}

