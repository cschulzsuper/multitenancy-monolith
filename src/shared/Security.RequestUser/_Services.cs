using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Security.RequestUser;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public static class _Services
{
    public static IServiceCollection AddRequestUser(this IServiceCollection services, Action<RequestUserOptions> setup)
    {
        services.Configure(setup);

        services.AddScoped<RequestUserContext>();
        services.AddScoped<IClaimsTransformation, RequestUserTransformation>();

        services.AddTransient(provider => provider.GetRequiredService<RequestUserContext>().User);

        return services;
    }
}