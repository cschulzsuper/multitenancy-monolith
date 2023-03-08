using ChristianSchulz.MultitenancyMonolith.Application.Admission;
using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

internal sealed class TickerBookmarkManager : ITickerBookmarkManager
{
    private readonly IRepository<TickerBookmark> _repository;

    public TickerBookmarkManager(IRepository<TickerBookmark> repository)
    {
        _repository = repository;
    }

    public async Task<TickerBookmark> GetAsync(long tickerBookmark)
    {
        TickerBookmarkValidation.EnsureTickerBookmark(tickerBookmark);

        var @object = await _repository.GetAsync(tickerBookmark);

        return @object;
    }

    public IAsyncEnumerable<TickerBookmark> GetAsyncEnumerable(Func<IQueryable<TickerBookmark>, IQueryable<TickerBookmark>> query)
        => _repository.GetAsyncEnumerable(query);

    public async Task InsertAsync(TickerBookmark @object)
    {
        TickerBookmarkValidation.EnsureInsertable(@object);

        await _repository.InsertAsync(@object);
    }

    public async Task UpdateAsync(long tickerBookmark, Action<TickerBookmark> action)
    {
        AuthenticationIdentityValidation.EnsureAuthenticationIdentity(tickerBookmark);

        var validatedAction = (TickerBookmark @object) =>
        {
            action.Invoke(@object);

            TickerBookmarkValidation.EnsureUpdatable(@object);
        };

        await _repository.UpdateOrThrowAsync(tickerBookmark, validatedAction);
    }

    public async Task DeleteAsync(long tickerBookmark)
    {
        TickerBookmarkValidation.EnsureTickerBookmark(tickerBookmark);

        await _repository.DeleteOrThrowAsync(tickerBookmark);
    }

    public async Task DeleteAsync(string tickerUser, long tickerMessage)
    {
        TickerBookmarkValidation.EnsureTickerUser(tickerUser);
        TickerBookmarkValidation.EnsureTickerMessage(tickerMessage);

        await _repository.DeleteOrThrowAsync(@object =>
            @object.TickerUser == tickerUser &&
            @object.TickerMessage == tickerMessage);
    }

    public async Task UpdateManyAsync(Expression<Func<TickerBookmark, bool>> predicate, Action<TickerBookmark> action)
    {
        var validatedAction = (TickerBookmark @object) =>
        {
            action.Invoke(@object);

            TickerBookmarkValidation.EnsureUpdatable(@object);
        };

        await _repository.UpdateAsync(predicate, validatedAction);
    }

    public async Task DeleteManyAsync(Expression<Func<TickerBookmark, bool>> predicate)
        => await _repository.DeleteAsync(predicate);
}