using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker.Requests;

public class TickerUserDependentBookmarkRequest
{
    [Display(Name = "ticker message")]
    [Snowflake]
    public required long TickerMessage { get; set; }
}