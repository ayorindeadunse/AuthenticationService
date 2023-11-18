using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthenticationService.Business.Consts;
using AuthenticationService.Business.Models;
using AuthenticationService.Business.Models.DTOs;
using AuthenticationService.Business.Services.Abstract;
using AuthenticationService.Data.Entities;
using AuthenticationService.Data.Repositories;
using FluentResults;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationService.Business.Services
{
	public class AuthService : IAuthService
	{
		private readonly IUserRepository _userRepository;
		private readonly SignInManager<User> _signInManager;
		private readonly IJwtService _jwtService;
		
		// inject UserManager and IConfiguration objects for updated Register and Login implementation
		private readonly UserManager<User> _userManager;
		private readonly IConfiguration _configuration;
		private readonly IHttpClientFactory _httpClientFactory;

		public AuthService(UserManager<User> userManager, IConfiguration configuration,IUserRepository userRepository,SignInManager<User> signInManager,
			IJwtService jwtService,IHttpClientFactory httpClientFactory)
		{
			_userRepository = userRepository;
			_signInManager = signInManager;
			_jwtService = jwtService;
			_userManager = userManager;
			_configuration = configuration;
			_httpClientFactory = httpClientFactory;
		}

		public async Task<Result<string>> SocialLogin(SocialLoginRequest request)
		{
			var tokenValidationResult = await ValidateSocialToken(request);
			if (tokenValidationResult.IsFailed)
			{
				return tokenValidationResult;
			}

			var user = await _userManager.FindByEmailAsync(request.Email);
			if (user is null)
			{
				var registerResult = await RegisterSocialUser(request);
				if (registerResult.IsFailed)
				{
					return registerResult.ToResult();
				}

				user = registerResult.Value;
			}

			if (user.ExternalLoginProvider != request.Provider)
			{
				return Result.Fail(
					$"$User was registered via {user.ExternalLoginProvider} and cannot be logged via {request.Provider}");
			}

			var token = GetToken(await GetClaims(user));
			return Result.Ok(new JwtSecurityTokenHandler().WriteToken(token));
		}

		private async Task<Result<User>> RegisterSocialUser(SocialLoginRequest request)
		{
			var user = new User()
			{
				Email = request.Email,
				UserName = request.Email,
				SecurityStamp = Guid.NewGuid().ToString(),
				ExternalLoginProvider = request.Provider
			};
			var result = await _userManager.CreateAsync(user,GenerateSecurePassword());
			if (!result.Succeeded)
			{
				return Result.Fail($"Unable to register user {request.Email}, errors: {GetErrorsText(result.Errors)}");
			}

			await _userManager.AddToRoleAsync(user, Role.User);
			return Result.Ok(user);
		}

		private async Task<List<Claim>> GetClaims(User user)
		{
			var authClaims = new List<Claim>
			{
				new(ClaimTypes.Name, user.Email!),
				new(ClaimTypes.Email, user.Email!),
				new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			};
			var userRoles = await _userManager.GetRolesAsync(user);

			if (userRoles is not null && userRoles.Any())
			{
				authClaims.AddRange((userRoles.Select(userRole => new Claim(ClaimTypes.Role,userRole))));
			}

			return authClaims;
		}
		//ValidateSocialToken
		private async Task<Result> ValidateSocialToken(SocialLoginRequest request)
		{
			//TODO: Remember to implement Facebook Social Token validation
			if (request.Provider == Constants.LoginProviders.Google)
			{
				return await ValidateGoogleToken(request);
			}

			return Result.Fail($"{request.Provider} provider is not supported.");
		}

		private async Task<Result> ValidateGoogleToken(SocialLoginRequest request)
		{
			try
			{
				var settings = new GoogleJsonWebSignature.ValidationSettings
				{
					Audience = new List<string> {_configuration["Google:TokenAudience"]}
				};
				await GoogleJsonWebSignature.ValidateAsync(request.ProviderToken, settings);
			}
			catch (InvalidJwtException _)
			{
				// log exception with logger
				return Result.Fail($"{request.Provider} access token is not valid");
			}

			return Result.Ok();
		}
		
		public async Task<string> Register(RegisterDTO request)
		{
			var userByEmail = await _userManager.FindByEmailAsync(request.Email);
			var userByUsername = await _userManager.FindByNameAsync(request.Username);

			if (userByEmail is not null || userByUsername is not null)
			{
				throw new ArgumentException(
					$"User with email {request.Email} or username {request.Username} already exists.");
			}

			User user = new()
			{
				Email = request.Email,
				UserName = request.Username,
				SecurityStamp = Guid.NewGuid().ToString()
			};
			var result = await _userManager.CreateAsync(user, request.Password);
			if (!result.Succeeded)
			{
				throw new ArgumentException(
					$"Unable to register user {request.Username} errors: {GetErrorsText(result.Errors)}");
			}

			return await Login(new LoginDTO { Username = request.Email, Password = request.Password });
		}

		public async Task<string> Login(LoginDTO request)
		{
			var user = await _userManager.FindByNameAsync(request.Username);
			if (user is null)
			{
				user = await _userManager.FindByEmailAsync(request.Username);
			}

			if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
			{
				throw new ArgumentException($"Unable to authenticate user {request.Username}");
			}

			var authClaims = new List<Claim>
			{
				new(ClaimTypes.Name, user.UserName),
				new(ClaimTypes.Email, user.Email),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			};

			var token = GetToken(authClaims);
			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		private JwtSecurityToken GetToken(IEnumerable<Claim> authClaims)
		{
			var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
			var token = new JwtSecurityToken(
				issuer:_configuration["JWT:ValidIssuer"],
				audience:_configuration["JWT:ValidAudience"],
				expires:DateTime.Now.AddHours(3),
				claims:authClaims,
				signingCredentials:new SigningCredentials(authSigningKey,SecurityAlgorithms.HmacSha256));
			return token;
		}

		private string GetErrorsText(IEnumerable<IdentityError> errors)
		{
			return string.Join(", ", errors.Select(error => error.Description).ToArray());
		}
		// refactor this to send a user token to user
		
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

