using ChristianSchulz.MultitenancyMonolith.Application.Access.Commands;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

internal static class ContextAccountMemberCommands
{
    private const string CouldNotAuthAccountMember = "Could not execute account member auth";
    private const string CouldNotVerifyAccountMember = "Could not execute account member verify";

    public static IEndpointRouteBuilder MapContextAccountMemberCommands(this IEndpointRouteBuilder endpoints)
    {
        var commands = endpoints
            .MapGroup("/account-members/_")
            .WithTags("Context Account Member Commands");

        commands
            .MapPost("/auth", Auth)
            .RequireAuthorization(policy => policy
                .RequireClaim("type", "identity"))
            .WithErrorMessage(CouldNotAuthAccountMember)
            .Authenticates();

        commands
            .MapPost("/verify", Verify)
            .RequireAuthorization(policy => policy
                .RequireClaim("type", "member"))
            .WithErrorMessage(CouldNotVerifyAccountMember);

        return endpoints;
    }

    private static Delegate Auth =>
        async (IContextAccountMemberCommandHandler commandHandler, ContextAccountMemberAuthCommand command)
            => Results.SignIn(await commandHandler.AuthAsync(command), authenticationScheme: BearerTokenDefaults.AuthenticationScheme);

    private static Delegate Verify =>
        (IContextAccountMemberCommandHandler commandHandler)
            => commandHandler.Verify();
}