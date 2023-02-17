using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

internal static class IdentityResource
{
    private const string CouldNotQueryIdentities = "Could not query identities";
    private const string CouldNotQueryIdentity = "Could not query identity";
    private const string CouldNotCreateIdentity = "Could not create identity";
    private const string CouldNotUpdateIdentity = "Could not update identity";
    private const string CouldNotDeleteIdentity = "Could not delete identity";

    public static IEndpointRouteBuilder MapIdentityResource(this IEndpointRouteBuilder endpoints)
    {
        var resource = endpoints
            .MapGroup("/identities")
            .WithTags("Identity API")
            .RequireAuthorization(policy => policy
                .RequireClaim("badge", "identity")
                .RequireClaim("scope", "endpoints"));

        resource
            .MapGet(string.Empty, GetAll)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
            .WithErrorMessage(CouldNotQueryIdentities);

        resource
            .MapGet("{identity}", Get)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
            .WithErrorMessage(CouldNotQueryIdentity);

        resource
            .MapPost(string.Empty, Post)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
            .WithErrorMessage(CouldNotCreateIdentity);

        resource
            .MapPut("{identity}", Put)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
            .WithErrorMessage(CouldNotUpdateIdentity);

        resource
            .MapDelete("{identity}", Delete)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
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