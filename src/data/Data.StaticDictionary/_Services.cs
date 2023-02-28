using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Administration;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Authentication;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Authorization;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Business;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Ticker;
using ChristianSchulz.MultitenancyMonolith.Objects.Administration;
using ChristianSchulz.MultitenancyMonolith.Objects.Authentication;
using ChristianSchulz.MultitenancyMonolith.Objects.Authorization;
using ChristianSchulz.MultitenancyMonolith.Objects.Business;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddStaticDictionary(this IServiceCollection services)
    {
        services.AddSingleton<SnowflakeGenerator>();

        services.AddSingleton(typeof(RepositoryContextFactory<>));
        services.AddScoped<MultitenancyContext>();

        return services;
    }

    public static IServiceCollection AddStaticDictionaryAdministrationData(this IServiceCollection services)
    {
        services.AddScoped(CreateRepository<ObjectTypeModel, ObjectType>);
        services.AddScoped(CreateRepository<DistinctionTypeModel, DistinctionType>);

        return services;
    }

    public static IServiceCollection AddStaticDictionaryAuthenticationData(this IServiceCollection services)
    {
        services.AddScoped(CreateRepository<IdentityModel, Identity>);

        return services;
    }

    public static IServiceCollection AddStaticDictionaryAuthorizationData(this IServiceCollection services)
    {
        services.AddScoped(CreateRepository<GroupModel, Group>);
        services.AddScoped(CreateRepository<MemberModel, Member>);

        return services;
    }

    public static IServiceCollection AddStaticDictionaryBusinessData(this IServiceCollection services)
    {
        services.AddScoped(CreateRepository<BusinessObjectModel, BusinessObject>);

        return services;
    }

    public static IServiceCollection AddStaticDictionaryTickerData(this IServiceCollection services)
    {
        services.AddScoped(CreateRepository<TickerBookmarkModel, TickerBookmark>);
        services.AddScoped(CreateRepository<TickerMessageModel, TickerMessage>);
        services.AddScoped(CreateRepository<TickerUserModel, TickerUser>);

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