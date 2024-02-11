using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies.Access;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite.Access;

public sealed class AccountGroupMapping : IEntityTypeConfiguration<AccountGroup>
{
    private readonly IConfigurationProxyProvider _configurationProxyProvider;

    public AccountGroupMapping(IConfigurationProxyProvider configurationProxyProvider)
    {
        _configurationProxyProvider = configurationProxyProvider;
    }
    public void Configure(EntityTypeBuilder<AccountGroup> builder)
    {
        builder
            .ToTable("access-account-group");

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

        // TODO https://github.com/dotnet/efcore/issues/32017
        // builder.HasData(CreateSeed());
    }


    [SuppressMessage("CodeQuality", "IDE0051:UnusedPrivateMember")]
    private AccountGroup[] CreateSeed()
    {
        var accountGroupSeeds = _configurationProxyProvider
            .GetSeedData<AccountGroupSeed>("access/account-groups");

        var accountGroups = accountGroupSeeds
            .Select(seed => new AccountGroup
            {
                Snowflake = 1 + Array.IndexOf(accountGroupSeeds, seed),
                UniqueName = seed.UniqueName
            })
            .ToArray();

        return accountGroups;
    }
}