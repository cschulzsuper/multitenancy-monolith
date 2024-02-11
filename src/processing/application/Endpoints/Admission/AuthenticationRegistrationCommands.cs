using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

internal static class AuthenticationRegistrationCommands
{
    private const string CouldNotApproveAuthenticationRegistration = "Could not execute authentication registration approve";

    public static IEndpointRouteBuilder MapAuthenticationRegistrationCommands(this IEndpointRouteBuilder endpoints)
    {
        var commands = endpoints
            .MapGroup("/authentication-registrations")
            .WithTags("Authentication Registration Commands")
            .RequireAuthorization(policy => policy
                .RequireClaim("type", "identity")
                .RequireClaim("scope", "endpoints"));

        commands
            .MapPost("{authenticationRegistration}/approve", Approve)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
            .WithErrorMessage(CouldNotApproveAuthenticationRegistration);

        return endpoints;
    }

    private static Delegate Approve =>
        (IAuthenticationRegistrationCommandHandler commandHandler, long authenticationRegistration)
            => commandHandler.ApproveAsync(authenticationRegistration);
}