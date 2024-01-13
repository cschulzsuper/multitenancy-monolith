using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Schedule;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public static class _Endpoints
{
    public static IEndpointRouteBuilder MapScheduleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var extension = endpoints
            .MapGroup("schedule")
            .WithGroupName("schedule");

        extension.MapPlannedJobResource();

        return extension;
    }
}