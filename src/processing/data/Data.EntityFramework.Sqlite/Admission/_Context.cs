using ChristianSchulz.MultitenancyMonolith.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite.Admission;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public class _Context : DbContext
{
    private readonly IConfigurationProxyProvider _configurationProxyProvider;

    public _Context(DbContextOptions<_Context> options, IConfigurationProxyProvider configurationProxyProvider)
        : base(options)
    {
        _configurationProxyProvider = configurationProxyProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AuthenticationIdentityAuthenticationMethodMapping(_configurationProxyProvider));
        modelBuilder.ApplyConfiguration(new AuthenticationIdentityMapping(_configurationProxyProvider));
        modelBuilder.ApplyConfiguration(new AuthenticationRegistrationMapping());
    }
}
