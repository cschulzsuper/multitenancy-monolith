using ChristianSchulz.MultitenancyMonolith.Application.Authorization.Commands;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

internal static class MemberCommands
{
    private const string CouldNotAuthMember = "Could not auth member";
    private const string CouldNotVerifyMember = "Could not verify member";

    public static IEndpointRouteBuilder MapMemberCommands(this IEndpointRouteBuilder endpoints)
    {
        var commands = endpoints
            .MapGroup("/members")
            .WithTags("Member Commands");

        commands
            .MapPost("/me/auth", Auth)
            .RequireAuthorization(policy => policy
                .RequireClaim("badge", "identity"))
            .WithErrorMessage(CouldNotAuthMember)
            .Authenticates()
            .AddEndpointFilter<BadgeResultEndpointFilter>();

        commands
            .MapPost("/me/verify", Verify)
            .RequireAuthorization(policy => policy
                .RequireClaim("badge", "member"))
            .WithErrorMessage(CouldNotVerifyMember);

        return endpoints;
    }

    private static Delegate Auth =>
        (IMemberCommandHandler commandHandler, MemberAuthCommand command)
            => commandHandler.AuthAsync(command);

    private static Delegate Verify =>
        (IMemberCommandHandler commandHandler)
            => commandHandler.Verify();
}