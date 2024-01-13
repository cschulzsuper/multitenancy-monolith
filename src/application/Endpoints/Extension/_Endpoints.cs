using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Extension;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public static class _Endpoints
{
    public static IEndpointRouteBuilder MapExtensionEndpoints(this IEndpointRouteBuilder endpoints)
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