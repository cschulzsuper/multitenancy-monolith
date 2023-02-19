using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Commands;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

internal static class IdentitySignInEndpoints
{
    private const string CouldNotSignInIdentity = "Could not sign in identity";
    private const string CouldNotVerifyIdentity = "Could not verify identity";

    public static IEndpointRouteBuilder MapIdentityCommands(this IEndpointRouteBuilder endpoints)
    {
        var commands = endpoints
            .MapGroup("/identities")
            .WithTags("Identity Commands");

        commands
            .MapPost("/me/auth", Auth)
            .WithErrorMessage(CouldNotSignInIdentity)
            .Authenticates()
            .AddEndpointFilter<BadgeResultEndpointFilter>();

        commands
            .MapPost("/me/verify", Verify)
            .RequireAuthorization(policy => policy
                .RequireClaim("badge", "identity"))
            .WithErrorMessage(CouldNotVerifyIdentity);

        return endpoints;
    }

    private static Delegate Auth =>
        (IIdentityCommandHandler commandHandler, IdentityAuthCommand command)
            => commandHandler.AuthAsync(command);
    private static Delegate Verify =>
        (IIdentityCommandHandler commandHandler)
            => commandHandler.Verify();

}