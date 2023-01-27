using ChristianSchulz.MultitenancyMonolith.Application.Administration.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

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
            .WithTags("Distinction Type API");

        resource
            .MapGet(string.Empty, GetAll)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("member", "observer")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotQueryDistinctionTypes);

        resource
            .MapGet("{distinctionType}", Get)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("member", "observer")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotQueryDistinctionType);

        resource
            .MapPost(string.Empty, Post)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("chief")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotCreateDistinctionType);

        resource
            .MapPut("{distinctionType}", Put)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("chief")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotUpdateDistinctionType);

        resource
            .MapDelete("{distinctionType}", Delete)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("chief")
                .RequireClaim("scope", "endpoints"))
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