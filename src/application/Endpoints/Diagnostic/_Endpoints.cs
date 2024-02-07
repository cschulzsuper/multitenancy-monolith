using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Diagnostic;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public static class _Endpoints
{
    public static IEndpointRouteBuilder MapDiagnosticEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var extension = endpoints
            .MapGroup("diagnostic")
            .WithGroupName("diagnostic");

        extension.MapBuildInfoResource();

        return extension;
    }
}