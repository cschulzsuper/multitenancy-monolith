using System.Threading.Tasks;
using Events.ThreadingChannels.Tests;
using Microsoft.Extensions.Logging;

internal sealed class MockHandler
{
    private readonly ILogger<MockHandler> _logger;
    private readonly MockHandlerContext _mockHandlerContext;
    private readonly MockHandlerStatistics _mockHandlerStatistics;

    public MockHandler(
        ILogger<MockHandler> logger,
        MockHandlerContext mockHandlerContext,
        MockHandlerStatistics mockHandlerStatistics)
    {
        _logger = logger;
        _mockHandlerContext = mockHandlerContext;
        _mockHandlerStatistics = mockHandlerStatistics;
    }

    public Task ExecuteAsync(long snowflake)
    {
        _logger.LogInformation("MockHandler for snowflake '{snowflake}' executed in scope '{group}'", snowflake, _mockHandlerContext.Scope);

        _mockHandlerStatistics.CallsPerScope.AddOrUpdate(_mockHandlerContext.Scope, _ => 1, (_, value) => value + 1);

        return Task.CompletedTask;
    }
}