using ChristianSchulz.MultitenancyMonolith.Application.Access.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

internal static class AccountRegistrationResource
{
    private const string CouldNotQueryAccountRegistrations = "Could not query account registrations";
    private const string CouldNotQueryAccountRegistration = "Could not query account registration";
    private const string CouldNotCreateAccountRegistration = "Could not create account registration";
    private const string CouldNotUpdateAccountRegistration = "Could not update account registration";
    private const string CouldNotDeleteAccountRegistration = "Could not delete account registration";

    public static IEndpointRouteBuilder MapAccountRegistrationResource(this IEndpointRouteBuilder endpoints)
    {
        var resource = endpoints
            .MapGroup("/account-registrations")
            .WithTags("Account Registration API")
            .RequireAuthorization(policy => policy
                .RequireClaim("badge", "identity")
                .RequireClaim("scope", "endpoints"));

        resource
            .MapGet("{accountRegistration}", Get)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
            .WithErrorMessage(CouldNotQueryAccountRegistration);

        resource
            .MapGet(string.Empty, GetAll)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
            .WithErrorMessage(CouldNotQueryAccountRegistrations);

        resource
            .MapPost(string.Empty, Post)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
            .WithErrorMessage(CouldNotCreateAccountRegistration);

        resource
            .MapPut("{accountRegistration}", Put)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
            .WithErrorMessage(CouldNotUpdateAccountRegistration);

        resource
            .MapDelete("{accountRegistration}", Delete)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
            .WithErrorMessage(CouldNotDeleteAccountRegistration);

        return endpoints;
    }

    private static Delegate GetAll =>
        (IAccountRegistrationRequestHandler requestHandler)
            => requestHandler.GetAll();

    private static Delegate Get =>
        (IAccountRegistrationRequestHandler requestHandler, long accountRegistration)
            => requestHandler.GetAsync(accountRegistration);

    private static Delegate Post =>
        (IAccountRegistrationRequestHandler requestHandler, AccountRegistrationRequest request)
            => requestHandler.InsertAsync(request);

    private static Delegate Put =>
        (IAccountRegistrationRequestHandler requestHandler, long accountRegistration, AccountRegistrationRequest request)
            => requestHandler.UpdateAsync(accountRegistration, request);

    private static Delegate Delete =>
        (IAccountRegistrationRequestHandler requestHandler, long accountRegistration)
            => requestHandler.DeleteAsync(accountRegistration);
}