using AuthenticationService.Business.Extensions;
using AuthenticationService.Business.Models.DTOs;
using AuthenticationService.Business.Services;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
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

    [AllowAnonymous]
    [HttpPost("user/login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UserLogin([FromBody] LoginDTO request)
    {
        var response = await _authService.Login(request);

        return Ok(response);
    }
    
    [AllowAnonymous]
    [HttpPost("user/register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UserRegister([FromBody] RegisterDTO request)
    {
        var response = await _authService.Register(request);

        return Ok(response);
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

    [AllowAnonymous]
    [HttpPost("social-login")]
    [ProducesResponseType(StatusCodes.Status200OK,Type = typeof(ResultDTO<string>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status400BadRequest,Type = typeof(ResultDTO<string>))]
    public async Task<IActionResult> SocialLogin([FromBody] SocialLoginRequest socialLoginRequest)
    {
        var result = await _authService.SocialLogin(socialLoginRequest);
        var resultDTO = result.ToResultDto();
        if (!resultDTO.IsSuccess)
        {
            return BadRequest(resultDTO);
        }

        return Ok(resultDTO);
    }
}