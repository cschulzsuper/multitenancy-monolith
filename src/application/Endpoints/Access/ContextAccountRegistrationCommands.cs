using ChristianSchulz.MultitenancyMonolith.Application.Access.Commands;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

internal static class ContextAccountRegistrationCommands
{

    private const string CouldNotConfirmAccountRegistration = "Could not execute account registration confirm";
    private const string CouldNotRegisterAccountRegistration = "Could not execute account registration register";

    public static IEndpointRouteBuilder MapContextAccountRegistrationCommands(this IEndpointRouteBuilder endpoints)
    {
        var commands = endpoints
            .MapGroup("/account-registrations/_")
            .WithTags("Context Account Registration Commands")
            .RequireAuthorization(policy => policy
                .RequireClaim("type", "identity", "member"));

        commands
            .MapPost("/confirm", Confirm)
            .WithErrorMessage(CouldNotConfirmAccountRegistration);

        commands
            .MapPost("/register", Register)
            .WithErrorMessage(CouldNotRegisterAccountRegistration);

        return endpoints;
    }

    private static Delegate Confirm =>
        (IContextAccountRegistrationCommandHandler commandHandler, ContextAccountRegistrationConfirmCommand command)
            => commandHandler.ConfirmAsync(command);

    private static Delegate Register =>
        (IContextAccountRegistrationCommandHandler commandHandler, ContextAccountRegistrationRegisterCommand command)
            => commandHandler.RegisterAsync(command);
}