using ChristianSchulz.MultitenancyMonolith.Objects.Documentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite.Documentation;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddDataEntityFrameworkSqliteDocumentation(this IServiceCollection services)
    {
        services.AddDbContext<_Context>((provider, options) =>
        {
            var connectionAccessor = provider.GetRequiredService<SqliteConnectionAccessor>();
            var connection = connectionAccessor.Connection;

            options.UseSqlite(connection);
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        services.AddScoped<IRepository<DevelopmentPost>>(p => new SqliteRepository<DevelopmentPost>(p.GetRequiredService<_Context>()));

        return services;
    }
}