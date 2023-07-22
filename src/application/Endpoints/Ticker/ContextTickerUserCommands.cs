using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Commands;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge.Serialization;
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
            .Authenticates()
            .AddEndpointFilter<BadgeResultEndpointFilter>();

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
        (IContextTickerUserCommandHandler commandHandler, ContextTickerUserAuthCommand command)
            => commandHandler.AuthAsync(command);

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