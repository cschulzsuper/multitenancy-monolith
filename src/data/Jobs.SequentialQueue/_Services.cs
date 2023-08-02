using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Jobs;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddPlannedJobs(this IServiceCollection services, Action<PlannedJobsOptions> setup)
    {
        services.Configure(setup);
        services.AddSingleton<PlannedJobsOptions>(provider => provider.GetRequiredService<IOptions<PlannedJobsOptions>>().Value);

        services.AddSingleton<IPlannedJobQueue, PlannedJobQueue>();
        services.AddSingleton<IPlannedJobScheduler, PlannedJobScheduler>();

        services.AddHostedService<PlannedJobService>();

        return services;
    }
}