using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies.Documentation;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Documentation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.DevLog.Data;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public static class _Configure
{
    public static IServiceProvider ConfigureDevelopmentPosts(this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var configured = scope.ServiceProvider
            .GetRequiredService<IRepository<DevelopmentPost>>()
            .GetQueryable().Any();

        if (configured) return services;

        var developmentPostSeeds = services
            .GetRequiredService<IConfigurationProxyProvider>()
            .GetSeedData<DevelopmentPostSeed>("documentation/development-posts");

        var developmentPosts = developmentPostSeeds
            .Select(seed => new DevelopmentPost
            {
                Index = Array.IndexOf(developmentPostSeeds, seed),
                Project = seed.Project,
                Title = seed.Title,
                Time = seed.Time,
                Text = seed.Text,
                Link = seed.Link,
                Tags = seed.Tags ?? [],
            })
            .ToArray();

        scope.ServiceProvider
            .GetRequiredService<IRepository<DevelopmentPost>>()
            .Insert(developmentPosts);

        return services;
    }
}