using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Jobs;

internal record PlannedJobInfo
{
    public required string UniqueName { get; init; }

    public required IPlannedJobSchedule Schedule { get; init; }

    public required Func<IServiceProvider, Task> Job { get; init; }
}