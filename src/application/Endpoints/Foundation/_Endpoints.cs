using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ChristianSchulz.MultitenancyMonolith.Application.Foundation;

public static class _Endpoints
{
    public static IEndpointRouteBuilder MapFoundationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var foundationEndpoints = endpoints
            .MapGroup("foundation")
            .WithGroupName("foundation");

        return foundationEndpoints;
    }
}