﻿using ChristianSchulz.MultitenancyMonolith.Backend.Server;
using ChristianSchulz.MultitenancyMonolith.Events;
using ChristianSchulz.MultitenancyMonolith.Shared.Logging;
using ChristianSchulz.MultitenancyMonolith.Shared.Multitenancy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
internal static class MockWebApplication
{
    public const string Client = "event-tests";

    public const string Group = "group";
    public const string Member = "default";
    public const string Mail = "default@localhost";

    private static readonly IDictionary<string, string> _configuration = new Dictionary<string, string>();

    public static WebApplicationFactory<Program> Mock(this WebApplicationFactory<Program> factory, ITestOutputHelper? output = null)
        => factory.WithWebHostBuilder(app => app
            .ConfigureServices(provider =>
            {
                provider.Configure<EventsOptions>(options =>
                {
                    options.ChannelNameResolver = _ => Group;
                });
            })
            .ConfigureLogging(loggingBuilder =>
            {
                if (output != null)
                {
                    loggingBuilder.Services.AddSingleton<ILoggerProvider>(_ => new XunitLoggerProvider(output));
                }
            })
            .ConfigureAppConfiguration((_, config) =>
            {
                config.Sources.Clear();
                config.AddInMemoryCollection(_configuration!);
            }));

    public static AsyncServiceScope CreateAsyncMultitenancyScope(this WebApplicationFactory<Program> factory)
        => factory.Services.CreateAsyncMultitenancyScope(Group);
}