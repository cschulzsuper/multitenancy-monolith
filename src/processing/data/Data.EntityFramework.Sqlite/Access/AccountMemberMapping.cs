using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies.Access;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using ChristianSchulz.MultitenancyMonolith.Shared.Multitenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite.Access;

public sealed class AccountMemberMapping : IEntityTypeConfiguration<AccountMember>
{
    private readonly IConfigurationProxyProvider _configurationProxyProvider;
    private readonly MultitenancyContext _multitenancyContext;

    public AccountMemberMapping(
        IConfigurationProxyProvider configurationProxyProvider,
        MultitenancyContext multitenancyContext)
    {
        _configurationProxyProvider = configurationProxyProvider;
        _multitenancyContext = multitenancyContext;
    }
    public void Configure(EntityTypeBuilder<AccountMember> builder)
    {
        builder
            .ToTable($"access-account-member");

        builder
            .HasKey(entity => entity.Snowflake);

        builder
            .Property(entity => entity.Snowflake)
            .HasValueGenerator<SnowflakeGenerator>()
            .ValueGeneratedOnAdd();

        builder
            .HasIndex(entity => entity.UniqueName)
            .IsUnique();

        builder
            .Property(entity => entity.UniqueName)
            .HasMaxLength(140)
            .IsRequired();

        builder
            .Property(entity => entity.MailAddress)
            .HasMaxLength(254)
            .IsRequired();

        builder
            .OwnsMany(entity => entity.AuthenticationIdentities, builder => {

                builder.ToJson();

                builder
                    .HasIndex(entity => entity.UniqueName)
                    .IsUnique();

                builder
                    .Property(entity => entity.UniqueName)
                    .HasMaxLength(140)
                    .IsRequired();
            });

        // TODO https://github.com/dotnet/efcore/issues/32017
        // builder.HasData(CreateSeed());
    }

    [SuppressMessage("CodeQuality", "IDE0051:UnusedPrivateMember")]
    private AccountMember[] CreateSeed()
    {
        var accountMemberSeeds = _configurationProxyProvider
            .GetSeedData<AccountMemberSeed>("access/account-members");

        var accountGroupAccountMemberSeeds = accountMemberSeeds
            .Where(accountMemberSeed => accountMemberSeed.AccountGroup == _multitenancyContext.MultitenancyDiscriminator);

        var accountMembers = accountMemberSeeds
            .Select(seed => new AccountMember
            {
                Snowflake = 1 + Array.IndexOf(accountMemberSeeds, seed),
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

        return accountMembers;
    }
}