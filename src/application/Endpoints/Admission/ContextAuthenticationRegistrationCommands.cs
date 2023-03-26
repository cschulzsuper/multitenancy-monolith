using ChristianSchulz.MultitenancyMonolith.Application.Admission.Commands;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

internal static class ContextAuthenticationRegistrationCommands
{
    private const string CouldNotConfirmAuthenticationRegistration = "Could not confirm authentication registration";
    private const string CouldNotRegisterAuthenticationRegistration = "Could not register authentication registration";

    public static IEndpointRouteBuilder MapContextAuthenticationRegistrationCommands(this IEndpointRouteBuilder endpoints)
    {
        var commands = endpoints
            .MapGroup("/authentication-registrations/_")
            .WithTags("Context Authentication Registration Commands");

        commands
            .MapPost("/confirm", Confirm)
            .WithErrorMessage(CouldNotConfirmAuthenticationRegistration);

        commands
            .MapPost("/register", Register)
            .WithErrorMessage(CouldNotRegisterAuthenticationRegistration);

        return endpoints;
    }

    private static Delegate Confirm =>
        (IContextAuthenticationRegistrationCommandHandler commandHandler, ContextAuthenticationRegistrationConfirmCommand command)
            => commandHandler.ConfirmAsync(command);

    private static Delegate Register =>
        (IContextAuthenticationRegistrationCommandHandler commandHandler, ContextAuthenticationRegistrationRegisterCommand command)
            => commandHandler.RegisterAsync(command);
}