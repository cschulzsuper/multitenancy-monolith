namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public class Member
{
    public long Snowflake { get; set; }
    public required string UniqueName { get; set; }
}