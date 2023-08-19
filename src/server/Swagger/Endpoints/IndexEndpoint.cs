using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Server.Swagger.Endpoints;

internal static class IndexEndpoint
{
    public static IEndpointRouteBuilder MapIndex(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapGet("/", Index);

        return endpoints;
    }

    private static Delegate Index =>
         () => Results.LocalRedirect("/swagger");
}