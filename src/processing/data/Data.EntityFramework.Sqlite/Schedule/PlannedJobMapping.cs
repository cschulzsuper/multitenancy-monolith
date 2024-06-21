using ChristianSchulz.MultitenancyMonolith.Objects.Schedule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite.Schedule;

public sealed class PlannedJobMapping : IEntityTypeConfiguration<PlannedJob>
{
    public void Configure(EntityTypeBuilder<PlannedJob> builder)
    {
        builder
            .ToTable("schedule-planned-job");

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
            .Property(entity => entity.ExpressionType)
            .HasMaxLength(140)
            .IsRequired();

        builder
            .Property(entity => entity.Expression)
            .HasMaxLength(4000)
            .IsRequired();
    }
}