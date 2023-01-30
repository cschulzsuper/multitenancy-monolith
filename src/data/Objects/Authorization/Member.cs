namespace ChristianSchulz.MultitenancyMonolith.Objects.Authorization;

public sealed class Member : ICloneable
{
    public object Clone()
        => MemberwiseClone();

    public long Snowflake { get; set; }

    public required string UniqueName { get; set; }
}