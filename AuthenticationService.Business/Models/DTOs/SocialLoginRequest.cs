using System.ComponentModel.DataAnnotations;
using AuthenticationService.Business.Consts;

namespace AuthenticationService.Business.Models.DTOs;

public class SocialLoginRequest
{
    public string Provider { get; set; }
    
    [MinLength(Constants.UsernameMinLength, ErrorMessage = Constants.UsernameLengthValidationError)]
    public string? Email { get; set; }
    public string ProviderToken { get; set; }
}