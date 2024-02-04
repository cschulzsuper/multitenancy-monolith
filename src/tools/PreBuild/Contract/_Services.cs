using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Contract;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public static class _Services
{
    public static IServiceCollection AddPreBuildSerialization(this IServiceCollection services, Action<PreBuildSerializationClientOptions> setup)
    {
        services
            .AddOptions<PreBuildSerializationClientOptions>()
            .Configure(setup)
            .ValidateDataAnnotations();

        services.AddSingleton<IPreBuildSerializationClient, StrategyPreBuildSerializationClient>();
        services.AddSingleton<PreBuildSerializationClientResolver>();

        return services;
    }

    public static IServiceCollection AddPreBuildSerializationClient<TClient>(this IServiceCollection services)
        where TClient : class, IPreBuildSerializationClient
    {
        services.AddSingleton<TClient>();

        return services;
    }

    public static IServiceCollection AddPreBuildOutputs(this IServiceCollection services, Action<PreBuildOutputOptions> setup)
    {
        services
            .AddOptions<PreBuildOutputOptions>()
            .Configure(setup)
            .ValidateDataAnnotations();

        services.AddSingleton<IPreBuildOutput, StrategyPreBuildOutput>();
        services.AddSingleton<PreBuildOutputResolver>();

        return services;
    }

    public static IServiceCollection AddPreBuildOutput<TClient>(this IServiceCollection services)
        where TClient : class, IPreBuildOutput
    {
        services.AddSingleton<TClient>();

        return services;
    }

    public static IServiceCollection AddPreBuildOutput<TClient, TOptions>(this IServiceCollection services, Action<TOptions> setup)
        where TClient : class, IPreBuildOutput
        where TOptions : class
    {
        services
            .AddOptions<TOptions>()
            .Configure(setup)
            .ValidateDataAnnotations();

        services.AddSingleton<TClient>();

        return services;
    }
}