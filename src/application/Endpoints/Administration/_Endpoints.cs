using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public static class _Endpoints
{
    public static IEndpointRouteBuilder MapAdministrationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var administrationEndpoints = endpoints
            .MapGroup("administration")
            .WithGroupName("administration");

        administrationEndpoints.MapMemberEndpoints();
        administrationEndpoints.MapMemberSignInEndpoints();

        return administrationEndpoints;
    }
}