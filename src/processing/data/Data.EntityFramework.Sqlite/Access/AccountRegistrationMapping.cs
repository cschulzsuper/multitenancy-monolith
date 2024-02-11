using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite.Access;

public sealed class AccountRegistrationMapping : IEntityTypeConfiguration<AccountRegistration>
{
    public void Configure(EntityTypeBuilder<AccountRegistration> builder)
    {
        builder
            .ToTable("access-account-registration");

        builder
            .HasKey(entity => entity.Snowflake);

        builder
            .Property(entity => entity.Snowflake)
            .HasValueGenerator<SnowflakeGenerator>()
            .ValueGeneratedOnAdd();

        builder
            .HasIndex(entity => entity.AccountGroup)
            .IsUnique();

        builder
            .Property(entity => entity.AccountGroup)
            .HasMaxLength(140)
            .IsRequired();

        builder
            .Property(entity => entity.AccountMember)
            .HasMaxLength(140)
            .IsRequired();

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
    }
}