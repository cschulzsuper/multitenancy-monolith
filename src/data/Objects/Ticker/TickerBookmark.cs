using ChristianSchulz.MultitenancyMonolith.Shared.Metadata;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Objects.Ticker;


[ObjectAnnotation("ticker-bookmark",
    DisplayName = "Ticker Bookmark",
    Area = "ticker",
    Collection = "ticker-bookmarks")]
public sealed class TickerBookmark : ICloneable
{
    public object Clone()
        => MemberwiseClone();

    public long Snowflake { get; set; }

    public required long TickerMessage { get; set; }

    public required string TickerUser { get; set; }

    public required bool Updated { get; set; }
}