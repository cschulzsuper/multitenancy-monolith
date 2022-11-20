using ChristianSchulz.MultitenancyMonolith.Application.Administration;
using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Request;
using ChristianSchulz.MultitenancyMonolith.Shared.Authentication.Badge.EndpointFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public static class AuthenticationEndpoints
{
    public static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var identitiesEndpoints = endpoints
            .MapGroup("/identities")
            .WithTags("Identities");

        identitiesEndpoints.MapPost("/register", Register);

        var identityEndpoints = identitiesEndpoints
            .MapGroup("/{identity}");

        identityEndpoints.MapPost("/sign-in", SignIn).AddEndpointFilter<BadgeResultEndpointFilter>();
        identityEndpoints.MapPost("/reset", Reset);

        var meEndpoints = identitiesEndpoints
            .MapGroup("/me");

        meEndpoints.MapPost("/sign-out", SignOut);
        meEndpoints.MapPost("/change-secret", ChangeSecret);
        meEndpoints.MapPost("/verify", Verify);

        return endpoints;
    }

    private static Delegate Register =>
        () => Results.StatusCode(StatusCodes.Status501NotImplemented);

    private static Delegate SignIn =>
        (IAuthenticationRequestHandler requestHandler, string identity, SignInRequest request)
            => requestHandler.SignIn(identity, request);

    private static Delegate Reset =>
        [Authorize]
        () => Results.StatusCode(StatusCodes.Status501NotImplemented);

    private static Delegate SignOut =>
        [Authorize]
        () => Results.StatusCode(StatusCodes.Status501NotImplemented);

    private static Delegate ChangeSecret =>
        [Authorize]
        () => Results.StatusCode(StatusCodes.Status501NotImplemented);

    private static Delegate Verify =>
        [Authorize]
        (IAuthenticationRequestHandler requestHandler)
            => requestHandler.Verify();


}
