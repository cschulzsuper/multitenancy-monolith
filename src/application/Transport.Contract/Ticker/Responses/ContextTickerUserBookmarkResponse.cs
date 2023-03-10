namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker.Responses;

public sealed class ContextTickerUserBookmarkResponse
{
    public required long TickerMessage { get; set; }

    public required bool Updated { get; set; }

}