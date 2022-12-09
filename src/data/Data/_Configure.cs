using ChristianSchulz.MultitenancyMonolith.Application.Administration;
using ChristianSchulz.MultitenancyMonolith.Application.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Data;

public static class _Configure
{
    private const string IdentitiesConfigurationKey
        = "SeedData:Authentication:Identities";

    private const string MembershipsConfigurationKey
        = "SeedData:Administration:Memberships";

    private const string MembersConfigurationKey 
        = "SeedData:Administration:Members";

    public static IServiceProvider ConfigureData(this IServiceProvider services)
    {
        services.ConfigureIdentities();
        services.ConfigureMemberships();
        services.ConfigureMembers();

        return services;
    }

    public static IServiceProvider ConfigureIdentities(this IServiceProvider services) 
    {
        var identities = services
            .GetRequiredService<IConfiguration>()
            .GetRequiredSection(IdentitiesConfigurationKey)
            .Get<Identity[]>()

            ?? throw new RepositoryException($"Could not get `{IdentitiesConfigurationKey}` configuration");

        using var scope = services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IRepository<Identity>>()
            .InsertMany(identities);

        return services;
    }

    public static IServiceProvider ConfigureMemberships(this IServiceProvider services)
    {
        var memberships = services
            .GetRequiredService<IConfiguration>()
            .GetRequiredSection(MembershipsConfigurationKey)
            .Get<Membership[]>()

            ?? throw new RepositoryException($"Could not get `{MembershipsConfigurationKey}` configuration");

        using var scope = services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<IRepository<Membership>>()
            .InsertMany(memberships);

        return services;
    }

    public static IServiceProvider ConfigureMembers(this IServiceProvider services)
    {
        var configuration = services.GetRequiredService<IConfiguration>();

        var groupSections = configuration.GetRequiredSection(MembersConfigurationKey).GetChildren();

        var groupedMembers = groupSections
            .ToDictionary(
                group => group.Key,
                group => group.Get<Member[]>() ?? throw new RepositoryException($"Could not get `{MembersConfigurationKey}` configuration for group `{group.Key}`"));

        foreach(var group in groupedMembers.Keys)
        {
            var members = groupedMembers[group];

            using var scope = services.CreateScope();

            scope.ServiceProvider
                .GetRequiredService<MultitenancyContext>()
                .MultitenancyDiscriminator = group;

            scope.ServiceProvider
                .GetRequiredService<IRepository<Member>>()
                .InsertMany(members);
        }

        return services;
    }
}
