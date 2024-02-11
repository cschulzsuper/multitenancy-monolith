using ChristianSchulz.MultitenancyMonolith.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public static class _Services
{
    public static IServiceCollection AddTransportWebServiceClientFactory(this IServiceCollection services)
    {
        services.AddScoped<TransportWebServiceClientFactory>();

        return services;
    }

    internal static IServiceCollection AddWebServiceTransportDefaultClient<TService>(this IServiceCollection services, Func<IConfigurationProxyProvider, string> webServiceResolver)
        where TService : class, IDisposable
    {
        services.AddScoped(provider =>
        {
            var configuration = provider.GetRequiredService<IConfigurationProxyProvider>();

            var configurationWebService = webServiceResolver.Invoke(configuration);
            var clientFactory = provider.GetRequiredService<TransportWebServiceClientFactory>();

            // TODO Add default token provider
            var client = clientFactory.Create<TService>(configurationWebService);

            return client;
        });

        return services;
    }
}