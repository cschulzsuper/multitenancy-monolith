using ChristianSchulz.MultitenancyMonolith.Objects.Schedule;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite.Schedule;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public static class _Services
{
    public static IServiceCollection AddDataEntityFrameworkSqliteSchedule(this IServiceCollection services)
    {
        services.AddDbContext<_Context>((provider, options) =>
        {
            var connections = provider.GetRequiredService<SqliteConnections>();
            var connection = connections.Get("schedule");

            options.UseSqlite(connection);
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            options.AddInterceptors(new SqliteSaveChangesExceptionInterceptor());
        });

        services.AddScoped<IRepository<PlannedJob>>(p => new Repository<PlannedJob>(p.GetRequiredService<_Context>()));

        return services;
    }
}