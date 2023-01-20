using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Requests;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge.EndpointFilters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

internal static class IdentitySignInEndpoints
{
    private const string CouldNotRegisterIdentity = "Could not register identity";
    private const string CouldNotSignInIdentity = "Could not sign in identity";
    private const string CouldNotResetIdentity = "Could not reset identity";
    private const string CouldNotSignOutIdentity = "Could not sign out identity";
    private const string CouldNotChangeIdentitySecret = "Could not change identity secret";
    private const string CouldNotVerifyIdentity = "Could not verify identity";

    public static IEndpointRouteBuilder MapIdentitySignInEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var identitiesEndpoints = endpoints
            .MapGroup("/identities")
            .WithTags("Identities");

        identitiesEndpoints
            .MapPost("/register", Register)
            .WithErrorMessage(CouldNotRegisterIdentity);

        var identityEndpoints = identitiesEndpoints
            .MapGroup("/{identity}");

        identityEndpoints
            .MapPost("/sign-in", SignIn)
            .WithErrorMessage(CouldNotSignInIdentity)
            .AddEndpointFilter<BadgeResultEndpointFilter>();

        identityEndpoints
            .MapPost("/reset", Reset)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("admin")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotResetIdentity);

        var meEndpoints = identitiesEndpoints
            .MapGroup("/me");

        meEndpoints
            .MapPost("/sign-out", SignOut)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("default")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotSignOutIdentity);

        meEndpoints
            .MapPost("/change-secret", ChangeSecret)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("secure")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotChangeIdentitySecret);

        meEndpoints
            .MapPost("/verify", Verify)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("default")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotVerifyIdentity);

        return endpoints;
    }

    private static Delegate Register =>
        () => Results.StatusCode(StatusCodes.Status501NotImplemented);

    private static Delegate SignIn =>
        (IIdentitySignInRequestHandler requestHandler, string identity, IdentitySignInRequest request)
            => requestHandler.SignIn(identity, request);

    private static Delegate Reset =>
        () => Results.StatusCode(StatusCodes.Status501NotImplemented);

    private static Delegate SignOut =>
        () => Results.StatusCode(StatusCodes.Status501NotImplemented);

    private static Delegate ChangeSecret =>
        () => Results.StatusCode(StatusCodes.Status501NotImplemented);

    private static Delegate Verify =>
        (IIdentitySignInRequestHandler requestHandler)
            => requestHandler.Verify();


}