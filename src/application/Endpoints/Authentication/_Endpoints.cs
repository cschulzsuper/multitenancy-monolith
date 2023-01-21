using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public static class _Endpoints
{
    public static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var authenticationEndpoints = endpoints
            .MapGroup("authentication")
            .WithGroupName("authentication");

        authenticationEndpoints.MapIdentityEndpoints();
        authenticationEndpoints.MapIdentitySignInEndpoints();

        return authenticationEndpoints;
    }
}