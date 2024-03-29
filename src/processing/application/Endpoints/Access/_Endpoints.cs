﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public static class _Endpoints
{
    public static IEndpointRouteBuilder MapAccessEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var authorization = endpoints
            .MapGroup("access")
            .WithGroupName("access");

        authorization.MapAccountGroupResource();
        authorization.MapAccountMemberResource();
        authorization.MapAccountRegistrationCommands();
        authorization.MapAccountRegistrationResource();
        authorization.MapContextAccountMemberCommands();
        authorization.MapContextAccountRegistrationCommands();

        return endpoints;
    }
}