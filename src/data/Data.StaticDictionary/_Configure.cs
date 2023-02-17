using ChristianSchulz.MultitenancyMonolith.Objects.Authentication;
using ChristianSchulz.MultitenancyMonolith.Objects.Authorization;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Configure
{
    private const string IdentitiesConfigurationKey
        = "SeedData:Authentication:Identities";

    private const string MembersConfigurationKey
        = "SeedData:Administration:Members";

    private const string TickerUsersConfigurationKey
        = "SeedData:Ticker:TickerUsers";

    public static IServiceProvider ConfigureIdentities(this IServiceProvider services)
    {
        var identities = services
                             .GetRequiredService<IConfiguration>()
                             .GetSection(IdentitiesConfigurationKey)?
                             .Get<Identity[]>();

        if (identities == null)
        {
            return services;
        }

        using var scope = services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IRepository<Identity>>()
            .Insert(identities);

        return services;
    }

    public static IServiceProvider ConfigureGroups(this IServiceProvider services)
    {
        var configuration = services.GetRequiredService<IConfiguration>();

        var groupSections = configuration.GetSection(MembersConfigurationKey)?.GetChildren();
        if (groupSections == null)
        {
            return services;
        }

        var groups = groupSections
            .Select(group => new Group {UniqueName = group.Key})
            .ToArray();

        using var scope = services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IRepository<Group>>()
            .Insert(groups);

        return services;
    }

    public static IServiceProvider ConfigureMembers(this IServiceProvider services)
    {
        var configuration = services.GetRequiredService<IConfiguration>();

        var groupSections = configuration.GetSection(MembersConfigurationKey)?.GetChildren();
        if (groupSections == null)
        {
            return services;
        }

        var groupedMembers = groupSections
            .ToDictionary(
                group => group.Key,
                group => group.Get<Member[]>() ?? throw new UnreachableException($"Could not get `{MembersConfigurationKey}` configuration for group `{group.Key}`"));

        foreach (var group in groupedMembers.Keys)
        {
            var members = groupedMembers[group];

            using var scope = services.CreateMultitenancyScope(group);

            scope.ServiceProvider
                .GetRequiredService<IRepository<Member>>()
                .Insert(members);
        }

        return services;
    }

    public static IServiceProvider ConfigureTickerUsers(this IServiceProvider services)
    {
        var configuration = services.GetRequiredService<IConfiguration>();

        var groupSections = configuration.GetSection(TickerUsersConfigurationKey)?.GetChildren();
        if (groupSections == null)
        {
            return services;
        }

        var groupedTickerUsers = groupSections
            .ToDictionary(
                group => group.Key,
                group => group.Get<TickerUser[]>() ?? throw new UnreachableException($"Could not get `{TickerUsersConfigurationKey}` configuration for group `{group.Key}`"));

        foreach (var group in groupedTickerUsers.Keys)
        {
            var tickerUsers = groupedTickerUsers[group];

            using var scope = services.CreateMultitenancyScope(group);

            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .Insert(tickerUsers);
        }

        return services;
    }
}