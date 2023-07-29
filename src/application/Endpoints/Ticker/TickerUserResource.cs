using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public static class TickerUserResource
{
    private const string CouldNotQueryTickerUsers = "Could not query ticker users";
    private const string CouldNotQueryTickerUser = "Could not query ticker user";
    private const string CouldNotCreateTickerUser = "Could not create ticker user";
    private const string CouldNotUpdateTickerUser = "Could not update ticker user";
    private const string CouldNotDeleteTickerUser = "Could not delete ticker user";

    public static IEndpointRouteBuilder MapTickerUserResource(this IEndpointRouteBuilder endpoints)
    {
        var resource = endpoints
            .MapGroup("/ticker-users")
            .WithTags("Ticker User API")
            .RequireAuthorization(policy => policy
                .RequireClaim("type", "member")
                .RequireClaim("scope", "endpoints"));

        resource
            .MapGet(string.Empty, GetAll)
            .RequireAuthorization(policy => policy
                .RequireRole("member", "member-observer"))
            .WithErrorMessage(CouldNotQueryTickerUsers);

        resource
            .MapGet("{tickerUser}", Get)
            .RequireAuthorization(policy => policy
                .RequireRole("member", "member-observer"))
            .WithErrorMessage(CouldNotQueryTickerUser);

        resource
            .MapPost(string.Empty, Post)
            .RequireAuthorization(policy => policy
                .RequireRole("member"))
            .WithErrorMessage(CouldNotCreateTickerUser);

        resource
            .MapPut("{tickerUser}", Put)
            .RequireAuthorization(policy => policy
                .RequireRole("member"))
            .WithErrorMessage(CouldNotUpdateTickerUser);

        resource
            .MapDelete("{tickerUser}", Delete)
            .RequireAuthorization(policy => policy
                .RequireRole("member"))
            .WithErrorMessage(CouldNotDeleteTickerUser);

        return endpoints;
    }

    private static Delegate GetAll =>
        (ITickerUserRequestHandler requestHandler,
            [FromQuery(Name = "q")] string? query,
            [FromQuery(Name = "s")] int? skip,
            [FromQuery(Name = "t")] int? take)

            => requestHandler.GetAll(query, skip, take);

    private static Delegate Get =>
        (ITickerUserRequestHandler requestHandler, long tickerUser)
            => requestHandler.GetAsync(tickerUser);

    private static Delegate Post =>
        (ITickerUserRequestHandler requestHandler, TickerUserRequest request)
            => requestHandler.InsertAsync(request);

    private static Delegate Put =>
        (ITickerUserRequestHandler requestHandler, long tickerUser, TickerUserRequest request)
            => requestHandler.UpdateAsync(tickerUser, request);

    private static Delegate Delete =>
        (ITickerUserRequestHandler requestHandler, long tickerUser)
            => requestHandler.DeleteAsync(tickerUser);
}