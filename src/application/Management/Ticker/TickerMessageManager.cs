using ChristianSchulz.MultitenancyMonolith.Application.Authentication;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

internal sealed class TickerMessageManager : ITickerMessageManager
{
    private readonly IRepository<TickerMessage> _repository;

    public TickerMessageManager(IRepository<TickerMessage> repository)
    {
        _repository = repository;
    }

    public async ValueTask<TickerMessage> GetAsync(long snowflake)
    {
        TickerMessageValidation.EnsureSnowflake(snowflake);

        var @object = await _repository.GetAsync(snowflake);
        return @object;
    }

    public IAsyncEnumerable<TickerMessage> GetAsyncEnumerable(Func<IQueryable<TickerMessage>, IQueryable<TickerMessage>> query)
        => _repository.GetAsyncEnumerable(query);

    public async ValueTask InsertAsync(TickerMessage tickerMessage)
    {
        TickerMessageValidation.EnsureInsertable(tickerMessage);

        await _repository.InsertAsync(tickerMessage);
    }

    public async ValueTask UpdateAsync(long snowflake, Action<TickerMessage> action)
    {
        IdentityValidation.EnsureSnowflake(snowflake);

        var validatedAction = (TickerMessage @object) =>
        {
            action.Invoke(@object);

            TickerMessageValidation.EnsureUpdatable(@object);
        };

        await _repository.UpdateOrThrowAsync(snowflake, validatedAction);
    }

    public async ValueTask DeleteAsync(long snowflake)
    {
        TickerMessageValidation.EnsureSnowflake(snowflake);

        await _repository.DeleteOrThrowAsync(snowflake);
    }
}