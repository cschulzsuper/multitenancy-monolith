using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal static class ObjectTypeResource
{
    private const string CouldNotQueryObjectTypes = "Could not query object types";
    private const string CouldNotQueryObjectType = "Could not query object type";

    public static IEndpointRouteBuilder MapObjectTypeResource(this IEndpointRouteBuilder endpoints)
    {
        var resource = endpoints
            .MapGroup("/object-types")
            .WithTags("Object Type API");

        resource
            .MapGet(string.Empty, GetAll)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("member", "observer")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotQueryObjectTypes);

        resource
            .MapGet("{objectType}", Get)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("member", "observer")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotQueryObjectType);

        return endpoints;
    }

    private static Delegate GetAll =>
        (IObjectTypeRequestHandler requestHandler)
            => requestHandler.GetAll();

    private static Delegate Get =>
        (IObjectTypeRequestHandler requestHandler, string objectType)
            => requestHandler.Get(objectType);
}