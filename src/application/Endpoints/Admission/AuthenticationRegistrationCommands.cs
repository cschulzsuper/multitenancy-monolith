using ChristianSchulz.MultitenancyMonolith.Application.Admission.Commands;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

internal static class AuthenticationRegistrationCommands
{
    private const string CouldNotRegisterAuthenticationRegistration = "Could not register authentication registration";

    public static IEndpointRouteBuilder MapAuthenticationRegistrationCommands(this IEndpointRouteBuilder endpoints)
    {
        var commands = endpoints
            .MapGroup("/authentication-registrations")
            .WithTags("Authentication Registration Commands");

        commands
            .MapPost("/_/register", Register)
            .WithErrorMessage(CouldNotRegisterAuthenticationRegistration);

        return endpoints;
    }

    private static Delegate Register =>
        (IContextAuthenticationRegistrationCommandHandler commandHandler, ContextAuthenticationRegistrationRegisterCommand command)
            => commandHandler.RegisterAsync(command);
}