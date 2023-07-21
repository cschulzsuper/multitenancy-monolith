using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ChristianSchulz.MultitenancyMonolith.Jobs;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddJobs(this IServiceCollection services, Action<JobsOptions> setup)
    {
        services.Configure(setup);
        services.AddSingleton<JobsOptions>(provider => provider.GetRequiredService<IOptions<JobsOptions>>().Value);

        services.AddSingleton<IJobQueue, JobQueue>();
        services.AddSingleton<IJobScheduler, JobScheduler>();

        services.AddHostedService<JobService>();

        return services;
    }
}