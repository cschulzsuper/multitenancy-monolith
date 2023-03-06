using ChristianSchulz.MultitenancyMonolith.Application.Access.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

internal static class AccountGroupResource
{
    private const string CouldNotQueryAccountGroups = "Could not query account groups";
    private const string CouldNotQueryAccountGroup = "Could not query account group";
    private const string CouldNotCreateAccountGroup = "Could not create account group";
    private const string CouldNotUpdateAccountGroup = "Could not update account group";
    private const string CouldNotDeleteAccountGroup = "Could not delete account group";

    public static IEndpointRouteBuilder MapAccountGroup(this IEndpointRouteBuilder endpoints)
    {
        var resource = endpoints
            .MapGroup("/account-groups")
            .WithTags("Account Group API")
            .RequireAuthorization(policy => policy
                .RequireClaim("badge", "identity")
                .RequireClaim("scope", "endpoints"));

        resource
            .MapGet(string.Empty, GetAll)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
            .WithErrorMessage(CouldNotQueryAccountGroups);

        resource
            .MapGet("{accountGroup}", Get)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
            .WithErrorMessage(CouldNotQueryAccountGroup);

        resource
            .MapPost(string.Empty, Post)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
            .WithErrorMessage(CouldNotCreateAccountGroup);

        resource
            .MapPut("{accountGroup}", Put)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
            .WithErrorMessage(CouldNotUpdateAccountGroup);

        resource
            .MapDelete("{accountGroup}", Delete)
            .RequireAuthorization(policy => policy
                .RequireRole("admin"))
            .WithErrorMessage(CouldNotDeleteAccountGroup);

        return endpoints;
    }

    private static Delegate GetAll =>
        (IAccountGroupRequestHandler requestHandler)
            => requestHandler.GetAll();

    private static Delegate Get =>
        (IAccountGroupRequestHandler requestHandler, string accountGroup)
            => requestHandler.GetAsync(accountGroup);

    private static Delegate Post =>
        (IAccountGroupRequestHandler requestHandler, AccountGroupRequest request)
            => requestHandler.InsertAsync(request);

    private static Delegate Put =>
        (IAccountGroupRequestHandler requestHandler, string accountGroup, AccountGroupRequest request)
            => requestHandler.UpdateAsync(accountGroup, request);

    private static Delegate Delete =>
        (IAccountGroupRequestHandler requestHandler, string accountGroup)
            => requestHandler.DeleteAsync(accountGroup);
}