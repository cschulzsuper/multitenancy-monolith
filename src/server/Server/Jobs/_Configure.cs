using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Events;
using ChristianSchulz.MultitenancyMonolith.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Server.Jobs;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
internal static class _Configure
{
    public static JobsOptions Configure(this JobsOptions options)
    {
        options.AfterJobInvocation = AfterJobInvocation;

        return options;
    }

    private static async Task AfterJobInvocation(IServiceProvider services, string _)
    {
        await services
            .GetRequiredService<IEventStorage>()
            .FlushAsync();
    }
}