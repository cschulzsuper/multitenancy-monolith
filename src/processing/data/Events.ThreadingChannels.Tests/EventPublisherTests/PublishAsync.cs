using ChristianSchulz.MultitenancyMonolith.Events;
using ChristianSchulz.MultitenancyMonolith.Shared.Logging;
using Events.ThreadingChannels.Tests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace EventPublisherTests;

public sealed class PublishAsync
{
    private readonly IServiceProvider _services;

    public PublishAsync(ITestOutputHelper output)
    {
        var services = new ServiceCollection();

        services.AddScoped<MockHandler>();
        services.AddScoped<MockHandlerContext>();
        services.AddSingleton<MockHandlerStatistics>();

        services.AddEvents(options =>
        {
            options.ChannelNameResolver = _ => $"{Guid.NewGuid()}";

            options.BeforeSubscriptionInvocation = (provider, channelName) =>
            {
                provider.GetRequiredService<MockHandlerContext>().Scope = channelName;

                return Task.CompletedTask;
            };
        });

        services.AddLogging(configure => configure.AddProvider(new XunitLoggerProvider(output)));

        _services = services.BuildServiceProvider();
    }

    [Theory]
    [InlineData(1, 100)]
    [InlineData(10, 100)]
    [InlineData(100, 100)]
    [InlineData(500, 100)]
    public async Task PublishAsync_ShouldPublish_ToScope(int scopes, int calls)
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();

        await _services.GetRequiredService<IHostedService>()
            .StartAsync(cancellationTokenSource.Token);

        _services.GetRequiredService<IEventSubscriptions>()
            .Map("mock-event", (MockHandler handler, long snowflake) => handler.ExecuteAsync(snowflake));

        //Act
        foreach (var _ in Enumerable.Range(0, scopes))
        {
            await using var scope = _services.CreateAsyncScope();

            foreach (var snowflake in Enumerable.Range(0, calls))
            {
                await scope.ServiceProvider
                    .GetRequiredService<IEventPublisher>()
                    .PublishAsync("mock-event", snowflake);
            }
        }

        // Assert
        await _services
            .GetRequiredService<NamedChannelDictionary<EventValue>>()
            .CompleteAllAsync(calls * calls);

        var mockHandlerStatistics = _services
            .GetRequiredService<MockHandlerStatistics>();

        Assert.All(mockHandlerStatistics.CallsPerScope.Values,
            callsOfScope => Assert.Equal(calls, callsOfScope));

        //Finalize
        cancellationTokenSource.Cancel();

        await _services
            .GetRequiredService<IHostedService>()
            .StopAsync(cancellationTokenSource.Token);
    }
}