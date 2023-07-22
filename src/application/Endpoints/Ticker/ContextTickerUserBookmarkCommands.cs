using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public static class ContextTickerUserBookmarkCommands
{
    private const string CouldNotAcknowledgeTickerBookmark = "Could not execute ticker bookmark acknowledge";

    public static IEndpointRouteBuilder MapContextTickerUserBookmarkCommands(this IEndpointRouteBuilder endpoints)
    {
        var commands = endpoints
            .MapGroup("/ticker-users/_/bookmarks")
            .RequireAuthorization(policy => policy
                .RequireClaim("type", "ticker")
                .RequireClaim("scope", "endpoints"))
            .WithTags("Context Ticker User Bookmark Commands");

        commands
            .MapPost("{tickerMessage}/confirm", Confirm)
            .RequireAuthorization(policy => policy
                .RequireRole("ticker"))
            .WithErrorMessage(CouldNotAcknowledgeTickerBookmark)
            .Authenticates();

        return endpoints;
    }

    private static Delegate Confirm =>
        (IContextTickerUserBookmarkCommandHandler commandHandler, long tickerMessage)
            => commandHandler.ConfirmAsync(tickerMessage);
}