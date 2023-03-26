using ChristianSchulz.MultitenancyMonolith.Application.Extension.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Extension;

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
            .WithTags("Distinction Type Custom Property API")
            .RequireAuthorization(policy => policy
                .RequireClaim("badge", "member")
                .RequireClaim("scope", "endpoints"));

        resource
            .MapGet(string.Empty, GetAll)
            .RequireAuthorization(policy => policy
                .RequireRole("member", "member-observer"))
            .WithErrorMessage(CouldNotQueryDistinctionTypeCustomProperties);

        resource
            .MapGet("{customProperty}", Get)
            .RequireAuthorization(policy => policy
                .RequireRole("member", "member-observer"))
            .WithErrorMessage(CouldNotQueryDistinctionTypeCustomProperty);

        resource
            .MapPost(string.Empty, Post)
            .RequireAuthorization(policy => policy
                .RequireRole("chief"))
            .WithErrorMessage(CouldNotCreateDistinctionTypeCustomProperty);

        resource
            .MapPut("{customProperty}", Put)
            .RequireAuthorization(policy => policy
                .RequireRole("chief"))
            .WithErrorMessage(CouldNotUpdateDistinctionTypeCustomProperty);

        resource
            .MapDelete("{customProperty}", Delete)
            .RequireAuthorization(policy => policy
                .RequireRole("chief"))
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