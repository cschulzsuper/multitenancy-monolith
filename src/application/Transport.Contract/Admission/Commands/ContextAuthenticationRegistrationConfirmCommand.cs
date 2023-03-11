using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission.Commands;

public sealed class ContextAuthenticationRegistrationConfirmCommand
{
    [Display(Name = "authentication identity")]
    [UniqueName]
    public required string AuthenticationIdentity { get; init; }

    [Secret]
    public required string Secret { get; init; }

    [Display(Name = "process token")]
    [Token]
    public required Guid ProcessToken { get; init; }
}