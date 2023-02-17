using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Endpoints
{
    public static IEndpointRouteBuilder MapAdministrationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var administration = endpoints
            .MapGroup("administration")
            .WithGroupName("administration");

        administration.MapObjectTypeResource();
        administration.MapObjectTypeCustomPropertyResource();

        administration.MapDistinctionTypeResource();
        administration.MapDistinctionTypeCustomPropertyResource();

        return administration;
    }
}