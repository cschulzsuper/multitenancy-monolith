using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization.Commands;

public class MemberAuthCommand
{
    [UniqueName]
    public required string Group { get; init; }

    [UniqueName]
    public required string Member { get; init; }

    [Required]
    [StringLength(140)]
    public required string Client { get; init; }
}