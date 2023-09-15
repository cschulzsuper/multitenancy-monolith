using System;

namespace ChristianSchulz.MultitenancyMonolith.Objects.Documentation;

public sealed class DevelopmentPost : ICloneable
{
    public object Clone()
        => MemberwiseClone();

    public long Snowflake { get; set; }

    public required long Index { get; set; }

    public required string Project { get; set; }

    public required string Title { get; set; }

    public required DateTime Time { get; set; }

    public required string Text { get; set; }

    public required string Link { get; set; }

    public required string[] Tags { get; set; }

}
