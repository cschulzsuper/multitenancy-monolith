using System;

namespace ChristianSchulz.MultitenancyMonolith.Jobs;

public sealed class PlannedJobContext
{
    public required IServiceProvider Services { get; init; }
}
