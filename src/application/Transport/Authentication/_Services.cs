﻿using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public static class _Services
{
    public static IServiceCollection AddAuthenticationTransport(this IServiceCollection services)
    {
        services.AddScoped<IIdentityCommandHandler, IdentityCommandHandler>();
        services.AddScoped<IIdentityRequestHandler, IdentityRequestHandler>();

        return services;
    }
}