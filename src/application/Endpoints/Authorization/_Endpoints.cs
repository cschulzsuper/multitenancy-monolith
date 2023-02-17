using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Endpoints
{
    public static IEndpointRouteBuilder MapAuthorizationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var authorization = endpoints
            .MapGroup("authorization")
            .WithGroupName("authorization");

        authorization.MapMemberResource();
        authorization.MapMemberCommands();

        return endpoints;
    }
}