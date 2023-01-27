using ChristianSchulz.MultitenancyMonolith.Application.Administration.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal static class AggregateTypeCustomPropertyResource
{
    private const string CouldNotQueryAggregateTypeCustomProperties = "Could not query aggregate type custom properties";
    private const string CouldNotQueryAggregateTypeCustomProperty = "Could not query aggregate type custom property";
    private const string CouldNotCreateAggregateTypeCustomProperty = "Could not create aggregate type custom property";
    private const string CouldNotUpdateAggregateTypeCustomProperty = "Could not update aggregate type custom property";
    private const string CouldNotDeleteAggregateTypeCustomProperty = "Could not delete aggregate type custom property";

    public static IEndpointRouteBuilder MapAggregateTypeCustomPropertyResource(this IEndpointRouteBuilder endpoints)
    {
        var resource = endpoints
            .MapGroup("/aggregate-types/{aggregateType}/custom-properties")
            .WithTags("Aggregate Type Custom Property API");

        resource
            .MapGet(string.Empty, GetAll)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("member", "observer")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotQueryAggregateTypeCustomProperties);

        resource
            .MapGet("{customProperty}", Get)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("member", "observer")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotQueryAggregateTypeCustomProperty);

        resource
            .MapPost(string.Empty, Post)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("chief")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotCreateAggregateTypeCustomProperty);

        resource
            .MapPut("{customProperty}", Put)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("chief")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotUpdateAggregateTypeCustomProperty);

        resource
            .MapDelete("{customProperty}", Delete)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("chief")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotDeleteAggregateTypeCustomProperty);

        return endpoints;
    }

    private static Delegate GetAll =>
        (IAggregateTypeCustomPropertyRequestHandler requestHandler, string aggregateType)
            => requestHandler.GetAll(aggregateType);

    private static Delegate Get =>
        (IAggregateTypeCustomPropertyRequestHandler requestHandler, string aggregateType, string customProperty)
            => requestHandler.GetAsync(aggregateType, customProperty);

    private static Delegate Post =>
        (IAggregateTypeCustomPropertyRequestHandler requestHandler, string aggregateType, AggregateTypeCustomPropertyRequest request)
            => requestHandler.InsertAsync(aggregateType, request);

    private static Delegate Put =>
        (IAggregateTypeCustomPropertyRequestHandler requestHandler, string aggregateType, string customProperty, AggregateTypeCustomPropertyRequest request)
            => requestHandler.UpdateAsync(aggregateType, customProperty, request);

    private static Delegate Delete =>
        (IAggregateTypeCustomPropertyRequestHandler requestHandler, string aggregateType, string customProperty)
            => requestHandler.DeleteAsync(aggregateType, customProperty);
}