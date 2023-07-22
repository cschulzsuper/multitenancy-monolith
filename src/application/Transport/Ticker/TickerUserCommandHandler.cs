using ChristianSchulz.MultitenancyMonolith.Events;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;
using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

internal sealed class TickerUserCommandHandler : ITickerUserCommandHandler
{
    private readonly ITickerUserManager _tickerUserManager;
    private readonly IEventStorage _eventStorage;

    public TickerUserCommandHandler(
        ITickerUserManager tickerUserManager,
        IEventStorage eventStorage)
    {
        _tickerUserManager = tickerUserManager;
        _eventStorage = eventStorage;
    }

    public async Task ResetAsync(long tickerUser)
    {
        var updateAction = (TickerUser @object) =>
        {
            @object.Secret = $"{Guid.NewGuid()}";
            @object.SecretState = TickerUserSecretStates.Reset;
            @object.SecretToken = Guid.NewGuid();

            _eventStorage.Add("ticker-user-secret-reset", @object.Snowflake);
        };

        await _tickerUserManager.UpdateAsync(tickerUser, updateAction);
    }
}