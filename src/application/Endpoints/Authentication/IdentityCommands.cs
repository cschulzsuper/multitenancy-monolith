using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Commands;
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

    public static IEndpointRouteBuilder MapIdentityCommands(this IEndpointRouteBuilder endpoints)
    {
        var commands = endpoints
            .MapGroup("/identities")
            .WithTags("Identity Commands");

        commands
            .MapPost("/register", Register)
            .WithErrorMessage(CouldNotRegisterIdentity);

        commands
            .MapPost("/{identity}/sign-in", SignIn)
            .WithErrorMessage(CouldNotSignInIdentity)
            .AddEndpointFilter<BadgeResultEndpointFilter>();

        commands
            .MapPost("/{identity}/reset", Reset)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("admin")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotResetIdentity);

        commands
            .MapPost("/me/sign-out", SignOut)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("default")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotSignOutIdentity);

        commands
            .MapPost("/me/change-secret", ChangeSecret)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("secure")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotChangeIdentitySecret);

        commands
            .MapPost("/me/verify", Verify)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("default")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotVerifyIdentity);

        return endpoints;
    }

    private static Delegate Register =>
        () => Results.StatusCode(StatusCodes.Status501NotImplemented);

    private static Delegate SignIn =>
        (IIdentityCommandHandler commandHandler, string identity, IdentitySignInCommand command)
            => commandHandler.SignIn(identity, command);

    private static Delegate Reset =>
        () => Results.StatusCode(StatusCodes.Status501NotImplemented);

    private static Delegate SignOut =>
        () => Results.StatusCode(StatusCodes.Status501NotImplemented);

    private static Delegate ChangeSecret =>
        () => Results.StatusCode(StatusCodes.Status501NotImplemented);

    private static Delegate Verify =>
        (IIdentityCommandHandler commandHandler)
            => commandHandler.Verify();

}