using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite.Schedule;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public class _Context : DbContext
{
    public _Context(DbContextOptions<_Context> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PlannedJobMapping());
    }
}
