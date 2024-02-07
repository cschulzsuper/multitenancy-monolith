using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using ChristianSchulz.MultitenancyMonolith.Shared.Multitenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite.Access;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public static class _Services
{
    public static IServiceCollection AddDataEntityFrameworkSqliteAccess(this IServiceCollection services)
    {
        services.AddDbContext<_Context>((provider, options) =>
        {
            var connections = provider.GetRequiredService<SqliteConnections>();
            var connection = connections.Get("access");

            options.UseSqlite(connection);
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            options.AddInterceptors(new SqliteSaveChangesExceptionInterceptor());
        });

        services.AddDbContext<_Multitenancy>((provider, options) =>
        {
            var discriminator = provider.GetRequiredService<MultitenancyContext>()
                .MultitenancyDiscriminator;

            var connections = provider.GetRequiredService<SqliteConnections>();
            var connection = connections.Get($"access-{discriminator}");

            options.UseSqlite(connection);
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            options.AddInterceptors(new SqliteSaveChangesExceptionInterceptor());
        });

        services.AddScoped<IRepository<AccountGroup>>(p => new Repository<AccountGroup>(p.GetRequiredService<_Context>()));
        services.AddScoped<IRepository<AccountMember>>(p => new Repository<AccountMember>(p.GetRequiredService<_Multitenancy>()));
        services.AddScoped<IRepository<AccountRegistration>>(p => new Repository<AccountRegistration>(p.GetRequiredService<_Context>()));

        return services;
    }
}