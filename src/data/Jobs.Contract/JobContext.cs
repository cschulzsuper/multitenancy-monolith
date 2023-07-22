using System;

namespace ChristianSchulz.MultitenancyMonolith.Jobs
{
    public sealed class JobContext
    {
        public required IServiceProvider Services { get; init; }
    }
}
