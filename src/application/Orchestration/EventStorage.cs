using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application;

public sealed class EventStorage : IEventStorage, IAsyncDisposable
{
    private readonly ILogger<EventStorage> _logger;

    private readonly ICollection<(string Event, long Snowflake)> _events;

    private Task _flush;

    private bool _flushed;

    private readonly SemaphoreSlim _flushLock = new SemaphoreSlim(1);

    public EventStorage(ILogger<EventStorage> logger)
    {
        _logger = logger;
        _events = new List<(string Event, long Snowflake)>();
        _flush = Task.CompletedTask;
    }

    public void Add(string @event, long snowflake)
    {
        if (_flushLock.CurrentCount == 0)
        {
            throw new Exception("Event storage is already flushing");
        }

        if (_flushed == true)
        {
            throw new Exception("Event storage is already flushing");
        }

        _events.Add((@event, snowflake));
    }

    public async ValueTask DisposeAsync()
    {
        await _flushLock.WaitAsync();
        await _flush;

        _flushLock.Release();

        GC.SuppressFinalize(this);
    }

    public async Task FlushAsync()
    {
        await _flushLock.WaitAsync();

        _flush = Task.Run(() =>
        {
            try
            {
                foreach (var @event in _events)
                {
                    _logger.LogInformation("Event '{event}' for snowflake '{snowflake}' has been flushed", @event.Event, @event.Snowflake);
                }

                _events.Clear();
            }
            finally
            {
                _flushed = true;
                _flushLock.Release();
            }
        });

    }
}