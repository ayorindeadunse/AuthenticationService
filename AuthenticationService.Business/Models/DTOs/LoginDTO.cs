using System;
using IdentityServer4.Models;
using System.ComponentModel.DataAnnotations;

namespace AuthenticationService.Business.Models.DTOs
{
	public class LoginDTO
	{
        [Required(ErrorMessage = "THe Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "THe Password is required.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*\W).{8,}$", ErrorMessage = "Password must have at least 8 characters, one upper case letter, one number, and one special character.")]
        public string Password { get; set; }
    }
}

