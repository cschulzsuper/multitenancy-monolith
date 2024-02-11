using ChristianSchulz.MultitenancyMonolith.Application.Admission.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

internal static class AuthenticationRegistrationResource
{
    private const string CouldNotQueryAuthenticationRegistrations = "Could not query authentication registrations";
    private const string CouldNotQueryAuthenticationRegistration = "Could not query authentication registration";
    private const string CouldNotCreateAuthenticationRegistration = "Could not create authentication registration";
    private const string CouldNotUpdateAuthenticationRegistration = "Could not update authentication registration";
    private const string CouldNotDeleteAuthenticationRegistration = "Could not delete authentication registration";

    public static IEndpointRouteBuilder MapAuthenticationRegistrationResource(this IEndpointRouteBuilder endpoints)
    {
        var resource = endpoints
            .MapGroup("/authentication-registrations")
            .WithTags("Authentication Registration API")
            .RequireAuthorization(policy => policy
                .RequireClaim("type", "identity")
                .RequireClaim("scope", "endpoints"));

        resource
            .MapGet("{authenticationRegistration}", Get)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
            .WithErrorMessage(CouldNotQueryAuthenticationRegistration);

        resource
            .MapGet(string.Empty, GetAll)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
            .WithErrorMessage(CouldNotQueryAuthenticationRegistrations);

        resource
            .MapPost(string.Empty, Post)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
            .WithErrorMessage(CouldNotCreateAuthenticationRegistration);

        resource
            .MapPut("{authenticationRegistration}", Put)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
            .WithErrorMessage(CouldNotUpdateAuthenticationRegistration);

        resource
            .MapDelete("{authenticationRegistration}", Delete)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
            .WithErrorMessage(CouldNotDeleteAuthenticationRegistration);

        return endpoints;
    }

    private static Delegate GetAll =>
        (IAuthenticationRegistrationRequestHandler requestHandler)
            => requestHandler.GetAll();

    private static Delegate Get =>
        (IAuthenticationRegistrationRequestHandler requestHandler, long authenticationRegistration)
            => requestHandler.GetAsync(authenticationRegistration);

    private static Delegate Post =>
        (IAuthenticationRegistrationRequestHandler requestHandler, AuthenticationRegistrationRequest request)
            => requestHandler.InsertAsync(request);

    private static Delegate Put =>
        (IAuthenticationRegistrationRequestHandler requestHandler, long authenticationRegistration, AuthenticationRegistrationRequest request)
            => requestHandler.UpdateAsync(authenticationRegistration, request);

    private static Delegate Delete =>
        (IAuthenticationRegistrationRequestHandler requestHandler, long authenticationRegistration)
            => requestHandler.DeleteAsync(authenticationRegistration);
}