using System.ComponentModel.DataAnnotations;

namespace Entwin.API.Models.Authentication;

public class LoginModel
{
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}