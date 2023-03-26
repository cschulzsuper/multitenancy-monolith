using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Extension;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Endpoints
{
    public static IEndpointRouteBuilder MapAdministrationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var extension = endpoints
            .MapGroup("extension")
            .WithGroupName("extension");

        extension.MapObjectTypeResource();
        extension.MapObjectTypeCustomPropertyResource();

        extension.MapDistinctionTypeResource();
        extension.MapDistinctionTypeCustomPropertyResource();

        return extension;
    }
}