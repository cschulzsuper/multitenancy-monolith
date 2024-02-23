using ChristianSchulz.MultitenancyMonolith.Data.EntityFramework;
using ChristianSchulz.MultitenancyMonolith.Objects.Extension;
using ChristianSchulz.MultitenancyMonolith.Objects.Schedule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite.Extension;

public sealed class ObjectTypeMapping : IEntityTypeConfiguration<ObjectType>
{

    public void Configure(EntityTypeBuilder<ObjectType> builder)
    {
        builder
            .ToTable("extension-object-type");

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
            .OwnsMany(entity => entity.CustomProperties,
                customPropertiesBuilder =>
                {
                    // TODO For now this does not work! (https://github.com/dotnet/efcore/issues/29380, https://github.com/dotnet/efcore/issues/28594)

                    customPropertiesBuilder.ToJson();

                    customPropertiesBuilder
                        .HasIndex(entity => entity.UniqueName)
                        .IsUnique();

                    customPropertiesBuilder
                        .Property(entity => entity.UniqueName)
                        .HasMaxLength(140)
                        .IsRequired();

                    customPropertiesBuilder
                        .Property(entity => entity.DisplayName)
                        .HasMaxLength(140)
                        .IsRequired();

                    customPropertiesBuilder
                        .HasIndex(entity => entity.PropertyName)
                        .IsUnique();

                    customPropertiesBuilder
                        .Property(entity => entity.PropertyName)
                        .HasMaxLength(140)
                        .IsRequired();

                    // TODO Maybe use a check constratint to limit the expression type to certain values

                    customPropertiesBuilder
                        .Property(entity => entity.PropertyType)
                        .HasMaxLength(140)
                        .IsRequired();

                });

    }
}