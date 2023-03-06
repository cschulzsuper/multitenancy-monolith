using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker.Commands;

public class ContextTickerUserAuthCommand
{

    [Display(Name = "client name")]
    [Required]
    [StringLength(140)]
    public required string ClientName { get; init; }

    [Display(Name = "account group")]
    [UniqueName]
    public required string AccountGroup { get; init; }

    [MailAddress]
    public required string Mail { get; init; }

    [Secret]
    public required string Secret { get; init; }
}