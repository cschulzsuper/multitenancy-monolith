using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal static class AggregateTypeResource
{
    private const string CouldNotQueryAggregateTypes = "Could not query aggregate types";
    private const string CouldNotQueryAggregateType = "Could not query aggregate type";

    public static IEndpointRouteBuilder MapAggregateTypeResource(this IEndpointRouteBuilder endpoints)
    {
        var resource = endpoints
            .MapGroup("/aggregate-types")
            .WithTags("Aggregate Type API");

        resource
            .MapGet(string.Empty, GetAll)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("member", "observer")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotQueryAggregateTypes);

        resource
            .MapGet("{aggregateType}", Get)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("member", "observer")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotQueryAggregateType);

        return endpoints;
    }

    private static Delegate GetAll =>
        (IAggregateTypeRequestHandler requestHandler)
            => requestHandler.GetAll();

    private static Delegate Get =>
        (IAggregateTypeRequestHandler requestHandler, string aggregateType)
            => requestHandler.Get(aggregateType);
}