using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public class TickerUserManager : ITickerUserManager
{
    private readonly IRepository<TickerUser> _repository;

    public TickerUserManager(IRepository<TickerUser> repository)
    {
        _repository = repository;
    }

    public async ValueTask<TickerUser?> GetOrDefaultAsync(string tickerUser)
    {
        TickerUserValidation.EnsureTicketUser(tickerUser);

        var @object = await _repository.GetOrDefaultAsync(x => x.MailAddress == tickerUser);

        return @object;
    }
    public async ValueTask<bool> ExistsAsync(string tickerUser, string secret)
    {
        TickerUserValidation.EnsureTicketUser(tickerUser);

        var exists = await _repository
            .ExistsAsync(x =>
                x.MailAddress == tickerUser &&
                x.Secret == secret);

        return exists;
    }


    public async ValueTask InsertAsync(TickerUser @object)
    {
        TickerUserValidation.EnsureInsertable(@object);

        await _repository.InsertAsync(@object);
    }
}
