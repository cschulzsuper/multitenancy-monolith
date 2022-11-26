using Microsoft.AspNetCore.Routing;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public static class _Endpoints
{
    public static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapIdentityEndpoints();
        endpoints.MapIdentitySignInEndpoints();

        return endpoints;
    }
}
