using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Schedule;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddScheduleOrchestration(this IServiceCollection services)
    {
        services.AddScoped<IPlannedJobRescheduler, PlannedJobRescheduler>();

        return services;
    }
}