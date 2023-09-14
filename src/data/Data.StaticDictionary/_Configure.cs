using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies.Access;
using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies.Admission;
using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies.Documentation;
using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies.Ticker;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using ChristianSchulz.MultitenancyMonolith.Objects.Documentation;
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
        var authenticationIdentitySeeds = services
            .GetRequiredService<IConfigurationProxyProvider>()
            .GetSeedData<AuthenticationIdentitySeed>("admission/authentication-identities");

        using var scope = services.CreateScope();

        var authenticationIdentities = authenticationIdentitySeeds
            .Select(seed => new AuthenticationIdentity
            {
                UniqueName = seed.UniqueName,
                MailAddress = seed.MailAddress,
                Secret = seed.Secret
            })
            .ToArray();

        scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationIdentity>>()
            .Insert(authenticationIdentities);

        return services;
    }

    public static IServiceProvider ConfigureAuthenticationIdentityAuthenticationMethods(this IServiceProvider services)
    {
        var authenticationIdentityAuthenticationMethodSeeds = services
            .GetRequiredService<IConfigurationProxyProvider>()
            .GetSeedData<AuthenticationIdentityAuthenticationMethodSeed>("admission/authentication-identity-authentication-methods");

        using var scope = services.CreateScope();

        var authenticationIdentityAuthenticationMethods = authenticationIdentityAuthenticationMethodSeeds
            .Select(seed => new AuthenticationIdentityAuthenticationMethod
            {
                AuthenticationMethod = seed.AuthenticationMethod,
                ClientName = seed.ClientName,
                AuthenticationIdentity = seed.AuthenticationIdentity
            })
            .ToArray();

        scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationIdentityAuthenticationMethod>>()
            .Insert(authenticationIdentityAuthenticationMethods);

        return services;
    }

    public static IServiceProvider ConfigureAccountGroups(this IServiceProvider services)
    {
        var accountGroupSeeds = services
            .GetRequiredService<IConfigurationProxyProvider>()
            .GetSeedData<AccountGroupSeed>("access/account-groups");

        using var scope = services.CreateScope();

        var accountGroups = accountGroupSeeds
            .Select(seed => new AccountGroup
            {
                UniqueName = seed.UniqueName,
            })
            .ToArray();

        scope.ServiceProvider
            .GetRequiredService<IRepository<AccountGroup>>()
            .Insert(accountGroups);

        return services;
    }

    public static IServiceProvider ConfigureAccountMembers(this IServiceProvider services)
    {
        var groupedAccountMemberSeeds = services
            .GetRequiredService<IConfigurationProxyProvider>()
            .GetSeedData<AccountMemberSeed>("access/account-members")
            .GroupBy(x => x.AccountGroup);

        foreach (var accountMemberSeedsGroup in groupedAccountMemberSeeds)
        {
            using var scope = services.CreateMultitenancyScope(accountMemberSeedsGroup.Key);

            var accountMembers = accountMemberSeedsGroup
                .Select(seed => new AccountMember
                {
                    UniqueName = seed.UniqueName,
                    MailAddress = seed.MailAddress,
                    
                    AuthenticationIdentities = seed.AuthenticationIdentities
                        .Select(authenticationIdentity => new AccountMemberAuthenticationIdentity 
                        { 
                            UniqueName = authenticationIdentity 
                        })
                        .ToArray()
                })
                .ToArray();

            scope.ServiceProvider
                .GetRequiredService<IRepository<AccountMember>>()
                .Insert(accountMembers);
        }

        return services;
    }

    public static IServiceProvider ConfigureTickerUsers(this IServiceProvider services)
    {
        var groupedTickerUserSeeds = services
            .GetRequiredService<IConfigurationProxyProvider>()
            .GetSeedData<TickerUserSeed>("ticker/ticker-users")
            .GroupBy(x => x.AccountGroup);

        foreach (var tickerUserSeedsGroup in groupedTickerUserSeeds)
        {
            using var scope = services.CreateMultitenancyScope(tickerUserSeedsGroup.Key);

            var tickerUsers = tickerUserSeedsGroup
                .Select(seed => new TickerUser
                {
                    DisplayName = seed.DisplayName,
                    MailAddress = seed.MailAddress,
                    Secret = seed.Secret,
                    SecretState = seed.SecretState,
                    SecretToken = seed.SecretToken
                })
                .ToArray();

            scope.ServiceProvider
                .GetRequiredService<IRepository<TickerUser>>()
                .Insert(tickerUsers);
        }

        return services;
    }

    public static IServiceProvider ConfigureDevelopmentPosts(this IServiceProvider services)
    {
        var developmentPostSeeds = services
            .GetRequiredService<IConfigurationProxyProvider>()
            .GetSeedData<DevelopmentPostSeed>("documentation/development-posts");

        using var scope = services.CreateScope();

        var developmentPosts = developmentPostSeeds
            .Select(seed => new DevelopmentPost
            {
                Index = Array.IndexOf(developmentPostSeeds, seed),
                Title = seed.Title,
                DateTime = seed.DateTime,
                Text = seed.Text,
                Link = seed.Link,
                Tags = seed.Tags
            })
            .ToArray();

        scope.ServiceProvider
            .GetRequiredService<IRepository<DevelopmentPost>>()
            .Insert(developmentPosts);

        return services;
    }
}