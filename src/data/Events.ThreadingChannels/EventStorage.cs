using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ChristianSchulz.MultitenancyMonolith.Events;

internal sealed class EventStorage : IEventStorage, IAsyncDisposable
{
    private readonly ILogger<IEventStorage> _logger;
    private readonly IEventPublisher _publisher;
    private readonly List<(string Event, long Snowflake)> _events;

    private Task _flush;

    private bool _flushed;

    private readonly SemaphoreSlim _flushLock = new(1);

    public EventStorage(
        ILogger<IEventStorage> logger,
        IEventPublisher publisher)
    {
        _logger = logger;

        _events = new List<(string Event, long Snowflake)>();
        _publisher = publisher;

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
            throw new Exception("Event storage is already flushed");
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
                    _publisher.PublishAsync(@event.Event, @event.Snowflake);

                    _logger.LogInformation("Event '{event}' for '{snowflake}' has been flushed", @event.Event, @event.Snowflake);
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