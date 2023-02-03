using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary.Model;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary.Model.Administration;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary.Model.Authentication;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary.Model.Authorization;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary.Model.Business;
using ChristianSchulz.MultitenancyMonolith.Objects.Administration;
using ChristianSchulz.MultitenancyMonolith.Objects.Authentication;
using ChristianSchulz.MultitenancyMonolith.Objects.Authorization;
using ChristianSchulz.MultitenancyMonolith.Objects.Business;
using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;

public static partial class _Services
{
    public static IServiceCollection AddData(this IServiceCollection services)
    {
        services.AddSingleton<SnowflakeGenerator>();

        services.AddSingleton(typeof(RepositoryContextFactory<>));
        services.AddScoped<MultitenancyContext>();

        services.AddScoped(CreateRepository<ObjectTypeModel, ObjectType>);
        services.AddScoped(CreateRepository<DistinctionTypeModel, DistinctionType>);

        services.AddScoped(CreateRepository<IdentityModel, Identity>);

        services.AddScoped(CreateRepository<GroupModel, Group>);
        services.AddScoped(CreateRepository<MemberModel, Member>);
        services.AddScoped(CreateRepository<MembershipModel, Membership>);

        services.AddScoped(CreateRepository<BusinessObjectModel, BusinessObject>);

        return services;
    }

    private static IRepository<TEntity> CreateRepository<TModel, TEntity>(IServiceProvider services)
        where TModel : IModel<TEntity>
        where TEntity : class, ICloneable
    {
        var multitenancyDiscriminator = !TModel.Multitenancy
            ? string.Empty
            : services.GetRequiredService<MultitenancyContext>()
                .MultitenancyDiscriminator;

        var snowflakeGenerator = services.GetRequiredService<SnowflakeGenerator>();

        var repositoryContextFactory = services.GetRequiredService<RepositoryContextFactory<TEntity>>();
        var repositoryContext = repositoryContextFactory.Create(multitenancyDiscriminator,
            entity => TModel.SetSnowflake(entity, snowflakeGenerator.Next()),
            entity => TModel.GetSnowflake(entity),
            (data, entity) => TModel.Ensure(services, data, entity));

        return new Repository<TEntity>(repositoryContext);
    }
}