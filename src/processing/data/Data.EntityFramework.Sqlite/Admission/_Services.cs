using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite.Admission;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public static class _Services
{
    public static IServiceCollection AddDataEntityFrameworkSqliteAdmission(this IServiceCollection services)
    {
        services.AddDbContext<_Context>((provider, options) =>
        {
            var connections = provider.GetRequiredService<SqliteConnections>();
            var connection = connections.Get("admission");

            options.UseSqlite(connection);
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            options.AddInterceptors(new SqliteSaveChangesExceptionInterceptor());
        });

        services.AddScoped<IRepository<AuthenticationIdentityAuthenticationMethod>>(p 
            => new Repository<AuthenticationIdentityAuthenticationMethod>(p.GetRequiredService<_Context>()));

        services.AddScoped<IRepository<AuthenticationIdentity>>(p 
            => new Repository<AuthenticationIdentity>(p.GetRequiredService<_Context>()));

        services.AddScoped<IRepository<AuthenticationRegistration>>(p 
            => new Repository<AuthenticationRegistration>(p.GetRequiredService<_Context>()));

        return services;
    }
}