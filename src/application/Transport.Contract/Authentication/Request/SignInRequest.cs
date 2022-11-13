using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication.Request;

public class SignInRequest
{
    [Required]
    [StringLength(140)]
    public required string Password { get; init; }
}