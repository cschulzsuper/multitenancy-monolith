namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker.Responses;

public sealed class TickerMessageResponse
{
    public required long Snowflake { get; set; }

    public required string Text { get; set; }

    public required string Priority { get; set; }

    public required long Timestamp { get; set; }

    public required string TickerUser { get; set; }
}