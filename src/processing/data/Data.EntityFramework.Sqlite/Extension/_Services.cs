using ChristianSchulz.MultitenancyMonolith.Objects.Extension;
using ChristianSchulz.MultitenancyMonolith.Objects.Schedule;
using ChristianSchulz.MultitenancyMonolith.Shared.Multitenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite.Extension;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public static class _Services
{
    public static IServiceCollection AddDataEntityFrameworkSqliteExtension(this IServiceCollection services)
    {
        services.AddDbContext<_Multitenancy>((provider, options) =>
        {
            var discriminator = provider.GetRequiredService<MultitenancyContext>()
                .MultitenancyDiscriminator;

            var connections = provider.GetRequiredService<SqliteConnections>();
            var connection = connections.Get($"extension-{discriminator}");

            options.UseSqlite(connection);
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            options.AddInterceptors(new SqliteSaveChangesExceptionInterceptor());
        });

        services.AddScoped<IRepository<DistinctionType>>(p => new Repository<DistinctionType>(p.GetRequiredService<_Multitenancy>()));
        services.AddScoped<IRepository<ObjectType>>(p => new Repository<ObjectType>(p.GetRequiredService<_Multitenancy>()));

        return services;
    }
}