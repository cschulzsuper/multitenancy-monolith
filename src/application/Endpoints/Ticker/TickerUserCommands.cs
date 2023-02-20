using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Commands;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public static class TickerUserCommands
{
    private const string CouldNotAuthTickerUser = "Could not execute ticker user auth";
    private const string CouldNotConfirmTickerUser = "Could not execute ticker user confirm";
    private const string CouldNotPostTickerUser = "Could not execute ticker user post";
    private const string CouldNotVerifyTickerUser = "Could not execute ticker user verify";

    public static IEndpointRouteBuilder MapTickerUserCommands(this IEndpointRouteBuilder endpoints)
    {
        var commands = endpoints
            .MapGroup("/ticker-users")
            .WithTags("Ticker User Commands");

        commands
            .MapPost("/me/auth",Auth)
            .WithErrorMessage(CouldNotAuthTickerUser)
            .Authenticates()
            .AddEndpointFilter<BadgeResultEndpointFilter>();

        commands
            .MapPost("/me/confirm", Confirm)
            .WithErrorMessage(CouldNotConfirmTickerUser)
            .Authenticates()
            .AddEndpointFilter<BadgeResultEndpointFilter>();

        commands
            .MapPost("/me/verify", Verify)
            .RequireAuthorization(policy => policy
                .RequireClaim("badge", "ticker"))
            .WithErrorMessage(CouldNotVerifyTickerUser);

        commands
            .MapPost("/me/post", Post)
            .RequireAuthorization(policy => policy
                .RequireClaim("badge", "ticker"))
            .WithErrorMessage(CouldNotPostTickerUser);

        return endpoints;
    }

    private static Delegate Auth =>
        (ITickerUserCommandHandler commandHandler, TickerUserAuthCommand command)
            => commandHandler.AuthAsync(command);

    private static Delegate Confirm =>
        (ITickerUserCommandHandler commandHandler, TickerUserConfirmCommand command)
            => commandHandler.ConfirmAsync(command);

    private static Delegate Post =>
        (ITickerUserCommandHandler commandHandler, TickerUserPostCommand command)
            => commandHandler.PostAsync(command);

    private static Delegate Verify =>
        (ITickerUserCommandHandler commandHandler)
            => commandHandler.Verify();
}