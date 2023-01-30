using ChristianSchulz.MultitenancyMonolith.Application.Administration.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal static class DistinctionTypeCustomPropertyResource
{
    private const string CouldNotQueryDistinctionTypeCustomProperties = "Could not query distinction type custom properties";
    private const string CouldNotQueryDistinctionTypeCustomProperty = "Could not query distinction type custom property";
    private const string CouldNotCreateDistinctionTypeCustomProperty = "Could not create distinction type custom property";
    private const string CouldNotUpdateDistinctionTypeCustomProperty = "Could not update distinction type custom property";
    private const string CouldNotDeleteDistinctionTypeCustomProperty = "Could not delete distinction type custom property";

    public static IEndpointRouteBuilder MapDistinctionTypeCustomPropertyResource(this IEndpointRouteBuilder endpoints)
    {
        var resource = endpoints
            .MapGroup("/distinction-types/{distinctionType}/custom-properties")
            .WithTags("Distinction Type Custom Property API");

        resource
            .MapGet(string.Empty, GetAll)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("member", "observer")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotQueryDistinctionTypeCustomProperties);

        resource
            .MapGet("{customProperty}", Get)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("member", "observer")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotQueryDistinctionTypeCustomProperty);

        resource
            .MapPost(string.Empty, Post)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("chief")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotCreateDistinctionTypeCustomProperty);

        resource
            .MapPut("{customProperty}", Put)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("chief")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotUpdateDistinctionTypeCustomProperty);

        resource
            .MapDelete("{customProperty}", Delete)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("chief")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotDeleteDistinctionTypeCustomProperty);

        return endpoints;
    }

    private static Delegate GetAll =>
        (IDistinctionTypeCustomPropertyRequestHandler requestHandler, string distinctionType)
            => requestHandler.GetAll(distinctionType);

    private static Delegate Get =>
        (IDistinctionTypeCustomPropertyRequestHandler requestHandler, string distinctionType, string customProperty)
            => requestHandler.GetAsync(distinctionType, customProperty);

    private static Delegate Post =>
        (IDistinctionTypeCustomPropertyRequestHandler requestHandler, string distinctionType, DistinctionTypeCustomPropertyRequest request)
            => requestHandler.InsertAsync(distinctionType, request);

    private static Delegate Put =>
        (IDistinctionTypeCustomPropertyRequestHandler requestHandler, string distinctionType, string customProperty, DistinctionTypeCustomPropertyRequest request)
            => requestHandler.UpdateAsync(distinctionType, customProperty, request);

    private static Delegate Delete =>
        (IDistinctionTypeCustomPropertyRequestHandler requestHandler, string distinctionType, string customProperty)
            => requestHandler.DeleteAsync(distinctionType, customProperty);
}