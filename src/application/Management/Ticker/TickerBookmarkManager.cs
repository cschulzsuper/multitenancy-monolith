using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Events;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

internal sealed class TickerBookmarkManager : ITickerBookmarkManager
{
    private readonly IRepository<TickerBookmark> _repository;
    private readonly IEventStorage _eventStorage;

    public TickerBookmarkManager(
        IRepository<TickerBookmark> repository,
        IEventStorage eventStorage)
    {
        _repository = repository;
        _eventStorage = eventStorage;
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

        _eventStorage.Add("ticker-bookmark-inserted", @object.Snowflake);
    }

    public async Task UpdateAsync(long tickerBookmark, Action<TickerBookmark> action)
    {
        TickerBookmarkValidation.EnsureTickerBookmark(tickerBookmark);

        var validatedAction = (TickerBookmark @object) =>
        {
            action.Invoke(@object);
            TickerBookmarkValidation.EnsureUpdatable(@object);
        };

        await _repository.UpdateOrThrowAsync(tickerBookmark, validatedAction);

        _eventStorage.Add("ticker-bookmark-updated", tickerBookmark);
    }

    public async Task UpdateAsync(string tickerUser, long tickerMessage, Action<TickerBookmark> action)
    {
        TickerBookmarkValidation.EnsureTickerUser(tickerUser);
        TickerBookmarkValidation.EnsureTickerMessage(tickerMessage);

        var validatedAction = (TickerBookmark @object) =>
        {
            action.Invoke(@object);
            TickerBookmarkValidation.EnsureUpdatable(@object);
        };

        var snowflake = await _repository.UpdateOrThrowAsync(@object =>
            @object.TickerUser == tickerUser &&
            @object.TickerMessage == tickerMessage, validatedAction);

        var unboxedSnowflake = snowflake as long?
            ?? throw new UnreachableException($"Expected snowflake to be of type '{nameof(Int64)}' but found '{snowflake.GetType()}'");

        _eventStorage.Add("ticker-bookmark-updated", unboxedSnowflake);
    }

    public async Task UpdateManyAsync(Expression<Func<TickerBookmark, bool>> predicate, Action<TickerBookmark> action)
    {
        var validatedAction = (TickerBookmark @object) =>
        {
            action.Invoke(@object);
            TickerBookmarkValidation.EnsureUpdatable(@object);
        };

        var snowflakes = await _repository.UpdateAsync(predicate, validatedAction);

        var unboxedSnowflakes = snowflakes.Select(snowflake => snowflake as long?
            ?? throw new UnreachableException($"Expected snowflake to be of type '{nameof(Int64)}' but found '{snowflake.GetType()}'"));

        foreach (var unboxedSnowflake in unboxedSnowflakes)
        {
            _eventStorage.Add("ticker-bookmark-updated", unboxedSnowflake);
        }
    }

    public async Task DeleteAsync(long tickerBookmark)
    {
        TickerBookmarkValidation.EnsureTickerBookmark(tickerBookmark);
        await _repository.DeleteOrThrowAsync(tickerBookmark);

        _eventStorage.Add("ticker-bookmark-deleted", tickerBookmark);
    }

    public async Task DeleteAsync(string tickerUser, long tickerMessage)
    {
        TickerBookmarkValidation.EnsureTickerUser(tickerUser);
        TickerBookmarkValidation.EnsureTickerMessage(tickerMessage);

        var snowflake = await _repository.DeleteOrThrowAsync(@object =>
            @object.TickerUser == tickerUser &&
            @object.TickerMessage == tickerMessage);

        var unboxedSnowflake = snowflake as long?
            ?? throw new UnreachableException($"Expected snowflake to be of type '{nameof(Int64)}' but found '{snowflake.GetType()}'");

        _eventStorage.Add("ticker-bookmark-deleted", unboxedSnowflake);
    }

    public async Task DeleteManyAsync(Expression<Func<TickerBookmark, bool>> predicate)
    {
        var snowflakes = await _repository.DeleteAsync(predicate);

        var unboxedSnowflakes = snowflakes.Select(snowflake => snowflake as long?
            ?? throw new UnreachableException($"Expected snowflake to be of type '{nameof(Int64)}' but found '{snowflake.GetType()}'"));

        foreach (var unboxedSnowflake in unboxedSnowflakes)
        {
            _eventStorage.Add("ticker-bookmark-deleted", unboxedSnowflake);
        }
    }
}