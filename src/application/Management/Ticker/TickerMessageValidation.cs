using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

internal static class TickerMessageValidation
{
    private static readonly Validator<TickerMessage> _insertValidator;
    private static readonly Validator<TickerMessage> _updateValidator;

    static TickerMessageValidation()
    {
        _insertValidator = new Validator<TickerMessage>();
        _insertValidator.AddRules(x => x.Snowflake, ZeroValidator<long>.CreateRules("snowflake"));
        _insertValidator.AddRules(x => x.TickerUser, MailAddressValidator.CreateRules("ticker user"));
        _insertValidator.AddRules(x => x.Text, TickerMessageTextValidator.CreateRules("text"));
        _insertValidator.AddRules(x => x.Priority, TickerMessagePriorityValidator.CreateRules("priority"));

        _updateValidator = new Validator<TickerMessage>();
        _updateValidator.AddRules(x => x.Snowflake, SnowflakeValidator.CreateRules());
        _updateValidator.AddRules(x => x.TickerUser, MailAddressValidator.CreateRules("ticker user"));
        _updateValidator.AddRules(x => x.Text, TickerMessageTextValidator.CreateRules("text"));
        _updateValidator.AddRules(x => x.Priority, TickerMessagePriorityValidator.CreateRules("priority"));
    }

    public static void EnsureInsertable(TickerMessage @object)
        => _insertValidator.Ensure(@object);

    public static void EnsureUpdatable(TickerMessage @object)
        => _updateValidator.Ensure(@object);

    public static void EnsureTickerMessage(long tickerMessage)
        => SnowflakeValidator.Ensure(tickerMessage);
}