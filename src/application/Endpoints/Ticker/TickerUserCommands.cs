using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Commands;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public static class TickerUserCommands
{
    private const string CouldNotAuthTickerUser = "Could not auth ticker user";
    private const string CouldNotConfirmTickerUser = "Could not confirm ticker user";
    private const string CouldNotVerifyTickerUser = "Could not verify ticker user";

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
                .RequireClaim("badge", "ticker", "member"))
            .WithErrorMessage(CouldNotVerifyTickerUser);

        return endpoints;
    }

    private static Delegate Auth =>
        (ITickerUserCommandHandler commandHandler, TickerUserAuthCommand command)
            => commandHandler.AuthAsync(command);

    private static Delegate Confirm =>
        (ITickerUserCommandHandler commandHandler, TickerUserConfirmCommand command)
            => commandHandler.ConfirmAsync(command);

    private static Delegate Verify =>
        (ITickerUserCommandHandler commandHandler)
            => commandHandler.Verify();
}