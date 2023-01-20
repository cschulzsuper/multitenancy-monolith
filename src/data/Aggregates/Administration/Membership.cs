namespace ChristianSchulz.MultitenancyMonolith.Aggregates.Administration;

public class Membership : ICloneable
{
    public object Clone()
        => MemberwiseClone();

    public long Snowflake { get; set; }

    public required string Identity { get; set; }
    public required long IdentitySnowflake { get; set; }

    public required string Group { get; set; }
    public required long GroupSnowflake { get; set; }

    public required string Member { get; set; }
    public required long MemberSnowflake { get; set; }
}