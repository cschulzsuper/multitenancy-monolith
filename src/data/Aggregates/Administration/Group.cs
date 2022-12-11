namespace ChristianSchulz.MultitenancyMonolith.Aggregates.Administration;

public class Group
{
    public long Snowflake { get; set; }
    public required string UniqueName { get; set; }
}