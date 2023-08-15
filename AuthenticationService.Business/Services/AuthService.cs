using System;
using AuthenticationService.Business.Models.DTOs;
using AuthenticationService.Data.Entities;
using AuthenticationService.Data.Repositories;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.Business.Services
{
	public class AuthService : IAuthService
	{
		private readonly IUserRepository _userRepository;
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;

		public AuthService(IUserRepository userRepository, UserManager<User> userManager, SignInManager<User> signInManager)
		{
			_userRepository = userRepository;
			_userManager = userManager;
			_signInManager = signInManager;
		}

		// refactor this to send a user token to user
		public async Task<bool> RegisterUserAsync(RegisterDTO registerDTO)
		{
			var user = new User { UserName = registerDTO.Username, Email = registerDTO.Email };
			var result = await _userManager.CreateAsync(user, registerDTO.Password);
			return result.Succeeded; // consider sending a user token of the DTO (without the password obvs, so refactor this to handle JWTs and what not)
		}

		// refactor this to send a user token to user
		public async Task<bool> LoginUserAsync(LoginDTO loginDTO)
		{
			var result = await _signInManager.PasswordSignInAsync(loginDTO.Username, loginDTO.Password, false, lockoutOnFailure: false);
			return result.Succeeded; // again, consider sending a bearer token as above and as a result refactor and reassess the current logic.
		}
	}
}

