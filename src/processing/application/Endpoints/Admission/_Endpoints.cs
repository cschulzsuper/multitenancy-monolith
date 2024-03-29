﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public static class _Endpoints
{
    public static IEndpointRouteBuilder MapAdmissionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var authentication = endpoints
            .MapGroup("admission")
            .WithGroupName("admission");

        authentication.MapAuthenticationIdentityResource();
        authentication.MapAuthenticationRegistrationCommands();
        authentication.MapAuthenticationRegistrationResource();
        authentication.MapContextAuthenticationIdentityCommands();
        authentication.MapContextAuthenticationRegistrationCommands();

        return endpoints;
    }
}