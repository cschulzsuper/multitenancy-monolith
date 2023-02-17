using ChristianSchulz.MultitenancyMonolith.Application.Authorization.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

internal static class MemberResource
{
    private const string CouldNotQueryMembers = "Could not query members";
    private const string CouldNotQueryMember = "Could not query member";
    private const string CouldNotCreateMember = "Could not create member";
    private const string CouldNotUpdateMember = "Could not update member";
    private const string CouldNotDeleteMember = "Could not delete member";

    public static IEndpointRouteBuilder MapMemberResource(this IEndpointRouteBuilder endpoints)
    {
        var resource = endpoints
            .MapGroup("/members")
            .WithTags("Member API")
            .RequireAuthorization(policy => policy
                .RequireClaim("badge", "member")
                .RequireClaim("scope", "endpoints"));

        resource
            .MapGet(string.Empty, GetAll)
            .RequireAuthorization(policy => policy
                .RequireRole("chief", "chief-observer"))
            .WithErrorMessage(CouldNotQueryMembers);

        resource
            .MapGet("{member}", Get)
            .RequireAuthorization(policy => policy
                .RequireRole("chief", "chief-observer"))
            .WithErrorMessage(CouldNotQueryMember);

        resource
            .MapPost(string.Empty, Post)
            .RequireAuthorization(policy => policy
                .RequireRole("chief"))
            .WithErrorMessage(CouldNotCreateMember);

        resource
            .MapPut("{member}", Put)
            .RequireAuthorization(policy => policy
                .RequireRole("chief"))
            .WithErrorMessage(CouldNotUpdateMember);

        resource
            .MapDelete("{member}", Delete)
            .RequireAuthorization(policy => policy
                .RequireRole("chief"))
            .WithErrorMessage(CouldNotDeleteMember);

        return endpoints;
    }

    private static Delegate GetAll =>
        (IMemberRequestHandler requestHandler)
            => requestHandler.GetAll();

    private static Delegate Get =>
        (IMemberRequestHandler requestHandler, string member)
            => requestHandler.GetAsync(member);

    private static Delegate Post =>
        (IMemberRequestHandler requestHandler, MemberRequest request)
            => requestHandler.InsertAsync(request);

    private static Delegate Put =>
        (IMemberRequestHandler requestHandler, string member, MemberRequest request)
            => requestHandler.UpdateAsync(member, request);

    private static Delegate Delete =>
        (IMemberRequestHandler requestHandler, string member)
            => requestHandler.DeleteAsync(member);
}