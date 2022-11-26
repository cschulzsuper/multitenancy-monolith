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

        return endpoints;
    }

    private static Delegate GetAll =>
        [Authorize(Roles = "Member")]
        (IMemberRequestHandler requestHandler)
            => requestHandler.GetAll();

    private static Delegate Get =>
        [Authorize(Roles = "Member")]
        (IMemberRequestHandler requestHandler, string member)
            => requestHandler.Get(member);
}
