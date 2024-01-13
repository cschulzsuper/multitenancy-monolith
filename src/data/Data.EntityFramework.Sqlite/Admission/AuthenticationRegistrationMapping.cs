using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite.Admission;

public sealed class AuthenticationRegistrationMapping : IEntityTypeConfiguration<AuthenticationRegistration>
{
    public void Configure(EntityTypeBuilder<AuthenticationRegistration> builder)
    {
        builder
            .ToTable("admission-authentication-registration");

        builder
            .HasKey(entity => entity.Snowflake);

        builder
            .Property(entity => entity.Snowflake)
            .HasValueGenerator<SnowflakeGenerator>()
            .ValueGeneratedOnAdd();

        builder
            .HasIndex(entity => entity.AuthenticationIdentity)
            .IsUnique();

        builder
            .Property(entity => entity.AuthenticationIdentity)
            .HasMaxLength(140)
            .IsRequired();

        builder
            .Property(entity => entity.MailAddress)
            .HasMaxLength(254)
            .IsRequired();

        builder
            .Property(entity => entity.ProcessToken)
            .IsRequired();

        // TODO Maybe use a check constratint to limit the process state to certain values

        builder
            .Property(entity => entity.ProcessState)
            .HasMaxLength(140)
            .IsRequired();

        builder
            .Property(entity => entity.Secret)
            .HasMaxLength(254)
            .IsRequired();
    }
}