using Microsoft.AspNetCore.Routing;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public static class _Endpoints
{
    public static IEndpointRouteBuilder MapAdministrationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapAdministrationAuthorizationEndpoints();
        endpoints.MapAdministrationMemberEndpoints();

        return endpoints;
    }
}
