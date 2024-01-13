using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies.Access;
using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies.Admission;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using ChristianSchulz.MultitenancyMonolith.Shared.Multitenancy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Server.Data;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public static class _Configure
{
    public static IServiceProvider ConfigureAuthenticationIdentities(this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var configured = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationIdentity>>()
            .GetQueryable().Any();

        if (configured) return services;

        var authenticationIdentitySeeds = services
            .GetRequiredService<IConfigurationProxyProvider>()
            .GetSeedData<AuthenticationIdentitySeed>("admission/authentication-identities");

        var authenticationIdentities = authenticationIdentitySeeds
            .Select(seed => new AuthenticationIdentity
            {
                Snowflake = Array.IndexOf(authenticationIdentitySeeds, seed),
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
        using var scope = services.CreateScope();

        var configured = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationIdentityAuthenticationMethod>>()
            .GetQueryable().Any();

        if (configured) return services;

        var authenticationIdentities = scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationIdentity>>()
            .GetQueryable();

        var authenticationIdentityAuthenticationMethodSeeds = services
            .GetRequiredService<IConfigurationProxyProvider>()
            .GetSeedData<AuthenticationIdentityAuthenticationMethodSeed>("admission/authentication-identity-authentication-methods");

        var authenticationIdentityAuthenticationMethods = authenticationIdentityAuthenticationMethodSeeds
            .Select(seed => new AuthenticationIdentityAuthenticationMethod
            {
                AuthenticationMethod = seed.AuthenticationMethod,
                ClientName = seed.ClientName,
                AuthenticationIdentity = authenticationIdentities.Single(x => x.UniqueName == seed.AuthenticationIdentity).Snowflake
            })
            .ToArray();

        scope.ServiceProvider
            .GetRequiredService<IRepository<AuthenticationIdentityAuthenticationMethod>>()
            .Insert(authenticationIdentityAuthenticationMethods);

        return services;
    }

    public static IServiceProvider ConfigureAccountGroups(this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var configured = scope.ServiceProvider
            .GetRequiredService<IRepository<AccountGroup>>()
            .GetQueryable().Any();

        if (configured) return services;

        var accountGroupSeeds = services
            .GetRequiredService<IConfigurationProxyProvider>()
            .GetSeedData<AccountGroupSeed>("access/account-groups");

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

            var configured = scope.ServiceProvider
                .GetRequiredService<IRepository<AccountMember>>()
                .GetQueryable().Any();

            if (configured) continue;

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
}