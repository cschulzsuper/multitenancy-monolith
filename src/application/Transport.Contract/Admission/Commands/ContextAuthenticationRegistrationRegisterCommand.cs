using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission.Commands;

public sealed class ContextAuthenticationRegistrationRegisterCommand
{
    [Display(Name = "authentication identity")]
    [UniqueName]
    public required string AuthenticationIdentity { get; init; }

    [MailAddress]
    public required string MailAddress { get; init; }

    [Secret]
    public required string Secret { get; init; }
}