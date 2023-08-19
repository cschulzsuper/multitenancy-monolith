using ChristianSchulz.MultitenancyMonolith.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;

namespace ChristianSchulz.MultitenancyMonolith.Web;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    private const int DefaultRequestTimeOut = 1000;

    public static IServiceCollection AddWebServices(this IServiceCollection services, params string[] uniqueNames)
    {
        foreach (var uniqueName in uniqueNames)
        {
            var builder = services
                .AddHttpClient(uniqueName, (services, httpClient) =>
                    {
                        var serviceMappings = services
                            .GetRequiredService<IConfigurationProxyProvider>()
                            .GetServiceMappings();

                        var serviceMapping = serviceMappings.SingleOrDefault(x => x.UniqueName == uniqueName)
                            ?? throw new UnreachableException($"Service mapping for '{uniqueName}' is not configured");

                        httpClient.BaseAddress = new Uri(serviceMapping.Url);
                        httpClient.Timeout = TimeSpan.FromMilliseconds(DefaultRequestTimeOut);
                    });

            var devCertTrust = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != Environments.Production;
            if (devCertTrust)
            {
                builder.ConfigurePrimaryHttpMessageHandler(services =>
                {
                    return new HttpClientHandler()
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };
                });
            }
        }

        services.AddScoped<IWebServiceClientFactory, WebServiceClientFactory>();

        return services;
    }
}
