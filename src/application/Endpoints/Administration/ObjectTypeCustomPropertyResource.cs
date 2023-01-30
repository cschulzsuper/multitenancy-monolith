using ChristianSchulz.MultitenancyMonolith.Application.Administration.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal static class ObjectTypeCustomPropertyResource
{
    private const string CouldNotQueryObjectTypeCustomProperties = "Could not query object type custom properties";
    private const string CouldNotQueryObjectTypeCustomProperty = "Could not query object type custom property";
    private const string CouldNotCreateObjectTypeCustomProperty = "Could not create object type custom property";
    private const string CouldNotUpdateObjectTypeCustomProperty = "Could not update object type custom property";
    private const string CouldNotDeleteObjectTypeCustomProperty = "Could not delete object type custom property";

    public static IEndpointRouteBuilder MapObjectTypeCustomPropertyResource(this IEndpointRouteBuilder endpoints)
    {
        var resource = endpoints
            .MapGroup("/object-types/{objectType}/custom-properties")
            .WithTags("Object Type Custom Property API");

        resource
            .MapGet(string.Empty, GetAll)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("member", "observer")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotQueryObjectTypeCustomProperties);

        resource
            .MapGet("{customProperty}", Get)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("member", "observer")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotQueryObjectTypeCustomProperty);

        resource
            .MapPost(string.Empty, Post)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("chief")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotCreateObjectTypeCustomProperty);

        resource
            .MapPut("{customProperty}", Put)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("chief")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotUpdateObjectTypeCustomProperty);

        resource
            .MapDelete("{customProperty}", Delete)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("chief")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotDeleteObjectTypeCustomProperty);

        return endpoints;
    }

    private static Delegate GetAll =>
        (IObjectTypeCustomPropertyRequestHandler requestHandler, string objectType)
            => requestHandler.GetAll(objectType);

    private static Delegate Get =>
        (IObjectTypeCustomPropertyRequestHandler requestHandler, string objectType, string customProperty)
            => requestHandler.GetAsync(objectType, customProperty);

    private static Delegate Post =>
        (IObjectTypeCustomPropertyRequestHandler requestHandler, string objectType, ObjectTypeCustomPropertyRequest request)
            => requestHandler.InsertAsync(objectType, request);

    private static Delegate Put =>
        (IObjectTypeCustomPropertyRequestHandler requestHandler, string objectType, string customProperty, ObjectTypeCustomPropertyRequest request)
            => requestHandler.UpdateAsync(objectType, customProperty, request);

    private static Delegate Delete =>
        (IObjectTypeCustomPropertyRequestHandler requestHandler, string objectType, string customProperty)
            => requestHandler.DeleteAsync(objectType, customProperty);
}