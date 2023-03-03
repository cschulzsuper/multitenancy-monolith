namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker.Responses;

public class TickerUserDependentBookmarkResponse
{
    public required long TickerMessage { get; set; }

    public required bool Updated { get; set; }

}