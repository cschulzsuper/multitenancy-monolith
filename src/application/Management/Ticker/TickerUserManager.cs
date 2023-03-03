using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public class TickerUserManager : ITickerUserManager
{
    private readonly IRepository<TickerUser> _repository;

    public TickerUserManager(IRepository<TickerUser> repository)
    {
        _repository = repository;
    }

    public async Task<TickerUser> GetAsync(long tickerUser)
    {
        TickerUserValidation.EnsureTickerUser(tickerUser);

        var @object = await _repository.GetAsync(tickerUser);

        return @object;
    }

    public IAsyncEnumerable<TickerUser> GetAsyncEnumerable(Func<IQueryable<TickerUser>, IQueryable<TickerUser>> query)
        => _repository.GetAsyncEnumerable(query);

    public async Task InsertAsync(TickerUser @object)
    {
        TickerUserValidation.EnsureInsertable(@object);

        await _repository.InsertAsync(@object);
    }

    public async Task UpdateAsync(long tickerUser, Action<TickerUser> action)
    {
        TickerUserValidation.EnsureTickerUser(tickerUser);

        var validatedAction = (TickerUser @object) =>
        {
            action.Invoke(@object);

            TickerUserValidation.EnsureUpdatable(@object);
        };

        await _repository.UpdateOrThrowAsync(tickerUser, validatedAction);
    }

    public async Task UpdateAsync(string tickerUser, Action<TickerUser> action, Action @default)
    {
        TickerUserValidation.EnsureTickerUser(tickerUser);

        var validatedAction = (TickerUser @object) =>
        {
            action.Invoke(@object);

            TickerUserValidation.EnsureUpdatable(@object);
        };

        await _repository.UpdateOrThrowAsync(@object => @object.MailAddress == tickerUser, validatedAction, @default);
    }

    public async Task DeleteAsync(long tickerUser)
    {
        TickerUserValidation.EnsureTickerUser(tickerUser);

        await _repository.DeleteOrThrowAsync(tickerUser);
    }
}
