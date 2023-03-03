using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public static class ContextTickerUserBookmarkResource
{
    private const string CouldNotQueryTickerUsers = "Could not query ticker users";
    private const string CouldNotCreateTickerUser = "Could not create ticker user";
    private const string CouldNotDeleteTickerUser = "Could not delete ticker user";

    public static IEndpointRouteBuilder MapContextTickerUserBookmarkResource(this IEndpointRouteBuilder endpoints)
    {
        var resource = endpoints
            .MapGroup("/ticker-users/me/bookmarks")
            .RequireAuthorization(policy => policy
                .RequireClaim("badge", "ticker")
                .RequireClaim("scope", "endpoints"))
            .WithTags("Ticker User Dependent Bookmark API");

        resource
            .MapGet(string.Empty, GetAll)
            .RequireAuthorization(policy => policy
                .RequireRole("ticker"))
            .WithErrorMessage(CouldNotQueryTickerUsers);

        resource
            .MapPost(string.Empty, Post)
            .RequireAuthorization(policy => policy
                .RequireRole("ticker"))
            .WithErrorMessage(CouldNotCreateTickerUser);

        resource
            .MapDelete("{tickerMessage}", Delete)
            .RequireAuthorization(policy => policy
                .RequireRole("ticker"))
            .WithErrorMessage(CouldNotDeleteTickerUser);

        return endpoints;
    }

    private static Delegate GetAll =>
        (IContextTickerUserBookmarkRequestHandler requestHandler,
            [FromQuery(Name = "q")] string? query,
            [FromQuery(Name = "s")] int? skip,
            [FromQuery(Name = "t")] int? take)

            => requestHandler.GetAll(query, skip, take);

    private static Delegate Post =>
        (IContextTickerUserBookmarkRequestHandler requestHandler, ContextTickerUserBookmarkRequest request)
            => requestHandler.InsertAsync(request);

    private static Delegate Delete =>
        (IContextTickerUserBookmarkRequestHandler requestHandler, long tickerMessage)
            => requestHandler.DeleteAsync(tickerMessage);
}