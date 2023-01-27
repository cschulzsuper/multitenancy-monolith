using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ChristianSchulz.MultitenancyMonolith.Application.Business;

public static class _Endpoints
{
    public static IEndpointRouteBuilder MapBusinessEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var business = endpoints
            .MapGroup("business")
            .WithGroupName("business");

        business.MapBusinessObjectResource();

        return endpoints;
    }
}