using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Commands;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public static class ContextTickerUserCommands
{
    private const string CouldNotAuthTickerUser = "Could not execute ticker user auth";
    private const string CouldNotConfirmTickerUser = "Could not execute ticker user confirm";
    private const string CouldNotPostTickerUser = "Could not execute ticker user post";
    private const string CouldNotVerifyTickerUser = "Could not execute ticker user verify";

    public static IEndpointRouteBuilder MapContextTickerUserCommands(this IEndpointRouteBuilder endpoints)
    {
        var commands = endpoints
            .MapGroup("/ticker-users/_")
            .WithTags("Context Ticker User Commands");

        commands
            .MapPost("/auth", Auth)
            .WithErrorMessage(CouldNotAuthTickerUser)
            .Authenticates();

        commands
            .MapPost("/confirm", Confirm)
            .WithErrorMessage(CouldNotConfirmTickerUser)
            .Authenticates();

        commands
            .MapPost("/verify", Verify)
            .RequireAuthorization(policy => policy
                .RequireClaim("type", "ticker")
                .RequireRole("ticker"))
            .WithErrorMessage(CouldNotVerifyTickerUser);

        commands
            .MapPost("/post", Post)
            .RequireAuthorization(policy => policy
                .RequireClaim("type", "ticker")
                .RequireRole("ticker"))
            .WithErrorMessage(CouldNotPostTickerUser);

        return endpoints;
    }

    private static Delegate Auth =>
        async  (IContextTickerUserCommandHandler commandHandler, ContextTickerUserAuthCommand command)
            => Results.SignIn(await commandHandler.AuthAsync(command), authenticationScheme: BearerTokenDefaults.AuthenticationScheme);

    private static Delegate Confirm =>
        (IContextTickerUserCommandHandler commandHandler, ContextTickerUserConfirmCommand command)
            => commandHandler.ConfirmAsync(command);

    private static Delegate Post =>
        (IContextTickerUserCommandHandler commandHandler, ContextTickerUserPostCommand command)
            => commandHandler.PostAsync(command);

    private static Delegate Verify =>
        (IContextTickerUserCommandHandler commandHandler)
            => commandHandler.Verify();
}