using ChristianSchulz.MultitenancyMonolith.Configuration;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.Portal.DataProtection;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Configure
{
    public static IDataProtectionBuilder Configure(this IDataProtectionBuilder builder,
        IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        builder.SetApplicationName(nameof(MultitenancyMonolith));

        if (!environment.IsDevelopment())
        {
            var configurationProxyProvider = new ConfigurationProxyProvider(configuration);
            var configuredDistributedCache = configurationProxyProvider.GetDistributedCache();

            var configurationOptions = new ConfigurationOptions
            {
                AllowAdmin = false,
                Password = configuredDistributedCache.Secret
            };

            configurationOptions.EndPoints.Add(configuredDistributedCache.Host);

            builder.PersistKeysToStackExchangeRedis(
                ConnectionMultiplexer.Connect(configurationOptions),
                "DataProtection-Keys");
        }

        return builder;
    }
}