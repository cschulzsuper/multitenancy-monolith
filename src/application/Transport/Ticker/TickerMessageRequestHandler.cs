using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Responses;
using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Requests;
using System.Linq;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

internal sealed class TickerMessageRequestHandler : ITickerMessageRequestHandler
{
    private readonly ITickerMessageManager _tickerMessageManager;

    private readonly string? _currentTickerUser;

    public TickerMessageRequestHandler(
        ITickerMessageManager tickerMessageManager, 
        ClaimsPrincipal user)
    {
        _tickerMessageManager = tickerMessageManager;
        
        _currentTickerUser = user.GetClaimOrDefault("mailAddress");
    }

    public async ValueTask<TickerMessageResponse> GetAsync(long tickerMessage)
    {
        var @object = await _tickerMessageManager.GetAsync(tickerMessage);

        EnsureCurrentTickerUser(@object.TickerUser);

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

    public async IAsyncEnumerable<TickerMessageResponse> GetAll(string query, int skip, int take)
    {
        var objects = _tickerMessageManager.GetAsyncEnumerable(
            query =>
            {          
                if (_currentTickerUser != null)
                {
                    query = query
                        .Where(x => x.TickerUser == _currentTickerUser);
                }

                query = query
                    .Skip(skip)
                    .Take(take);

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

    public async ValueTask<TickerMessageResponse> InsertAsync(TickerMessageRequest request)
    {
        EnsureCurrentTickerUser(request.TickerUser);

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

    public async ValueTask UpdateAsync(long tickerMessage, TickerMessageRequest request)
    {
        EnsureCurrentTickerUser(request.TickerUser);

        await _tickerMessageManager.UpdateAsync(tickerMessage,
            @object =>
            {
                EnsureCurrentTickerUser(@object.TickerUser);

                @object.Text = request.Text;
                @object.Priority = request.Priority;
                @object.TickerUser = request.TickerUser;
            });
    }

    public async ValueTask DeleteAsync(long tickerMessage)
    {
        if (_currentTickerUser != null)
        {
            var @object = await _tickerMessageManager.GetAsync(tickerMessage);
            EnsureCurrentTickerUser(@object.TickerUser);
        }

        await _tickerMessageManager.DeleteAsync(tickerMessage);
    }



    private void EnsureCurrentTickerUser(string tickerUser)
    {
        if (_currentTickerUser != null &&
            _currentTickerUser != tickerUser)
        {
            TransportException.ThrowSecurityViolation($"Ticker message operation is not allowed.");
        }
    }
}