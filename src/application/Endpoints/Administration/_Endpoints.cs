using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public static class _Endpoints
{
    public static IEndpointRouteBuilder MapAdministrationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var administration = endpoints
            .MapGroup("administration")
            .WithGroupName("administration");

        administration.MapAggregateTypeResource();
        administration.MapAggregateTypeCustomPropertyResource();

        administration.MapDistinctionTypeResource();
        administration.MapDistinctionTypeCustomPropertyResource();

        return administration;
    }
}