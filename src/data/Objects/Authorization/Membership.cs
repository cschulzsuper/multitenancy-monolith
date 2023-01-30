namespace ChristianSchulz.MultitenancyMonolith.Objects.Authorization;

public sealed class Membership : ICloneable
{
    public object Clone()
        => MemberwiseClone();

    public long Snowflake { get; set; }

    public required string Identity { get; set; }

    public required string Group { get; set; }

    public required string Member { get; set; }
}