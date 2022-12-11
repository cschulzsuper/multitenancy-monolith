using ChristianSchulz.MultitenancyMonolith.Aggregates.Administration;
using ChristianSchulz.MultitenancyMonolith.Aggregates.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Data;

public static partial class _Services
{
    private static Func<IServiceProvider, IRepository<Member>> MemberRepositoryFactory =>
        (services) =>
        {
            var multitenancyContext = services.GetRequiredService<MultitenancyContext>();
            var multitenancyDiscriminator = multitenancyContext.MultitenancyDiscriminator;

            var snowflakeGenerator = services.GetRequiredService<SnowflakeGenerator>();

            var repositoryContextFactory = services.GetRequiredService<RepositoryContextFactory<Member>>();
            var repositoryContext = repositoryContextFactory.Create(multitenancyDiscriminator, member => member.Snowflake = snowflakeGenerator.Next());

            return new Repository<Member>(repositoryContext);
        };

    private static Func<IServiceProvider, IRepository<Membership>> MembershipRepositoryFactory =>
        (services) =>
        {
            var snowflakeGenerator = services.GetRequiredService<SnowflakeGenerator>();

            var repositoryContextFactory = services.GetRequiredService<RepositoryContextFactory<Membership>>();
            var repositoryContext = repositoryContextFactory.Create(membership => membership.Snowflake = snowflakeGenerator.Next());

            return new Repository<Membership>(repositoryContext);
        };

    private static Func<IServiceProvider, IRepository<Group>> GroupRepositoryFactory =>
        (services) =>
        {
            var snowflakeGenerator = services.GetRequiredService<SnowflakeGenerator>();

            var repositoryContextFactory = services.GetRequiredService<RepositoryContextFactory<Group>>();
            var repositoryContext = repositoryContextFactory.Create(group => group.Snowflake = snowflakeGenerator.Next());

            return new Repository<Group>(repositoryContext);
        };

    private static Func<IServiceProvider, IRepository<Identity>> IdentityRepositoryFactory =>
        (services) =>
        {
            var snowflakeGenerator = services.GetRequiredService<SnowflakeGenerator>();

            var repositoryContextFactory = services.GetRequiredService<RepositoryContextFactory<Identity>>();
            var repositoryContext = repositoryContextFactory.Create(identity => identity.Snowflake = snowflakeGenerator.Next());

            return new Repository<Identity>(repositoryContext);
        };
}
