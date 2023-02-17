using ChristianSchulz.MultitenancyMonolith.Shared.Metadata;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Objects.Ticker;

[ObjectAnnotation("ticker-user",
    DisplayName = "Ticker User",
    Area = "ticker",
    Collection = "ticker-users")]
public class TickerUser : ICloneable
{
    public object Clone()
        => MemberwiseClone();

    public long Snowflake { get; set; }

    public required string DisplayName { get; set; }

    public required string MailAddress { get; set; }

    public required string Secret { get; set; }
}
