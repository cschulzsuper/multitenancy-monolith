using ChristianSchulz.MultitenancyMonolith.Application.Administration.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal static class MemberEndpoints
{
    public static IEndpointRouteBuilder MapMemberEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var membersEndpoints = endpoints
            .MapGroup("/members")
            .WithTags("Members");

        membersEndpoints.MapGet(string.Empty, GetAll);
        membersEndpoints.MapGet("{member}", Get);
        membersEndpoints.MapPost(string.Empty, Post);
        membersEndpoints.MapPut("{member}", Put);
        membersEndpoints.MapDelete("{member}", Delete);

        return endpoints;
    }

    private static Delegate GetAll =>
        [Authorize(Roles = "Member")]
        (IMemberRequestHandler requestHandler)
            => requestHandler.GetAll();

    private static Delegate Get =>
        [Authorize(Roles = "Member")]
        (IMemberRequestHandler requestHandler, string member)
            => requestHandler.GetAsync(member);

    private static Delegate Post =>
        [Authorize(Roles = "Member")]
        (IMemberRequestHandler requestHandler, MemberRequest request)
            => requestHandler.InsertAsync(request);

    private static Delegate Put =>
        [Authorize(Roles = "Member")]
        (IMemberRequestHandler requestHandler, string member, MemberRequest request)
            => requestHandler.UpdateAsync(member, request);

    private static Delegate Delete =>
        [Authorize(Roles = "Member")]
        (IMemberRequestHandler requestHandler, string member)
            => requestHandler.DeleteAsync(member);
}
