using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker.Requests;

public sealed class ContextTickerUserBookmarkRequest
{
    [Display(Name = "ticker message")]
    [Snowflake]
    public required long TickerMessage { get; set; }
}