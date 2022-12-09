using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Data;

public static partial class _Services
{
    public static IServiceCollection AddData(this IServiceCollection services)
    {
        services.AddSingleton<SnowflakeGenerator>();

        services.AddSingleton(typeof(RepositoryContextFactory<>));
        services.AddScoped<MultitenancyContext>();

        services.AddScoped(GroupRepositoryFactory);
        services.AddScoped(MemberRepositoryFactory);
        services.AddScoped(MembershipRepositoryFactory);
        services.AddScoped(IdentityRepositoryFactory);

        return services;
    }

}
