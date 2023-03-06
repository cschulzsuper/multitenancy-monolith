using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Endpoints
{
    public static IEndpointRouteBuilder MapAdmissionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var authentication = endpoints
            .MapGroup("admission")
            .WithGroupName("admission");

        authentication.MapAuthenticationIdentityCommands();
        authentication.MapAuthenticationIdentityResource();

        return endpoints;
    }
}