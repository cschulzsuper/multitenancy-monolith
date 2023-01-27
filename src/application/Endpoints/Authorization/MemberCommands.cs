using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

internal static class MemberCommands
{
    private const string CouldNotVerifyMember = "Could not verify member";

    public static IEndpointRouteBuilder MapMemberCommands(this IEndpointRouteBuilder endpoints)
    {
        var commands = endpoints
            .MapGroup("/members")
            .WithTags("Member Commands");

        commands
            .MapPost("/me/verify", Verify)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("member", "observer")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotVerifyMember);

        return endpoints;
    }

    private static Delegate Verify =>
        (IMemberCommandHandler requestHandler)
            => requestHandler.Verify();
}