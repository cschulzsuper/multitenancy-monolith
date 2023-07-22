using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public static class TickerUserCommands
{
    private const string CouldNotResetTickerUser = "Could not execute ticker user reset";

    public static IEndpointRouteBuilder MapTickerUserCommands(this IEndpointRouteBuilder endpoints)
    {
        var resource = endpoints
            .MapGroup("/ticker-users")
            .WithTags("Ticker User Commands")
            .RequireAuthorization(policy => policy
                .RequireClaim("type", "member")
                .RequireClaim("scope", "endpoints"));

        resource
            .MapPost("{tickerUser}/reset", Reset)
            .RequireAuthorization(policy => policy
                .RequireRole("member"))
            .WithErrorMessage(CouldNotResetTickerUser);

        return endpoints;
    }

    private static Delegate Reset =>
        (ITickerUserCommandHandler commandHandler, long tickerUser)
            => commandHandler.ResetAsync(tickerUser);
}