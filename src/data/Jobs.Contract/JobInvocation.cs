using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Jobs;

public sealed class JobCallback
{
    public required string UniqueName { get; init; }

    public required DateTime Timestamp { get; init; }

    public required Func<IServiceProvider, Task> Job { get; init; }
}
