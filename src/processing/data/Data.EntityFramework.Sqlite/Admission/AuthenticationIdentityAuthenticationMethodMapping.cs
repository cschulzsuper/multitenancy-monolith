using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies.Admission;
using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite.Admission;

public sealed class AuthenticationIdentityAuthenticationMethodMapping : IEntityTypeConfiguration<AuthenticationIdentityAuthenticationMethod>
{
    private readonly IConfigurationProxyProvider _configurationProxyProvider;

    public AuthenticationIdentityAuthenticationMethodMapping(IConfigurationProxyProvider configurationProxyProvider)
    {
        _configurationProxyProvider = configurationProxyProvider;
    }

    public void Configure(EntityTypeBuilder<AuthenticationIdentityAuthenticationMethod> builder)
    {
        builder
            .ToTable("admission-account-identity-authentication-method");

        builder
            .HasKey(entity => entity.Snowflake);

        builder
            .Property(entity => entity.Snowflake)
            .HasValueGenerator<SnowflakeGenerator>()
            .ValueGeneratedOnAdd();

        builder
            .HasIndex(entity => new { entity.ClientName, entity.AuthenticationIdentity, entity.AuthenticationMethod })
            .IsUnique();

        builder
            .Property(entity => entity.ClientName)
            .HasMaxLength(140)
            .IsRequired();

        builder
            .Property(entity => entity.AuthenticationIdentity)
            .IsRequired();

        builder
            .Property(entity => entity.AuthenticationMethod)
            .HasMaxLength(140)
            .IsRequired();

        // TODO https://github.com/dotnet/efcore/issues/32017
        // builder.HasData(CreateSeed());
    }

    [SuppressMessage("CodeQuality", "IDE0051:UnusedPrivateMember")]
    private AuthenticationIdentityAuthenticationMethod[] CreateSeed()
    {
        var authenticationIdentitySeeds = _configurationProxyProvider
            .GetSeedData<AuthenticationIdentitySeed>("admission/authentication-identities");

        var authenticationIdentityAuthenticationMethodSeeds = _configurationProxyProvider
            .GetSeedData<AuthenticationIdentityAuthenticationMethodSeed>("admission/authentication-identity-authentication-methods");

        var snowflakeGenerator = new SnowflakeGenerator();

        var authenticationIdentityAuthenticationMethods = authenticationIdentityAuthenticationMethodSeeds
            .Select(seed => new AuthenticationIdentityAuthenticationMethod
            {
                Snowflake = snowflakeGenerator.Next(null!),
                ClientName = seed.ClientName,
                AuthenticationMethod = seed.AuthenticationMethod,
                AuthenticationIdentity = 1 + Array.IndexOf(
                    authenticationIdentitySeeds,
                    authenticationIdentitySeeds.Single(x => x.UniqueName == seed.AuthenticationIdentity))
            })
            .ToArray();

        return authenticationIdentityAuthenticationMethods;
    }
}