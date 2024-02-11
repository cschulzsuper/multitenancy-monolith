using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public static class _Services
{
    public static IServiceCollection AddDataEntityFrameworkSqlite(this IServiceCollection services)
    {
        services.AddSingleton<SqliteConnections>();

        return services;
    }
}