using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies.Admission;
using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite.Admission;

public sealed class AuthenticationIdentityMapping : IEntityTypeConfiguration<AuthenticationIdentity>
{
    private readonly IConfigurationProxyProvider _configurationProxyProvider;

    public AuthenticationIdentityMapping(IConfigurationProxyProvider configurationProxyProvider)
    {
        _configurationProxyProvider = configurationProxyProvider;
    }
    public void Configure(EntityTypeBuilder<AuthenticationIdentity> builder)
    {
        builder
            .ToTable("admission-account-identity");

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
            .Property(entity => entity.Secret)
            .HasMaxLength(140)
            .IsRequired();

        // TODO https://github.com/dotnet/efcore/issues/32017
        // builder.HasData(CreateSeed());
    }

    [SuppressMessage("CodeQuality", "IDE0051:UnusedPrivateMember")]
    private AuthenticationIdentity[] CreateSeed()
    {
        var authenticationIdentitySeeds = _configurationProxyProvider
            .GetSeedData<AuthenticationIdentitySeed>("admission/authentication-identities");

        var authenticationIdentities = authenticationIdentitySeeds
            .Select(seed => new AuthenticationIdentity
            {
                Snowflake = 1 + Array.IndexOf(authenticationIdentitySeeds, seed),
                UniqueName = seed.UniqueName,
                MailAddress = seed.MailAddress,
                Secret = seed.Secret
            })
            .ToArray();

        return authenticationIdentities;
    }
}