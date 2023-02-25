namespace ChristianSchulz.MultitenancyMonolith.Events;

internal sealed class EventValue
{
    public required string Event { get; init; }

    public required string Scope { get; init; }

    public required long Snowflake { get; init; }
}