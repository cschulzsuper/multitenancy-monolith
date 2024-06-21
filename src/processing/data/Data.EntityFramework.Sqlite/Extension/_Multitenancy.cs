using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Shared.Multitenancy;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite.Extension;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public class _Multitenancy : DbContext
{
    public _Multitenancy(DbContextOptions<_Multitenancy> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new DistinctionTypeMapping());
        modelBuilder.ApplyConfiguration(new ObjectTypeMapping());
    }
}
