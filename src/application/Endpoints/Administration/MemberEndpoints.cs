using ChristianSchulz.MultitenancyMonolith.Application.Administration.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal static class MemberEndpoints
{
    private const string CouldNotQueryMembers = "Could not query members";
    private const string CouldNotQueryMember = "Could not query member";
    private const string CouldNotCreateMember = "Could not create member";
    private const string CouldNotUpdateMember = "Could not update member";
    private const string CouldNotDeleteMember = "Could not delete member";

    public static IEndpointRouteBuilder MapMemberEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var membersEndpoints = endpoints
            .MapGroup("/members")
            .WithTags("Member API");

        membersEndpoints
            .MapGet(string.Empty, GetAll)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("chief", "observer")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotQueryMembers);

        membersEndpoints
            .MapGet("{member}", Get)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("chief", "observer")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotQueryMember);

        membersEndpoints
            .MapPost(string.Empty, Post)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("chief")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotCreateMember);

        membersEndpoints
            .MapPut("{member}", Put)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("chief")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotUpdateMember);

        membersEndpoints
            .MapDelete("{member}", Delete)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("chief")
                .RequireClaim("scope", "endpoints"))
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