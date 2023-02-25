using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Events;
using ChristianSchulz.MultitenancyMonolith.Shared.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace EventPublisher;

public sealed class PublishAsync
{
    private readonly IServiceProvider _services;

    public PublishAsync(ITestOutputHelper output)
    {
        var services = new ServiceCollection();

        services.AddSingleton<MockHandler>();

        services.AddEvents(options => { options.ChannelNameResolver = _ => $"{Guid.NewGuid()}"; });

        services.AddLogging(configure => configure.AddProvider(new XunitLoggerProvider(output)));

        _services = services.BuildServiceProvider();
    }

    [Theory]
    [InlineData(1, 100)]
    [InlineData(10, 100)]
    [InlineData(100, 100)]
    [InlineData(1000, 100)]
    public async Task PublishAsync_ShouldPublish_ToScope(int scopes, int calls)
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();

        await _services.GetRequiredService<IHostedService>()
            .StartAsync(cancellationTokenSource.Token);

        _services.GetRequiredService<IEventSubscriptions>()
            .Map("mock-event", (MockHandler handler, EventSubscriptionInvocationContext context) => handler.ExecuteAsync(context));

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

        var mockHandler = _services
            .GetRequiredService<MockHandler>();

        Assert.All(mockHandler.CallsPerScope,
            callsOfScope => Assert.Equal(calls, callsOfScope));

        //Finalize
        await _services
            .GetRequiredService<IHostedService>()
            .StopAsync(cancellationTokenSource.Token);
    }
}