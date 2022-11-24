using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge.EndpointFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal static class AuthorizationEndpoints
{
    public static IEndpointRouteBuilder MapAdministrationAuthorizationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var groupsEndpoints = endpoints
            .MapGroup("/groups")
            .WithTags("Groups");

        groupsEndpoints.MapPost("/register", Register);

        var memberEndpoints = groupsEndpoints
            .MapGroup("/{group}/members/{uniqueName}");

        memberEndpoints.MapPost("/take-up", TakeUp).AddEndpointFilter<BadgeResultEndpointFilter>();
        memberEndpoints.MapPost("/verify", Verify);

        return endpoints;
    }

    private static Delegate Register =>
        [Authorize]
        () => Results.StatusCode(StatusCodes.Status501NotImplemented);

    private static Delegate TakeUp =>
        [Authorize]
        (IAuthorizationRequestHandler requestHandler, ClaimsPrincipal user, string group, string member)
            => requestHandler.TakeUp(user, group, member);

    private static Delegate Verify =>
        [Authorize(Roles = "Memeber")]
        (IAuthorizationRequestHandler requestHandler)
            => requestHandler.Verify();
}
