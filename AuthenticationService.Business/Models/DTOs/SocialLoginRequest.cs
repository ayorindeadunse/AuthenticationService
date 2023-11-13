namespace AuthenticationService.Business.Models.DTOs;

public class SocialLoginRequest
{
    public string Provider { get; set; }
    public string ProviderToken { get; set; }
}