using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Configure
{
    public static IServiceProvider ConfigureAuthenticationIdentities(this IServiceProvider services)
    {
        var authenticationIdentities = services
            .GetRequiredService<ISeedDataProvider>()
            .Get<AuthenticationIdentity>("Admission", "AuthenticationIdentities");

        using var scope = services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationIdentity>>()
            .Insert(authenticationIdentities);

        return services;
    }

    public static IServiceProvider ConfigureAccountGroups(this IServiceProvider services)
    {
        var accountGroups = services
            .GetRequiredService<ISeedDataProvider>()
            .GetGrouped<AccountMember>("Access", "AccountMembers")
            .Select(x => x.Key)
            .Distinct();

        var @objects = accountGroups
            .Select(group => new AccountGroup { UniqueName = group })
            .ToArray();

        using var scope = services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IRepository<AccountGroup>>()
            .Insert(@objects);

        return services;
    }

    public static IServiceProvider ConfigureAccountMembers(this IServiceProvider services)
    {
        var groupedAccountMembers = services
            .GetRequiredService<ISeedDataProvider>()
            .GetGrouped<AccountMember>("Access", "AccountMembers");

        foreach (var accountGroup in groupedAccountMembers)
        {
            using var scope = services.CreateMultitenancyScope(accountGroup.Key);

            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountMember>>()
                .Insert(accountGroup.Value);
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