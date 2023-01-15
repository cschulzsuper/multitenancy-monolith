using ChristianSchulz.MultitenancyMonolith.Application.Administration.Requests;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge.EndpointFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal static class MemberSignInEndpoints
{
    private const string CouldNotRegisterMember = "Could not register member";
    private const string CouldNotSignInMember = "Could not sign in member";
    private const string CouldNotVerifyMember = "Could not verify member";

    public static IEndpointRouteBuilder MapMemberSignInEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var groupsEndpoints = endpoints
            .MapGroup("/groups")
            .WithTags("Groups");

        groupsEndpoints
            .MapPost("/register", Register)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("admin")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotRegisterMember);

        var groupMemberEndpoints = groupsEndpoints
            .MapGroup("/{group}/members/{member}");

        groupMemberEndpoints
            .MapPost("/sign-in", SignIn)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("default")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotSignInMember)
            .AddEndpointFilter<BadgeResultEndpointFilter>();

        var membersMeEndpoints = endpoints
            .MapGroup("/members/me")
            .WithTags("Members");

        membersMeEndpoints
            .MapPost("/verify", Verify)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("member", "observer")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotVerifyMember);

        return endpoints;
    }

    private static Delegate Register =>
        () => Results.StatusCode(StatusCodes.Status501NotImplemented);

    private static Delegate SignIn =>
        (IMemberSignInRequestHandler requestHandler, string group, string member, MemberSignInRequest request)
            => requestHandler.SignIn(group, member, request);

    private static Delegate Verify =>
        (IMemberSignInRequestHandler requestHandler)
            => requestHandler.Verify();
}
