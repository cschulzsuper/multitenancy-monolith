using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public class TickerUserManager : ITickerUserManager
{
    private readonly IRepository<TickerUser> _repository;

    public TickerUserManager(IRepository<TickerUser> repository)
    {
        _repository = repository;
    }

    public async ValueTask<TickerUser> GetAsync(string tickerUser)
    {
        TickerUserValidation.EnsureTicketUser(tickerUser);

        var @object = await _repository.GetAsync(x => x.MailAddress == tickerUser);

        return @object;
    }

    public async ValueTask<TickerUser?> GetOrDefaultAsync(string tickerUser)
    {
        TickerUserValidation.EnsureTicketUser(tickerUser);

        var exists = await _repository.GetOrDefaultAsync(x =>x.MailAddress == tickerUser);

        return exists;
    }


    public async ValueTask InsertAsync(TickerUser @object)
    {
        TickerUserValidation.EnsureInsertable(@object);

        await _repository.InsertAsync(@object);
    }

    public async ValueTask UpdateOrDefaultAsync(string tickerUser, Action<TickerUser> action, Action @default)
    {
        TickerUserValidation.EnsureTicketUser(tickerUser);

        var validatedAction = (TickerUser @object) =>
        {
            action.Invoke(@object);

            TickerUserValidation.EnsureUpdatable(@object);
        };

        await _repository.UpdateOrThrowAsync(x => x.MailAddress == tickerUser, validatedAction, @default);
    }
}
