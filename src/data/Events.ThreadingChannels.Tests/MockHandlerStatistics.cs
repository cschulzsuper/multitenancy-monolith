using System.Collections.Concurrent;

namespace Events.ThreadingChannels.Tests;

internal sealed class MockHandlerStatistics
{
    public ConcurrentDictionary<string, int> CallsPerScope { get; set; } = new ConcurrentDictionary<string, int>();
}