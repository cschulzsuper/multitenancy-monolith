using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Responses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public static class TickerUserDependentBookmarkResource
{
    private const string CouldNotQueryTickerUsers = "Could not query ticker users";
    private const string CouldNotCreateTickerUser = "Could not create ticker user";
    private const string CouldNotDeleteTickerUser = "Could not delete ticker user";

    public static IEndpointRouteBuilder MapTickerUserDependentBookmarkResource(this IEndpointRouteBuilder endpoints)
    {
        var resource = endpoints
            .MapGroup("/ticker-users/me/bookmarks")
            .WithTags("Ticker User Dependent Bookmark API");

        resource
            .MapGet(string.Empty, GetAll)
            .RequireAuthorization(policy => policy
                .RequireClaim("badge", "ticker"))
            .WithErrorMessage(CouldNotQueryTickerUsers);

        resource
            .MapPost(string.Empty, Post)
            .RequireAuthorization(policy => policy
                .RequireClaim("badge", "ticker"))
            .WithErrorMessage(CouldNotCreateTickerUser);

        resource
            .MapDelete("{tickerMessage}", Delete)
            .RequireAuthorization(policy => policy
                .RequireClaim("badge", "ticker"))
            .WithErrorMessage(CouldNotDeleteTickerUser);

        return endpoints;
    }

    private static Delegate GetAll =>
        (ITickerUserDependentBookmarkRequestHandler requestHandler,
            [FromQuery(Name = "q")] string? query,
            [FromQuery(Name = "s")] int? skip,
            [FromQuery(Name = "t")] int? take)

            => requestHandler.GetAll(query, skip, take);

    private static Delegate Post =>
        (ITickerUserDependentBookmarkRequestHandler requestHandler, TickerUserDependentBookmarkRequest request)
            => requestHandler.InsertAsync(request);

    private static Delegate Delete =>
        (ITickerUserDependentBookmarkRequestHandler requestHandler, long tickerMessage)
            => requestHandler.DeleteAsync(tickerMessage);
}