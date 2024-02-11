using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies.Documentation;
using ChristianSchulz.MultitenancyMonolith.Objects.Documentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;


namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite.Documentation;

public sealed class DevelopmentPostMapping : IEntityTypeConfiguration<DevelopmentPost>
{
    private readonly IConfigurationProxyProvider _configurationProxyProvider;

    public DevelopmentPostMapping(IConfigurationProxyProvider configurationProxyProvider)
    {
        _configurationProxyProvider = configurationProxyProvider;
    }

    public void Configure(EntityTypeBuilder<DevelopmentPost> builder)
    {
        builder
            .ToTable("documentation-development-post");

        builder
            .HasKey(entity => entity.Snowflake);

        builder
            .Property(entity => entity.Snowflake)
            .HasValueGenerator<SnowflakeGenerator>()
            .ValueGeneratedOnAdd();

        builder
            .HasIndex(entity => entity.Index)
            .IsUnique();

        builder
            .Property(entity => entity.Index)
            .IsRequired();

        builder
            .Property(entity => entity.Project)
            .HasMaxLength(140)
            .IsRequired();

        builder
            .Property(entity => entity.Time)
            .IsRequired();

        builder
            .Property(entity => entity.Title)
            .HasMaxLength(140)
            .IsRequired();

        builder
            .Property(entity => entity.Text)
            .HasMaxLength(4000)
            .IsRequired();

        builder
            .Property(entity => entity.Link)
            .HasMaxLength(140)
            .IsRequired();

        builder
            .Property(entity => entity.Tags)
            .IsRequired();

        // TODO https://github.com/dotnet/efcore/issues/32017
        // builder.HasData(CreateSeed());
    }

    private DevelopmentPost[] CreateSeed()
    {
        var developmentPostSeeds = _configurationProxyProvider
            .GetSeedData<DevelopmentPostSeed>("documentation/development-posts");

        var snowflakeGenerator = new SnowflakeGenerator();

        var developmentPosts = developmentPostSeeds
            .Select(seed => new DevelopmentPost
            {
                Snowflake = snowflakeGenerator.Next(null!),
                Index = Array.IndexOf(developmentPostSeeds, seed),
                Project = seed.Project,
                Title = seed.Title,
                Time = seed.Time,
                Text = seed.Text,
                Link = seed.Link,
                Tags = seed.Tags ?? [],
            })
            .ToArray();

        return developmentPosts;
    }
}