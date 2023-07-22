using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker.Commands;

public sealed class ContextTickerUserConfirmCommand
{
    [Display(Name = "account group")]
    [UniqueName]
    public required string AccountGroup { get; init; }

    [MailAddress]
    public required string Mail { get; init; }

    [Secret]
    public required string Secret { get; init; }

    [Display(Name = "secret token")]
    [Token]
    public required Guid SecretToken { get; set; }
}