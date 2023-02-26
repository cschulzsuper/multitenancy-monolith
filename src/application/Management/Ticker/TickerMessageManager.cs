using ChristianSchulz.MultitenancyMonolith.Application.Authentication;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Events;

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

    public async Task<TickerMessage> GetAsync(long snowflake)
    {
        TickerMessageValidation.EnsureSnowflake(snowflake);

        var @object = await _repository.GetAsync(snowflake);
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

    public async Task UpdateAsync(long snowflake, Action<TickerMessage> action)
    {
        IdentityValidation.EnsureSnowflake(snowflake);

        var validatedAction = (TickerMessage @object) =>
        {
            action.Invoke(@object);

            TickerMessageValidation.EnsureUpdatable(@object);
        };

        await _repository.UpdateOrThrowAsync(snowflake, validatedAction);
    }

    public async Task DeleteAsync(long snowflake)
    {
        TickerMessageValidation.EnsureSnowflake(snowflake);

        await _repository.DeleteOrThrowAsync(snowflake);
    }
}