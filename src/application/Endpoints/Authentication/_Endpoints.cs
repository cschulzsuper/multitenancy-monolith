using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Endpoints
{
    public static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var authentication = endpoints
            .MapGroup("authentication")
            .WithGroupName("authentication");

        authentication.MapIdentityCommands();
        authentication.MapIdentityResource();

        return endpoints;
    }
}