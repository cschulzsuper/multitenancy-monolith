using ChristianSchulz.MultitenancyMonolith.Application.Access;
using ChristianSchulz.MultitenancyMonolith.Application.Admission.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

internal static class AuthenticationIdentityResource
{
    private const string CouldNotQueryAuthenticationIdentities = "Could not query authentication identities";
    private const string CouldNotQueryAuthenticationIdentity = "Could not query authentication identity";
    private const string CouldNotCreateAuthenticationIdentity = "Could not create authentication identity";
    private const string CouldNotUpdateAuthenticationIdentity = "Could not update authentication identity";
    private const string CouldNotDeleteAuthenticationIdentity = "Could not delete authentication identity";

    public static IEndpointRouteBuilder MapAuthenticationIdentityResource(this IEndpointRouteBuilder endpoints)
    {
        var resource = endpoints
            .MapGroup("/authentication-identities")
            .WithTags("Authentication Identity API")
            .RequireAuthorization(policy => policy
                .RequireClaim("badge", "identity")
                .RequireClaim("scope", "endpoints"));

        resource
            .MapMethods("{authenticationIdentity}", new[] { HttpMethods.Head }, Head)
            .AllowAnonymous()
            .WithErrorMessage(CouldNotQueryAuthenticationIdentity);

        resource
            .MapGet("{authenticationIdentity}", Get)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
            .WithErrorMessage(CouldNotQueryAuthenticationIdentity);

        resource
            .MapGet(string.Empty, GetAll)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
            .WithErrorMessage(CouldNotQueryAuthenticationIdentities);

        resource
            .MapPost(string.Empty, Post)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
            .WithErrorMessage(CouldNotCreateAuthenticationIdentity);

        resource
            .MapPut("{authenticationIdentity}", Put)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
            .WithErrorMessage(CouldNotUpdateAuthenticationIdentity);

        resource
            .MapDelete("{authenticationIdentity}", Delete)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
            .WithErrorMessage(CouldNotDeleteAuthenticationIdentity);

        return endpoints;
    }

    private static Delegate Head =>
        (IAuthenticationIdentityRequestHandler requestHandler, string authenticationIdentity)
            => requestHandler.HeadAsync(authenticationIdentity);

    private static Delegate GetAll =>
        (IAuthenticationIdentityRequestHandler requestHandler)
            => requestHandler.GetAll();

    private static Delegate Get =>
        (IAuthenticationIdentityRequestHandler requestHandler, string authenticationIdentity)
            => requestHandler.GetAsync(authenticationIdentity);

    private static Delegate Post =>
        (IAuthenticationIdentityRequestHandler requestHandler, AuthenticationIdentityRequest request)
            => requestHandler.InsertAsync(request);

    private static Delegate Put =>
        (IAuthenticationIdentityRequestHandler requestHandler, string authenticationIdentity, AuthenticationIdentityRequest request)
            => requestHandler.UpdateAsync(authenticationIdentity, request);

    private static Delegate Delete =>
        (IAuthenticationIdentityRequestHandler requestHandler, string authenticationIdentity)
            => requestHandler.DeleteAsync(authenticationIdentity);
}