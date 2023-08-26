using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.Portal.Models;

public class SignInModel
{
    [Required]
    public string? ClientName { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
