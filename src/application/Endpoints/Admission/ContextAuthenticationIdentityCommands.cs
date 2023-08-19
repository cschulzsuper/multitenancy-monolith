using ChristianSchulz.MultitenancyMonolith.Application.Access;
using ChristianSchulz.MultitenancyMonolith.Application.Admission.Commands;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Diagnostics;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

internal static class ContextAuthenticationIdentityCommands
{
    private const string CouldNotSignInAuthenticationIdentity = "Could not execute authentication identity sign in";
    private const string CouldNotVerifyAuthenticationIdentity = "Could not execute authentication identity verify";

    public static IEndpointRouteBuilder MapContextAuthenticationIdentityCommands(this IEndpointRouteBuilder endpoints)
    {
        var commands = endpoints
            .MapGroup("/authentication-identities/_")
            .WithTags("Context Authentication Identity Commands");

        commands
            .MapPost("/auth", Auth)
            .WithErrorMessage(CouldNotSignInAuthenticationIdentity)
            .Authenticates();

        commands
            .MapPost("/verify", Verify)
            .RequireAuthorization(policy => policy
                .RequireClaim("type", "identity")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotVerifyAuthenticationIdentity);

        return endpoints;
    }

    private static Delegate Auth =>
        async (IContextAuthenticationIdentityCommandHandler commandHandler, ContextAuthenticationIdentityAuthCommand command) =>
        {
            var response = await commandHandler.AuthAsync(command) as ClaimsPrincipal;
            if (response == null)
            {
                throw new UnreachableException($"{nameof(IContextAccountMemberCommandHandler.AuthAsync)} response is not a {nameof(ClaimsPrincipal)}");
            }

            return Results.SignIn(response, authenticationScheme: BearerTokenDefaults.AuthenticationScheme);
        };

    private static Delegate Verify =>
        (IContextAuthenticationIdentityCommandHandler commandHandler)
            => commandHandler.VerifyAsync();

}