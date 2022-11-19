using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public static class AuthorizationEndpoints
{
    public static IEndpointRouteBuilder MapAuthorizationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var groupsEndpoints = endpoints
            .MapGroup("/groups")
            .WithTags("Groups");

        groupsEndpoints.MapPost("/register", Register);

        var memberEndpoints = groupsEndpoints
            .MapGroup("/{group}/members/{member}");

        memberEndpoints.MapPost("/take-up", TakeUp).AddEndpointFilter<BadgeResultEndpointFilter>(); ;
        memberEndpoints.MapPost("/verify", TakeUp);

        return endpoints;
    }

    private static Delegate Register =>
        [Authorize]
        () => Results.StatusCode(StatusCodes.Status501NotImplemented);

    private static Delegate TakeUp =>
        [Authorize]
        (IAuthorizationRequestHandler requestHandler, ClaimsPrincipal user, string group, string member)
            => requestHandler.TakeUp(user, group, member);
}
