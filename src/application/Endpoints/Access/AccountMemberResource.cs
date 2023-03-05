using ChristianSchulz.MultitenancyMonolith.Application.Access.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

internal static class AccountMemberResource
{
    private const string CouldNotQueryAccountMembers = "Could not query account members";
    private const string CouldNotQueryAccountMember = "Could not query account member";
    private const string CouldNotCreateAccountMember = "Could not create account member";
    private const string CouldNotUpdateAccountMember = "Could not update account member";
    private const string CouldNotDeleteAccountMember = "Could not delete account member";

    public static IEndpointRouteBuilder MapAccountMemberResource(this IEndpointRouteBuilder endpoints)
    {
        var resource = endpoints
            .MapGroup("/account-members")
            .WithTags("Account Member API")
            .RequireAuthorization(policy => policy
                .RequireClaim("badge", "member")
                .RequireClaim("scope", "endpoints"));

        resource
            .MapGet(string.Empty, GetAll)
            .RequireAuthorization(policy => policy
                .RequireRole("chief", "chief-observer"))
            .WithErrorMessage(CouldNotQueryAccountMembers);

        resource
            .MapGet("{accountMember}", Get)
            .RequireAuthorization(policy => policy
                .RequireRole("chief", "chief-observer"))
            .WithErrorMessage(CouldNotQueryAccountMember);

        resource
            .MapPost(string.Empty, Post)
            .RequireAuthorization(policy => policy
                .RequireRole("chief"))
            .WithErrorMessage(CouldNotCreateAccountMember);

        resource
            .MapPut("{accountMember}", Put)
            .RequireAuthorization(policy => policy
                .RequireRole("chief"))
            .WithErrorMessage(CouldNotUpdateAccountMember);

        resource
            .MapDelete("{accountMember}", Delete)
            .RequireAuthorization(policy => policy
                .RequireRole("chief"))
            .WithErrorMessage(CouldNotDeleteAccountMember);

        return endpoints;
    }

    private static Delegate GetAll =>
        (IAccountMemberRequestHandler requestHandler)
            => requestHandler.GetAll();

    private static Delegate Get =>
        (IAccountMemberRequestHandler requestHandler, string accountMember)
            => requestHandler.GetAsync(accountMember);

    private static Delegate Post =>
        (IAccountMemberRequestHandler requestHandler, AccountMemberRequest request)
            => requestHandler.InsertAsync(request);

    private static Delegate Put =>
        (IAccountMemberRequestHandler requestHandler, string accountMember, AccountMemberRequest request)
            => requestHandler.UpdateAsync(accountMember, request);

    private static Delegate Delete =>
        (IAccountMemberRequestHandler requestHandler, string accountMember)
            => requestHandler.DeleteAsync(accountMember);
}