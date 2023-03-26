using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Extension;

internal static class ObjectTypeResource
{
    private const string CouldNotQueryObjectTypes = "Could not query object types";
    private const string CouldNotQueryObjectType = "Could not query object type";

    public static IEndpointRouteBuilder MapObjectTypeResource(this IEndpointRouteBuilder endpoints)
    {
        var resource = endpoints
            .MapGroup("/object-types")
            .WithTags("Object Type API")
            .RequireAuthorization(policy => policy
                .RequireClaim("badge", "member")
                .RequireClaim("scope", "endpoints"));

        resource
            .MapGet(string.Empty, GetAll)
            .RequireAuthorization(policy => policy
                .RequireRole("member", "member-observer"))
            .WithErrorMessage(CouldNotQueryObjectTypes);

        resource
            .MapGet("{objectType}", Get)
            .RequireAuthorization(policy => policy
                .RequireRole("member", "member-observer"))
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