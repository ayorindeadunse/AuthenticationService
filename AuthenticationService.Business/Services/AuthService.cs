using System;
using System.Security.Claims;
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
		private readonly SignInManager<User> _signInManager;
		private readonly IJwtService _jwtService;

		public AuthService(IUserRepository userRepository,SignInManager<User> signInManager,
			IJwtService jwtService)
		{
			_userRepository = userRepository;
			_signInManager = signInManager;
			_jwtService = jwtService;
		}

		// refactor this to send a user token to user
		public async Task<ResponseWrapper<string>> SocialRegisterAsync(string provider, string providerToken)
		{
			try
			{
				var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
				if (externalLoginInfo != null)
				{
					//use this bit to debug the claims that come from the external login provider (twitter, facebook, etc)
					// put a breakpoint here to get the values
					var claims = externalLoginInfo.Principal.Claims.ToList();
					foreach (var claim in claims)
					{
						// Use logger object instead
						Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
					}
				}
				
				// retrieve possible values of  username and email.
				var newUserData = new
				{
					Username = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Name),
					Email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email)
				};

				var generatedPassword = GenerateSecurePassword();
				var newUser = new User
				{
					UserName = newUserData.Username,
					Email = newUserData.Email,
					// set other properties after refactoring
				};
				
				//Perform social registration logic...
				var result = await _userRepository.CreateUserAsync(newUser, generatedPassword);
				if (result.Succeeded)
				{
					await _signInManager.SignInAsync(newUser, isPersistent: false);
					// Generate token
					var token = _jwtService.GenerateJwtToken(newUser);
					return new ResponseWrapper<string>
					{
						IsRequestSuccessful = true,
						Data = token
					};
				}

				return new ResponseWrapper<string>
				{
					// Registration failed
					IsRequestSuccessful = false,
					Errors = result.Errors.Select(e => e.Description).ToArray()
				};
			}
			catch (Exception e)
			{
				//Log exception 
				return new ResponseWrapper<string>
				{
					IsRequestSuccessful = false,
					Errors = new[] { "Registration failed using your social login credentials. "}
				};
			}
		}
		
		public async Task<ResponseWrapper<string>> RegisterUserAsync(RegisterDTO registerDTO) 
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
				//var user = await _userManager.FindByNameAsync(loginDTO.Username);
				var user = await _userRepository.GetUserByUsernameAsync(loginDTO.Username);
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

        private static string GenerateSecurePassword()
        {
	        return Guid.NewGuid().ToString();
        }
    }
}

