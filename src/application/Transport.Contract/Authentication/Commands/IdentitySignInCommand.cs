using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication.Commands;

public class IdentitySignInCommand
{
    [Required]
    [StringLength(140)]
    public required string Secret { get; init; }

    [Required]
    [StringLength(140)]
    public required string Client { get; init; }
}