using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public static class ContextTickerUserBookmarkResource
{
    private const string CouldNotQueryTickerBookmarks = "Could not query ticker bookmarks";
    private const string CouldNotCreateTickerBookmark = "Could not create ticker bookmark";
    private const string CouldNotDeleteTickerBookmark = "Could not delete ticker bookmark";

    public static IEndpointRouteBuilder MapContextTickerUserBookmarkResource(this IEndpointRouteBuilder endpoints)
    {
        var resource = endpoints
            .MapGroup("/ticker-users/_/bookmarks")
            .RequireAuthorization(policy => policy
                .RequireClaim("type", "ticker")
                .RequireClaim("scope", "endpoints"))
            .WithTags("Context Ticker User Bookmark API");

        resource
            .MapGet(string.Empty, GetAll)
            .RequireAuthorization(policy => policy
                .RequireRole("ticker"))
            .WithErrorMessage(CouldNotQueryTickerBookmarks);

        resource
            .MapPost(string.Empty, Post)
            .RequireAuthorization(policy => policy
                .RequireRole("ticker"))
            .WithErrorMessage(CouldNotCreateTickerBookmark);

        resource
            .MapDelete("{tickerMessage}", Delete)
            .RequireAuthorization(policy => policy
                .RequireRole("ticker"))
            .WithErrorMessage(CouldNotDeleteTickerBookmark);

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