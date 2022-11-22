using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication.Requests;

public class SignInRequest
{
    [Required]
    [StringLength(140)]
    public required string Secret { get; init; }
}