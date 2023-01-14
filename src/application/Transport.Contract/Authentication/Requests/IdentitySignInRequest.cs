using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication.Requests;

public class IdentitySignInRequest
{
    [Required]
    [StringLength(140)]
    public required string Secret { get; init; }

    [Required]
    [StringLength(140)]
    public required string Client { get; init; }
}