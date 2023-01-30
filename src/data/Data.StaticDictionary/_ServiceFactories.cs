using ChristianSchulz.MultitenancyMonolith.Objects.Administration;
using ChristianSchulz.MultitenancyMonolith.Objects.Authentication;
using ChristianSchulz.MultitenancyMonolith.Objects.Authorization;
using ChristianSchulz.MultitenancyMonolith.Objects.Business;
using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Data;

public static partial class _Services
{
    private static Func<IServiceProvider, IRepository<ObjectType>> ObjectTypeRepositoryFactory =>
        (services) =>
        {
            var multitenancyContext = services.GetRequiredService<MultitenancyContext>();
            var multitenancyDiscriminator = multitenancyContext.MultitenancyDiscriminator;

            var snowflakeGenerator = services.GetRequiredService<SnowflakeGenerator>();

            var repositoryContextFactory = services.GetRequiredService<RepositoryContextFactory<ObjectType>>();
            var repositoryContext = repositoryContextFactory.Create(multitenancyDiscriminator,
                entity => entity.Snowflake = snowflakeGenerator.Next(),
                entity => entity.Snowflake,
                (data, entity) => entity.CustomProperties.GroupBy(x => x.UniqueName).All(x => x.Count() == 1),
                (data, entity) => data.All(x => x.UniqueName != entity.UniqueName));

            return new Repository<ObjectType>(repositoryContext);
        };

    private static Func<IServiceProvider, IRepository<DistinctionType>> DistinctionTypeRepositoryFactory =>
        (services) =>
        {
            var multitenancyContext = services.GetRequiredService<MultitenancyContext>();
            var multitenancyDiscriminator = multitenancyContext.MultitenancyDiscriminator;

            var snowflakeGenerator = services.GetRequiredService<SnowflakeGenerator>();

            var repositoryContextFactory = services.GetRequiredService<RepositoryContextFactory<DistinctionType>>();
            var repositoryContext = repositoryContextFactory.Create(multitenancyDiscriminator,
                entity => entity.Snowflake = snowflakeGenerator.Next(),
                entity => entity.Snowflake,
                (data, entity) => entity.CustomProperties.GroupBy(x => x.UniqueName).All(x => x.Count() == 1),
                (data, entity) => data.All(x => x.UniqueName != entity.UniqueName));

            return new Repository<DistinctionType>(repositoryContext);
        };

    private static Func<IServiceProvider, IRepository<Identity>> IdentityRepositoryFactory =>
        (services) =>
        {
            var snowflakeGenerator = services.GetRequiredService<SnowflakeGenerator>();

            var repositoryContextFactory = services.GetRequiredService<RepositoryContextFactory<Identity>>();
            var repositoryContext = repositoryContextFactory.Create(
                entity => entity.Snowflake = snowflakeGenerator.Next(),
                entity => entity.Snowflake,
                (data, entity) => data.All(x => x.UniqueName != entity.UniqueName));

            return new Repository<Identity>(repositoryContext);
        };

    private static Func<IServiceProvider, IRepository<Member>> MemberRepositoryFactory =>
        (services) =>
        {
            var multitenancyContext = services.GetRequiredService<MultitenancyContext>();
            var multitenancyDiscriminator = multitenancyContext.MultitenancyDiscriminator;

            var snowflakeGenerator = services.GetRequiredService<SnowflakeGenerator>();

            var repositoryContextFactory = services.GetRequiredService<RepositoryContextFactory<Member>>();
            var repositoryContext = repositoryContextFactory.Create(multitenancyDiscriminator,
                entity => entity.Snowflake = snowflakeGenerator.Next(),
                entity => entity.Snowflake,
                (data, entity) => data.All(x => x.UniqueName != entity.UniqueName));

            return new Repository<Member>(repositoryContext);
        };

    private static Func<IServiceProvider, IRepository<Membership>> MembershipRepositoryFactory =>
        (services) =>
        {
            var snowflakeGenerator = services.GetRequiredService<SnowflakeGenerator>();

            var repositoryContextFactory = services.GetRequiredService<RepositoryContextFactory<Membership>>();
            var repositoryContext = repositoryContextFactory.Create(
                entity => entity.Snowflake = snowflakeGenerator.Next(),
                entity => entity.Snowflake);

            return new Repository<Membership>(repositoryContext);
        };

    private static Func<IServiceProvider, IRepository<Group>> GroupRepositoryFactory =>
        (services) =>
        {
            var snowflakeGenerator = services.GetRequiredService<SnowflakeGenerator>();

            var repositoryContextFactory = services.GetRequiredService<RepositoryContextFactory<Group>>();
            var repositoryContext = repositoryContextFactory.Create(
                entity => entity.Snowflake = snowflakeGenerator.Next(),
                entity => entity.Snowflake,
                (data, entity) => data.All(x => x.UniqueName != entity.UniqueName));

            return new Repository<Group>(repositoryContext);
        };

    private static Func<IServiceProvider, IRepository<BusinessObject>> BusinessObjectRepositoryFactory =>
        (services) =>
        {
            var multitenancyContext = services.GetRequiredService<MultitenancyContext>();
            var multitenancyDiscriminator = multitenancyContext.MultitenancyDiscriminator;

            var snowflakeGenerator = services.GetRequiredService<SnowflakeGenerator>();

            var repositoryContextFactory = services.GetRequiredService<RepositoryContextFactory<BusinessObject>>();
            var repositoryContext = repositoryContextFactory.Create(multitenancyDiscriminator,
                entity => entity.Snowflake = snowflakeGenerator.Next(),
                entity => entity.Snowflake,
                (data, entity) => data.All(x => x.UniqueName != entity.UniqueName));

            return new Repository<BusinessObject>(repositoryContext);
        };
}