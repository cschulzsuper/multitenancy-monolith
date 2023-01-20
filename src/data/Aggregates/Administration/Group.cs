namespace ChristianSchulz.MultitenancyMonolith.Aggregates.Administration;

public class Group : ICloneable
{
    public object Clone()
        => MemberwiseClone();

    public long Snowflake { get; set; }

    public required string UniqueName { get; set; }
}