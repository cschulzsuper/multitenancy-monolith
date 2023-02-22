using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Responses;
using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Requests;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

internal sealed class TickerMessageRequestHandler : ITickerMessageRequestHandler
{
    private readonly ITickerMessageManager _tickerMessageManager;

    public TickerMessageRequestHandler(ITickerMessageManager tickerMessageManager)
    {
        _tickerMessageManager = tickerMessageManager;
    }

    public async Task<TickerMessageResponse> GetAsync(long tickerMessage)
    {
        var @object = await _tickerMessageManager.GetAsync(tickerMessage);

        var response = new TickerMessageResponse
        {
            Snowflake = @object.Snowflake,
            Text = @object.Text,
            Priority = @object.Priority,
            Timestamp = @object.Timestamp,
            TickerUser = @object.TickerUser
        };

        return response;
    }

    public async IAsyncEnumerable<TickerMessageResponse> GetAll(string? query, int? skip, int? take)
    {
        var objects = _tickerMessageManager.GetAsyncEnumerable(
            query =>
            {
                query = skip != null ? query.Skip(skip.Value) : query;
                query = take != null ? query.Take(take.Value) : query;

                return query;
            });

        await foreach (var @object in objects)
        {
            var response = new TickerMessageResponse
            {
                Snowflake = @object.Snowflake,
                Text = @object.Text,
                Priority = @object.Priority,
                Timestamp = @object.Timestamp,
                TickerUser = @object.TickerUser
            };

            yield return response;
        }
    }

    public async Task<TickerMessageResponse> InsertAsync(TickerMessageRequest request)
    {
        var @object = new TickerMessage
        {
            Text = request.Text,
            Priority = request.Priority,
            Timestamp = DateTime.UtcNow.Ticks,
            TickerUser = request.TickerUser
        };

        await _tickerMessageManager.InsertAsync(@object);

        var response = new TickerMessageResponse
        {
            Snowflake = @object.Snowflake,
            Text = @object.Text,
            Priority = @object.Priority,
            Timestamp = @object.Timestamp,
            TickerUser = @object.TickerUser
        };

        return response;
    }

    public async Task UpdateAsync(long tickerMessage, TickerMessageRequest request)
    {
        await _tickerMessageManager.UpdateAsync(tickerMessage,
            @object =>
            {
                @object.Text = request.Text;
                @object.Priority = request.Priority;
                @object.TickerUser = request.TickerUser;
            });
    }

    public async Task DeleteAsync(long tickerMessage)
        => await _tickerMessageManager.DeleteAsync(tickerMessage);
}