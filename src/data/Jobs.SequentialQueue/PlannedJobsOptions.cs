using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Jobs;

public sealed class PlannedJobsOptions
{
    public Func<IServiceProvider, string, Task> BeforeJobInvocation { get; set; } = (_, _) => Task.CompletedTask;

    public Func<IServiceProvider, string, Task> AfterJobInvocation { get; set; } = (_, _) => Task.CompletedTask;
}