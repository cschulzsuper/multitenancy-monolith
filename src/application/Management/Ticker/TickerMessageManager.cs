using ChristianSchulz.MultitenancyMonolith.Application.Admission;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Events;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

internal sealed class TickerMessageManager : ITickerMessageManager
{
    private readonly IRepository<TickerMessage> _repository;
    private readonly IEventStorage _eventStorage;

    public TickerMessageManager(
        IRepository<TickerMessage> repository,
        IEventStorage eventStorage)
    {
        _repository = repository;
        _eventStorage = eventStorage;
    }

    public async Task<TickerMessage> GetAsync(long tickerMessage)
    {
        TickerMessageValidation.EnsureTickerMessage(tickerMessage);

        var @object = await _repository.GetAsync(tickerMessage);

        return @object;
    }

    public IAsyncEnumerable<TickerMessage> GetAsyncEnumerable(Func<IQueryable<TickerMessage>, IQueryable<TickerMessage>> query)
        => _repository.GetAsyncEnumerable(query);

    public async Task InsertAsync(TickerMessage @object)
    {
        TickerMessageValidation.EnsureInsertable(@object);

        await _repository.InsertAsync(@object);

        _eventStorage.Add("ticker-message-inserted", @object.Snowflake);
    }

    public async Task UpdateAsync(long tickerMessage, Action<TickerMessage> action)
    {
        AuthenticationIdentityValidation.EnsureAuthenticationIdentity(tickerMessage);

        var validatedAction = (TickerMessage @object) =>
        {
            action.Invoke(@object);

            TickerMessageValidation.EnsureUpdatable(@object);

            _eventStorage.Add("ticker-message-updated", @object.Snowflake);
        };

        await _repository.UpdateOrThrowAsync(tickerMessage, validatedAction);
    }

    public async Task DeleteAsync(long tickerMessage)
    {
        TickerMessageValidation.EnsureTickerMessage(tickerMessage);

        await _repository.DeleteOrThrowAsync(tickerMessage);

        _eventStorage.Add("ticker-message-deleted", tickerMessage);

    }
}