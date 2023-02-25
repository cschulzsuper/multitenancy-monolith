using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Events;
using Microsoft.Extensions.Logging;

internal sealed class MockHandler
{
    private readonly ILogger<MockHandler> _logger;

    private readonly ConcurrentDictionary<string, int> _callsPerScope;

    public MockHandler(ILogger<MockHandler> logger)
    {
        _logger = logger;
        _callsPerScope = new ConcurrentDictionary<string, int>();
    }

    public Task ExecuteAsync(EventSubscriptionInvocationContext context)
    {
        _logger.LogInformation("MockHandler for snowflake '{snowflake}' executed in scope '{group}'", context.Snowflake, context.Scope);

        _callsPerScope.AddOrUpdate(context.Scope, _ => 1, (_, value) => value + 1);

        return Task.CompletedTask;
    }

    public ICollection<int> CallsPerScope
        => _callsPerScope.Values;
}