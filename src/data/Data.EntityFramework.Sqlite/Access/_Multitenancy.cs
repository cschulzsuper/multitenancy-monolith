using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Shared.Multitenancy;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite.Access;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public class _Multitenancy : DbContext
{
    private readonly IConfigurationProxyProvider _configurationProxyProvider;
    private readonly MultitenancyContext _multitenancyContext;

    internal MultitenancyContext MultitenancyContext => _multitenancyContext;

    public _Multitenancy(
        DbContextOptions<_Multitenancy> options, 
        IConfigurationProxyProvider configurationProxyProvider,
        MultitenancyContext multitenancyContext)
        : base(options)
    {
        _configurationProxyProvider = configurationProxyProvider;
        _multitenancyContext = multitenancyContext;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AccountMemberMapping(_configurationProxyProvider, _multitenancyContext));
    }
}
