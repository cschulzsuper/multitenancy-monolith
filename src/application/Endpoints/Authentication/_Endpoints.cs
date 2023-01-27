using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public static class _Endpoints
{
    public static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var authentication = endpoints
            .MapGroup("authentication")
            .WithGroupName("authentication");

        authentication.MapIdentityResource();
        authentication.MapIdentityCommands();

        return endpoints;
    }
}