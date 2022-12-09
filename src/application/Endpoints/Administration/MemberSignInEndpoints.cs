﻿using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge.EndpointFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal static class MemberSignInEndpoints
{
    public static IEndpointRouteBuilder MapMemberSignInEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var groupsEndpoints = endpoints
            .MapGroup("/groups")
            .WithTags("Groups");

        groupsEndpoints.MapPost("/register", Register);

        var groupMemberEndpoints = groupsEndpoints
            .MapGroup("/{group}/members/{member}");

        groupMemberEndpoints.MapPost("/sign-in", SignIn).AddEndpointFilter<BadgeResultEndpointFilter>();

        var membersMeEndpoints = endpoints
            .MapGroup("/members/me")
            .WithTags("Members");

        membersMeEndpoints.MapPost("/verify", Verify);

        return endpoints;
    }

    private static Delegate Register =>
        [Authorize]
        () => Results.StatusCode(StatusCodes.Status501NotImplemented);

    private static Delegate SignIn =>
        [Authorize]
        (IMemberSignInRequestHandler requestHandler, string group, string member)
            => requestHandler.SignIn(group, member);

    private static Delegate Verify =>
        [Authorize(Roles = "Member")]
        (IMemberSignInRequestHandler requestHandler)
            => requestHandler.Verify();
}