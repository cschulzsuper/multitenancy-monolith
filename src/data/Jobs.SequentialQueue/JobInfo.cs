using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Jobs;

internal record JobInfo
{
    public required string UniqueName { get; init; }

    public required IJobSchedule Schedule { get; init; }

    public required Func<IServiceProvider, Task> Job { get; init; }
}