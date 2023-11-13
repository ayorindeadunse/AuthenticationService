using AuthenticationService.Business.Models.DTOs;
using AuthenticationService.Business.Services;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
    {
        var result = await _authService.RegisterUserAsync(registerDto);
        if (result.IsRequestSuccessful)
        {
            return Ok(new {Token = result.Data});
        }

        return BadRequest(new {ErrorMessage = result.Errors});
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
    {
        var result = await _authService.LoginUserAsync(loginDto);
        if (result.IsRequestSuccessful)
        {
            return Ok(new { Token = result.Data });
        }

        return BadRequest(new { ErrorMessage = result.Errors });
    }

    [HttpPost("social-login")]
    public IActionResult SocialLogin([FromBody] SocialLoginRequest socialLoginRequest)
    {
        var result = _authService.SocialLoginAsync(socialLoginRequest.Provider, socialLoginRequest.ProviderToken);

        if (result.IsRequestSuccessful)
        {
            return Ok(new { Token = result.Data });
        }

        return BadRequest(new { ErrorMessage = result.Errors });
    }

    [HttpPost("social-register")]
    public async Task<IActionResult> SocialRegister([FromBody] SocialLoginRequest socialLoginRequest)
    {
        var result =
            await _authService.SocialRegisterAsync(socialLoginRequest.Provider, socialLoginRequest.ProviderToken);
        if (result.IsRequestSuccessful)
        {
            return Ok(new { Token = result.Data });
        }

        return BadRequest(new { ErrorMessage = result.Errors });
    }
}