﻿using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddAdmissionTransport(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationIdentityCommandHandler, AuthenticationIdentityCommandHandler>();
        services.AddScoped<IAuthenticationIdentityRequestHandler, AuthenticationIdentityRequestHandler>();

        return services;
    }
}