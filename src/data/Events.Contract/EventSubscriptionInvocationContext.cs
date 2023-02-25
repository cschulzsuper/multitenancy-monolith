using System;

namespace ChristianSchulz.MultitenancyMonolith.Events;

public class EventSubscriptionInvocationContext
{
    public required string Scope { get; init; }

    public required IServiceProvider Services { get; set; }

    public required long Snowflake { get; init; }
}