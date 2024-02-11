using ChristianSchulz.MultitenancyMonolith.Application.Extension.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Extension;

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
            .WithTags("Object Type Custom Property API")
            .RequireAuthorization(policy => policy
                .RequireClaim("type", "member")
                .RequireClaim("scope", "endpoints"));

        resource
            .MapGet(string.Empty, GetAll)
            .RequireAuthorization(policy => policy
                .RequireRole("member", "member-observer"))
            .WithErrorMessage(CouldNotQueryObjectTypeCustomProperties);

        resource
            .MapGet("{customProperty}", Get)
            .RequireAuthorization(policy => policy
                .RequireRole("member", "member-observer"))
            .WithErrorMessage(CouldNotQueryObjectTypeCustomProperty);

        resource
            .MapPost(string.Empty, Post)
            .RequireAuthorization(policy => policy
                .RequireRole("chief"))
            .WithErrorMessage(CouldNotCreateObjectTypeCustomProperty);

        resource
            .MapPut("{customProperty}", Put)
            .RequireAuthorization(policy => policy
                .RequireRole("chief"))
            .WithErrorMessage(CouldNotUpdateObjectTypeCustomProperty);

        resource
            .MapDelete("{customProperty}", Delete)
            .RequireAuthorization(policy => policy
                .RequireRole("chief"))
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