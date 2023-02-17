using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public static class TickerMessageResource
{
    private const string CouldNotQueryTickerMessages = "Could not query ticker messages";
    private const string CouldNotQueryTickerMessage = "Could not query ticker message";
    private const string CouldNotCreateTickerMessage = "Could not create ticker message";
    private const string CouldNotUpdateTickerMessage = "Could not update ticker message";
    private const string CouldNotDeleteTickerMessage = "Could not delete ticker message";

    public static IEndpointRouteBuilder MapTickerMessageResource(this IEndpointRouteBuilder endpoints)
    {
        var resource = endpoints
            .MapGroup("/ticker-messages")
            .WithTags("Ticker Message API")
            .RequireAuthorization(policy => policy
                .RequireClaim("badge", "member")
                .RequireClaim("scope", "endpoints"));

        resource
            .MapGet(string.Empty, GetAll)
            .RequireAuthorization(policy => policy
                .RequireRole("member"))
            .WithErrorMessage(CouldNotQueryTickerMessages);

        resource
            .MapGet("{TickerMessage}", Get)
            .RequireAuthorization(policy => policy
                .RequireRole("member"))
            .WithErrorMessage(CouldNotQueryTickerMessage);

        resource
            .MapPost(string.Empty, Post)
            .RequireAuthorization(policy => policy
                .RequireRole("member"))
            .WithErrorMessage(CouldNotCreateTickerMessage);

        resource
            .MapPut("{TickerMessage}", Put)
            .RequireAuthorization(policy => policy
                .RequireRole("member"))
            .WithErrorMessage(CouldNotUpdateTickerMessage);

        resource
            .MapDelete("{TickerMessage}", Delete)
            .RequireAuthorization(policy => policy
                .RequireRole("member"))
            .WithErrorMessage(CouldNotDeleteTickerMessage);

        return endpoints;
    }

    private static Delegate GetAll =>
        (ITickerMessageRequestHandler requestHandler, string query, int skip, int take)
            => requestHandler.GetAll(query, skip, take);

    private static Delegate Get =>
        (ITickerMessageRequestHandler requestHandler, long tickerMessage)
            => requestHandler.GetAsync(tickerMessage);

    private static Delegate Post =>
        (ITickerMessageRequestHandler requestHandler, TickerMessageRequest request)
            => requestHandler.InsertAsync(request);

    private static Delegate Put =>
        (ITickerMessageRequestHandler requestHandler, long tickerMessage, TickerMessageRequest request)
            => requestHandler.UpdateAsync(tickerMessage, request);

    private static Delegate Delete =>
        (ITickerMessageRequestHandler requestHandler, long tickerMessage)
            => requestHandler.DeleteAsync(tickerMessage);
}