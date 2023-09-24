using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite.Admission;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddDataEntityFrameworkSqliteAdmission(this IServiceCollection services)
    {
        services.AddDbContext<_Context>((provider, options) =>
        {
            var connectionAccessor = provider.GetRequiredService<SqliteConnectionAccessor>();
            var connection = connectionAccessor.Connection;

            options.UseSqlite(connection);
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        services.AddScoped<IRepository<AuthenticationIdentityAuthenticationMethod>>(p => new Repository<AuthenticationIdentityAuthenticationMethod>(p.GetRequiredService<_Context>()));
        services.AddScoped<IRepository<AuthenticationIdentity>>(p => new Repository<AuthenticationIdentity>(p.GetRequiredService<_Context>()));
        services.AddScoped<IRepository<AuthenticationRegistration>>(p => new Repository<AuthenticationRegistration>(p.GetRequiredService<_Context>()));

        return services;
    }
}