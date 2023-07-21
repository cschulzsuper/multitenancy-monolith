using ChristianSchulz.MultitenancyMonolith.Jobs;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Subscriptions
{
    public static IJobScheduler MapHeartbeat(this IJobScheduler scheduler)
    {
        scheduler
            .Map("heartbeat", Heartbeat);

        return scheduler;
    }

    private static Func<ILoggerFactory, Task> Heartbeat =>
        (logger)
            =>
        {
            logger.CreateLogger("heartbeat").LogInformation("I'm alive {timestamp}", DateTime.Now);

            return Task.CompletedTask;
        };
}