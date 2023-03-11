using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access.Commands;

public sealed class ContextAccountRegistrationConfirmCommand
{
    [Display(Name = "account group")]
    [UniqueName]
    public required string AccountGroup { get; init; }

    [Display(Name = "process token")]
    [Token]
    public required Guid ProcessToken { get; init; }
}