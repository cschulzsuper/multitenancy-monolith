using ChristianSchulz.MultitenancyMonolith.Application.Extension.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Extension;

internal static class DistinctionTypeResource
{
    private const string CouldNotQueryDistinctionTypes = "Could not query distinction types";
    private const string CouldNotQueryDistinctionType = "Could not query distinction type";
    private const string CouldNotCreateDistinctionType = "Could not create distinction type";
    private const string CouldNotUpdateDistinctionType = "Could not update distinction type";
    private const string CouldNotDeleteDistinctionType = "Could not delete distinction type";

    public static IEndpointRouteBuilder MapDistinctionTypeResource(this IEndpointRouteBuilder endpoints)
    {
        var resource = endpoints
            .MapGroup("/distinction-types")
            .WithTags("Distinction Type API")
            .RequireAuthorization(policy => policy
                .RequireClaim("type", "member")
                .RequireClaim("scope", "endpoints"));

        resource
            .MapGet(string.Empty, GetAll)
            .RequireAuthorization(policy => policy
                .RequireRole("member", "member-observer"))
            .WithErrorMessage(CouldNotQueryDistinctionTypes);

        resource
            .MapGet("{distinctionType}", Get)
            .RequireAuthorization(policy => policy
                .RequireRole("member", "member-observer"))
            .WithErrorMessage(CouldNotQueryDistinctionType);

        resource
            .MapPost(string.Empty, Post)
            .RequireAuthorization(policy => policy
                .RequireRole("chief"))
            .WithErrorMessage(CouldNotCreateDistinctionType);

        resource
            .MapPut("{distinctionType}", Put)
            .RequireAuthorization(policy => policy
                .RequireRole("chief"))
            .WithErrorMessage(CouldNotUpdateDistinctionType);

        resource
            .MapDelete("{distinctionType}", Delete)
            .RequireAuthorization(policy => policy
                .RequireRole("chief"))
            .WithErrorMessage(CouldNotDeleteDistinctionType);

        return endpoints;
    }

    private static Delegate GetAll =>
        (IDistinctionTypeRequestHandler requestHandler)
            => requestHandler.GetAll();

    private static Delegate Get =>
        (IDistinctionTypeRequestHandler requestHandler, string distinctionType)
            => requestHandler.GetAsync(distinctionType);

    private static Delegate Post =>
        (IDistinctionTypeRequestHandler requestHandler, DistinctionTypeRequest request)
            => requestHandler.InsertAsync(request);

    private static Delegate Put =>
        (IDistinctionTypeRequestHandler requestHandler, string distinctionType, DistinctionTypeRequest request)
            => requestHandler.UpdateAsync(distinctionType, request);

    private static Delegate Delete =>
        (IDistinctionTypeRequestHandler requestHandler, string distinctionType)
            => requestHandler.DeleteAsync(distinctionType);
}