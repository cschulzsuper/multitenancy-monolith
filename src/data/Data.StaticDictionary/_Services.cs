using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Access;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Admission;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Business;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Documentation;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Extension;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Schedule;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Ticker;
using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using ChristianSchulz.MultitenancyMonolith.Objects.Business;
using ChristianSchulz.MultitenancyMonolith.Objects.Documentation;
using ChristianSchulz.MultitenancyMonolith.Objects.Extension;
using ChristianSchulz.MultitenancyMonolith.Objects.Schedule;
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

    public static IServiceCollection AddStaticDictionaryExtensionData(this IServiceCollection services)
    {
        services.AddScoped(CreateRepository<ObjectTypeMapping, ObjectType>);
        services.AddScoped(CreateRepository<DistinctionTypeMapping, DistinctionType>);

        return services;
    }

    public static IServiceCollection AddStaticDictionaryAdmissionData(this IServiceCollection services)
    {
        services.AddScoped(CreateRepository<AuthenticationIdentityMapping, AuthenticationIdentity>);
        services.AddScoped(CreateRepository<AuthenticationIdentityAuthenticationMethodMapping, AuthenticationIdentityAuthenticationMethod>);
        services.AddScoped(CreateRepository<AuthenticationRegistrationMapping, AuthenticationRegistration>);

        return services;
    }

    public static IServiceCollection AddStaticDictionaryAccessData(this IServiceCollection services)
    {
        services.AddScoped(CreateRepository<AccountGroupMapping, AccountGroup>);
        services.AddScoped(CreateRepository<AccountMemberMapping, AccountMember>);
        services.AddScoped(CreateRepository<AccountRegistrationMapping, AccountRegistration>);

        return services;
    }

    public static IServiceCollection AddStaticDictionaryBusinessData(this IServiceCollection services)
    {
        services.AddScoped(CreateRepository<BusinessObjectMapping, BusinessObject>);

        return services;
    }

    public static IServiceCollection AddStaticDictionaryDocumentationData(this IServiceCollection services)
    {
        services.AddScoped(CreateRepository<DevelopmentPostMapping, DevelopmentPost>);

        return services;
    }

    public static IServiceCollection AddStaticDictionaryScheduleData(this IServiceCollection services)
    {
        services.AddScoped(CreateRepository<PlannedJobMapping, PlannedJob>);

        return services;
    }

    public static IServiceCollection AddStaticDictionaryTickerData(this IServiceCollection services)
    {
        services.AddScoped(CreateRepository<TickerBookmarkMapping, TickerBookmark>);
        services.AddScoped(CreateRepository<TickerMessageMapping, TickerMessage>);
        services.AddScoped(CreateRepository<TickerUserMapping, TickerUser>);

        return services;
    }

    [SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
    private static IRepository<TEntity> CreateRepository<TMapping, TEntity>(IServiceProvider services)
        where TMapping : IMapping<TEntity>
        where TEntity : class, ICloneable
    {
        var multitenancyDiscriminator = !TMapping.Multitenancy
            ? string.Empty
            : services.GetRequiredService<MultitenancyContext>()
                .MultitenancyDiscriminator;

        var snowflakeGenerator = services.GetRequiredService<SnowflakeGenerator>();

        var repositoryContextFactory = services.GetRequiredService<RepositoryContextFactory<TEntity>>();
        var repositoryContext = repositoryContextFactory.Create(multitenancyDiscriminator,
            entity => TMapping.SetSnowflake(entity, snowflakeGenerator.Next()),
            entity => TMapping.GetSnowflake(entity),
            (data, entity) => TMapping.Ensure(services, data, entity));

        return new Repository<TEntity>(repositoryContext);
    }
}