using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

public static class _Endpoints
{
    public static IEndpointRouteBuilder MapAuthorizationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var authorization = endpoints
            .MapGroup("authorization")
            .WithGroupName("authorization");

        authorization.MapGroupCommands();

        authorization.MapGroupMemberCommands();

        authorization.MapMemberResource();
        authorization.MapMemberCommands();

        return endpoints;
    }
}