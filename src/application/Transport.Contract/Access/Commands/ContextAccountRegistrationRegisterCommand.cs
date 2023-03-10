using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access.Commands;

public sealed class ContextAccountRegistrationRegisterCommand
{
    [Display(Name = "account group")]
    [UniqueName]
    public required string AccountGroup { get; init; }

    [Display(Name = "account member")]
    [UniqueName]
    public required string AccountMember { get; init; }

    [MailAddress]
    public required string MailAddress { get; init; }
}