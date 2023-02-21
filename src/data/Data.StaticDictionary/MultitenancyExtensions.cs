using Microsoft.Extensions.DependencyInjection;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;

public static partial class MultitenancyExtensions
{
    public static IServiceScope CreateMultitenancyScope(this IServiceProvider services, string multitenancyDiscriminator)
    {
        var scope = services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<MultitenancyContext>()
            .MultitenancyDiscriminator = multitenancyDiscriminator;

        return scope;
    }

    public static AsyncServiceScope CreateAsyncMultitenancyScope(this IServiceProvider services, string multitenancyDiscriminator)
    {
        var scope = services.CreateAsyncScope();

        scope.ServiceProvider
            .GetRequiredService<MultitenancyContext>()
            .MultitenancyDiscriminator = multitenancyDiscriminator;

        return scope;
    }

    public static IServiceScope CreateMultitenancyScope(this IServiceScopeFactory services, string multitenancyDiscriminator)
    {
        var scope = services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<MultitenancyContext>()
            .MultitenancyDiscriminator = multitenancyDiscriminator;

        return scope;
    }

    public static AsyncServiceScope CreateAsyncMultitenancyScope(this IServiceScopeFactory services, string multitenancyDiscriminator)
    {
        var scope = services.CreateAsyncScope();

        scope.ServiceProvider
            .GetRequiredService<MultitenancyContext>()
            .MultitenancyDiscriminator = multitenancyDiscriminator;

        return scope;
    }
}