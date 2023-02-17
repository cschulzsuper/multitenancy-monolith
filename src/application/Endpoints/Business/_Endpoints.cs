using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Business;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
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