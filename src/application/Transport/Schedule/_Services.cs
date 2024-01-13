using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Schedule;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public static class _Services
{
    public static IServiceCollection AddScheduleTransport(this IServiceCollection services)
    {
        services.AddScoped<IPlannedJobRequestHandler, PlannedJobRequestHandler>();

        return services;
    }
}