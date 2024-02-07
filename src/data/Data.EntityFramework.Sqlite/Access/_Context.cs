using ChristianSchulz.MultitenancyMonolith.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite.Access;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public class _Context : DbContext
{
    private readonly IConfigurationProxyProvider _configurationProxyProvider;

    public _Context(
        DbContextOptions<_Context> options, 
        IConfigurationProxyProvider configurationProxyProvider)
        : base(options)
    {
        _configurationProxyProvider = configurationProxyProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AccountGroupMapping(_configurationProxyProvider));
        modelBuilder.ApplyConfiguration(new AccountRegistrationMapping());
    }
}
