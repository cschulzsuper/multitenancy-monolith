using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using ChristianSchulz.MultitenancyMonolith.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChristianSchulz.MultitenancyMonolith.Web;

public static class _Services
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, params string[] uniqueNames)
    {
        foreach (var uniqueName in uniqueNames)
        {
            var builder = services.AddHttpClient(uniqueName,
                (services, httpClient) =>
                    {
                        var serviceMappings = services
                            .GetRequiredService<IServiceMappingsProvider>()
                            .Get();

                        var serviceMapping = serviceMappings.SingleOrDefault(x => x.UniqueName == uniqueName);

                        if (serviceMapping == null)
                        {
                            throw new UnreachableException($"Service mapping for '{uniqueName}' is not configured");
                        }

                        httpClient.BaseAddress = new Uri(serviceMapping.ServiceUrl);
                        httpClient.Timeout = TimeSpan.FromMilliseconds(250);

                    });

            var devCertTrust = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development;
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
