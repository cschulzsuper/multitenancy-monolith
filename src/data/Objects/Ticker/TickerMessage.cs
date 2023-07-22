using ChristianSchulz.MultitenancyMonolith.Shared.Metadata;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Objects.Ticker;

[ObjectAnnotation("ticker-message",
    DisplayName = "Ticker Message",
    Area = "ticker",
    Collection = "ticker-messages")]
public sealed class TickerMessage : ICloneable
{
    public object Clone()
        => MemberwiseClone();

    public long Snowflake { get; set; }

    public required string Text { get; set; }

    public required string Priority { get; set; }

    public required long Timestamp { get; set; }

    public required string TickerUser { get; set; }
}