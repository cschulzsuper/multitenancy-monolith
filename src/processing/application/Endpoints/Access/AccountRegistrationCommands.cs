using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

internal static class AccountRegistrationCommands
{
    private const string CouldNotApproveAccountRegistration = "Could not execute account registration approve";

    public static IEndpointRouteBuilder MapAccountRegistrationCommands(this IEndpointRouteBuilder endpoints)
    {
        var commands = endpoints
            .MapGroup("/account-registrations")
            .WithTags("Account Registration Commands")
            .RequireAuthorization(policy => policy
                .RequireClaim("type", "identity")
                .RequireClaim("scope", "endpoints"));

        commands
            .MapPost("{accountRegistration}/approve", Approve)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
            .WithErrorMessage(CouldNotApproveAccountRegistration);

        return endpoints;
    }

    private static Delegate Approve =>
        (IAccountRegistrationCommandHandler commandHandler, long accountRegistration)
            => commandHandler.ApproveAsync(accountRegistration);
}