using ChristianSchulz.MultitenancyMonolith.Application.Access.Commands;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

internal static class AccountRegistrationCommands
{
    private const string CouldNotRegisterAccountRegistration = "Could not register account registration";

    public static IEndpointRouteBuilder MapAccountRegistrationCommands(this IEndpointRouteBuilder endpoints)
    {
        var commands = endpoints
            .MapGroup("/account-registrations")
            .WithTags("Account Registration Commands");

        commands
            .MapPost("/_/register", Register)
            .RequireAuthorization(policy => policy
                .RequireClaim("badge", "identity", "member"))
            .WithErrorMessage(CouldNotRegisterAccountRegistration);

        return endpoints;
    }

    private static Delegate Register =>
        (IContextAccountRegistrationCommandHandler commandHandler, ContextAccountRegistrationRegisterCommand command)
            => commandHandler.RegisterAsync(command);
}