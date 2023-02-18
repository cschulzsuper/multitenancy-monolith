using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Objects.Authentication;
using ChristianSchulz.MultitenancyMonolith.Objects.Authorization;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Configure
{
    public static IServiceProvider ConfigureIdentities(this IServiceProvider services)
    {
        var @objects = services
            .GetRequiredService<ISeedDataProvider>()
            .Get<Identity>("Authentication", "Identities");

        using var scope = services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IRepository<Identity>>()
            .Insert(@objects);

        return services;
    }

    public static IServiceProvider ConfigureGroups(this IServiceProvider services)
    {
        var groups = services
            .GetRequiredService<ISeedDataProvider>()
            .GetGrouped<Member>("Administration", "Members")
            .Select(x => x.Key)
            .Distinct();

        var @objects = groups
            .Select(group => new Group {UniqueName = group})
            .ToArray();

        using var scope = services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IRepository<Group>>()
            .Insert(@objects);

        return services;
    }

    public static IServiceProvider ConfigureMembers(this IServiceProvider services)
    {
        var groupedMembers = services
            .GetRequiredService<ISeedDataProvider>()
            .GetGrouped<Member>("Administration", "Members");

        foreach (var group in groupedMembers)
        {
            using var scope = services.CreateMultitenancyScope(group.Key);

            scope.ServiceProvider
                .GetRequiredService<IRepository<Member>>()
                .Insert(group.Value);
        }

        return services;
    }

    public static IServiceProvider ConfigureTickerUsers(this IServiceProvider services)
    {
        var groupedTickerUsers = services
            .GetRequiredService<ISeedDataProvider>()
            .GetGrouped<TickerUser>("Ticker", "TickerUsers");

        foreach (var group in groupedTickerUsers)
        {
            using var scope = services.CreateMultitenancyScope(group.Key);

            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .Insert(group.Value);
        }

        return services;
    }
}