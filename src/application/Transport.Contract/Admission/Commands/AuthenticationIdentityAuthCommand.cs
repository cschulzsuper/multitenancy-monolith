using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission.Commands;

public class AuthenticationIdentityAuthCommand
{
    [UniqueName]
    public required string Identity { get; init; }

    [Secret]
    public required string Secret { get; init; }

    [Required]
    [StringLength(140)]
    public required string Client { get; init; }
}