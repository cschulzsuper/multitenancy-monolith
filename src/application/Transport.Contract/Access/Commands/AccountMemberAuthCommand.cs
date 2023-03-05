using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access.Commands;

public class AccountMemberAuthCommand
{
    [UniqueName]
    public required string Group { get; init; }

    [UniqueName]
    public required string Member { get; init; }

    [Required]
    [StringLength(140)]
    public required string Client { get; init; }
}