using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access.Commands;

public class AccountMemberAuthCommand
{
    [Display(Name = "account group")]
    [UniqueName]
    public required string AccountGroup { get; init; }

    [Display(Name = "account member")]
    [UniqueName]
    public required string AccountMember { get; init; }

    [Display(Name = "client name")]
    [Required]
    [StringLength(140)]
    public required string ClientName { get; init; }
}