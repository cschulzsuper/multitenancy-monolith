using ChristianSchulz.MultitenancyMonolith.Objects.Documentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite.Documentation;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public static class _Services
{
    public static IServiceCollection AddDataEntityFrameworkSqliteDocumentation(this IServiceCollection services)
    {
        services.AddDbContext<_Context>((provider, options) =>
        {
            var connections = provider.GetRequiredService<SqliteConnections>();
            var connection = connections.Get("documentation");

            options.UseSqlite(connection);
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            options.AddInterceptors(new SqliteSaveChangesExceptionInterceptor());
        });

        services.AddScoped<IRepository<DevelopmentPost>>(p => new Repository<DevelopmentPost>(p.GetRequiredService<_Context>()));

        return services;
    }
}