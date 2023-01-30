namespace ChristianSchulz.MultitenancyMonolith.Objects.Business;

public sealed class BusinessObject : ICloneable
{
    public object Clone()
        => MemberwiseClone();

    public long Snowflake { get; set; }

    public required string UniqueName { get; set; }
}
