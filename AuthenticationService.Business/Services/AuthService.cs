using System;
using AuthenticationService.Business.Models;
using AuthenticationService.Business.Models.DTOs;
using AuthenticationService.Business.Services.Abstract;
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
		private readonly IJwtService _jwtService;

		public AuthService(IUserRepository userRepository, UserManager<User> userManager, SignInManager<User> signInManager,
			IJwtService jwtService)
		{
			_userRepository = userRepository;
			_userManager = userManager;
			_signInManager = signInManager;
			_jwtService = jwtService;
		}

		// refactor this to send a user token to user
		public async Task<ResponseWrapper<string>> RegisterUserAsync(RegisterDTO registerDTO) //TODO: Consider adding Social login registration as well
		{
			try
			{
				var user = new User { UserName = registerDTO.Username, Email = registerDTO.Email };
				var result = await _userRepository.CreateUserAsync(user, registerDTO.Password);
				// generate user token and send in body of response

				if (result.Succeeded)
				{
					var token = GenerateJwtToken(user);
					return new ResponseWrapper<string>
					{
						IsRequestSuccessful = true,
						Data = token
					};
				}

                return new ResponseWrapper<string>
                {
                    IsRequestSuccessful = false,
                    Errors = new[]
                    {
                        "Registration failed"
                    }
                };
            }
			catch(Exception ex)
			{
				// log exception
				return new ResponseWrapper<string>
				{
					IsRequestSuccessful = false,
					Errors = new[]
					{
						"Registration failed"
					}
				};
			}
			//return result.Succeeded; // consider sending a user token of the DTO (without the password obvs, so refactor this to handle JWTs and what not)

		}

		// refactor this to send a user token to user
		public async Task<ResponseWrapper<string>> LoginUserAsync(LoginDTO loginDTO)
		{
			var result = await _signInManager.PasswordSignInAsync(loginDTO.Username, loginDTO.Password, false, lockoutOnFailure: false);
			if (result.Succeeded)
			{
				var user = await _userManager.FindByNameAsync(loginDTO.Username);
				// Generate token
				var token = GenerateJwtToken(user);
				return new ResponseWrapper<string>
				{
					IsRequestSuccessful = true,
					Data = token
				};
			}

			return new ResponseWrapper<string>
			{
				IsRequestSuccessful = false,
				Errors = new[]
				{
					"Login failed"
				}
			};
		}

        public ResponseWrapper<string> SocialLoginAsync(string provider, string providerToken)
        {
			var externalLoginInfo = _signInManager.GetExternalLoginInfoAsync();
			if (externalLoginInfo == null)
			{
				return new ResponseWrapper<string>
				{
					IsRequestSuccessful = false,
					Errors = new[]
					{
						"External login failed."
					}
				};
			}

			var user = _userRepository.FindByExternalProviderAsync(provider, providerToken);
			if (user != null)
			{
				// sign in with that user
				_signInManager.SignInAsync(user, isPersistent: false);

				// Generate token
				var token = GenerateJwtToken(user);
				return new ResponseWrapper<string>
				{
					IsRequestSuccessful = true,
					Data = token
				};
			}
                // Send error message that registration is required or to contact support.

                return new ResponseWrapper<string>
				{
					IsRequestSuccessful = false,
					Errors = new[]
					{ "User not found. Please register or contact support."

					}			
				};
            
        }


        private string GenerateJwtToken(User user)
        {
            // Generate JWT token using _jwtService
            var token = _jwtService.GenerateJwtToken(user);
            return token;
        }

    }
}

