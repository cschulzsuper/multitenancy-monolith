using ChristianSchulz.MultitenancyMonolith.Application.Access.Commands;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

internal static class AccountMemberCommands
{
    private const string CouldNotAuthAccountMember = "Could not auth account member";
    private const string CouldNotVerifyAccountMember = "Could not verify account member";

    public static IEndpointRouteBuilder MapAccountMemberCommands(this IEndpointRouteBuilder endpoints)
    {
        var commands = endpoints
            .MapGroup("/account-members")
            .WithTags("Account Member Commands");

        commands
            .MapPost("/me/auth", Auth)
            .RequireAuthorization(policy => policy
                .RequireClaim("badge", "identity"))
            .WithErrorMessage(CouldNotAuthAccountMember)
            .Authenticates()
            .AddEndpointFilter<BadgeResultEndpointFilter>();

        commands
            .MapPost("/me/verify", Verify)
            .RequireAuthorization(policy => policy
                .RequireClaim("badge", "member"))
            .WithErrorMessage(CouldNotVerifyAccountMember);

        return endpoints;
    }

    private static Delegate Auth =>
        (IAccountMemberCommandHandler commandHandler, AccountMemberAuthCommand command)
            => commandHandler.AuthAsync(command);

    private static Delegate Verify =>
        (IAccountMemberCommandHandler commandHandler)
            => commandHandler.Verify();
}