using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

internal static class GroupCommands
{
    private const string CouldNotRegisterGroup = "Could not register group";

    public static IEndpointRouteBuilder MapGroupCommands(this IEndpointRouteBuilder endpoints)
    {
        var commands = endpoints
            .MapGroup("/groups")
            .WithTags("Group Commands");

        commands
            .MapPost("/register", Register)
            .RequireAuthorization(ploicy => ploicy
                .RequireRole("admin")
                .RequireClaim("scope", "endpoints"))
            .WithErrorMessage(CouldNotRegisterGroup);

        return endpoints;
    }

    private static Delegate Register =>
        () => Results.StatusCode(StatusCodes.Status501NotImplemented);
}