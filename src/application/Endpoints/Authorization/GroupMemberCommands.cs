using ChristianSchulz.MultitenancyMonolith.Application.Authorization.Commands;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge.EndpointFilters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

internal static class GroupMemberCommands
{
    private const string CouldNotSignInGroupMember = "Could not sign in group member";

    public static IEndpointRouteBuilder MapGroupMemberCommands(this IEndpointRouteBuilder endpoints)
    {
        var commands = endpoints
            .MapGroup("/groups/{group}/members")
            .WithTags("Group Member Commands");

        commands
            .MapPost("/{member}/sign-in", SignIn)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("default")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotSignInGroupMember)
            .AddEndpointFilter<BadgeResultEndpointFilter>();

        return endpoints;
    }

    private static Delegate SignIn =>
        (IMemberCommandHandler commandHandler, string group, string member, MemberCommand command)
            => commandHandler.SignIn(group, member, command);
}