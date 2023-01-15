using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

internal static class IdentityEndpoints
{
    private const string CouldNotQueryIdentities = "Could not query identities";
    private const string CouldNotQueryIdentity = "Could not query identity";
    private const string CouldNotCreateIdentity = "Could not create identity";
    private const string CouldNotUpdateIdentity = "Could not update identity";
    private const string CouldNotDeleteIdentity = "Could not delete identity";

    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var identitiesEndpoints = endpoints
            .MapGroup("/identities")
            .WithTags("Identities");

        identitiesEndpoints
            .MapGet(string.Empty, GetAll)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("admin")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotQueryIdentities);

        identitiesEndpoints
            .MapGet("{identity}", Get)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("admin")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotQueryIdentity);

        identitiesEndpoints
            .MapPost(string.Empty, Post)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("admin")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotCreateIdentity);

        identitiesEndpoints
            .MapPut("{identity}", Put)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("admin")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotUpdateIdentity);

        identitiesEndpoints
            .MapDelete("{identity}", Delete)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("admin")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotDeleteIdentity);

        return endpoints;
    }

    private static Delegate GetAll =>
        (IIdentityRequestHandler requestHandler) 
            => requestHandler.GetAll();

    private static Delegate Get =>
        (IIdentityRequestHandler requestHandler, string identity)
            => requestHandler.GetAsync(identity);

    private static Delegate Post =>
        (IIdentityRequestHandler requestHandler, IdentityRequest request)
            => requestHandler.InsertAsync(request);

    private static Delegate Put =>
        (IIdentityRequestHandler requestHandler, string identity, IdentityRequest request)
            => requestHandler.UpdateAsync(identity, request);

    private static Delegate Delete =>
        (IIdentityRequestHandler requestHandler, string identity)
            => requestHandler.DeleteAsync(identity);
}
