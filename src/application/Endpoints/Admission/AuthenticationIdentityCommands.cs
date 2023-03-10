using ChristianSchulz.MultitenancyMonolith.Application.Admission.Commands;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

internal static class IdentitySignInEndpoints
{
    private const string CouldNotSignInAuthenticationIdentity = "Could not sign in authentication identity";
    private const string CouldNotVerifyAuthenticationIdentity = "Could not verify authentication identity";

    public static IEndpointRouteBuilder MapAuthenticationIdentityCommands(this IEndpointRouteBuilder endpoints)
    {
        var commands = endpoints
            .MapGroup("/authentication-identities")
            .WithTags("Authentication Identity Commands");

        commands
            .MapPost("/_/auth", Auth)
            .WithErrorMessage(CouldNotSignInAuthenticationIdentity)
            .Authenticates()
            .AddEndpointFilter<BadgeResultEndpointFilter>();

        commands
            .MapPost("/_/verify", Verify)
            .RequireAuthorization(policy => policy
                .RequireClaim("badge", "identity"))
            .WithErrorMessage(CouldNotVerifyAuthenticationIdentity);

        return endpoints;
    }

    private static Delegate Auth =>
        (IContextAuthenticationIdentityCommandHandler commandHandler, ContextAuthenticationIdentityAuthCommand command)
            => commandHandler.AuthAsync(command);
    private static Delegate Verify =>
        (IContextAuthenticationIdentityCommandHandler commandHandler)
            => commandHandler.Verify();

}