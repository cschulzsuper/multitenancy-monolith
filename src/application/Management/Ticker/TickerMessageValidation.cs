using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

internal static class TickerMessageValidation
{
    private readonly static Validator<TickerMessage> _insertValidator;
    private readonly static Validator<TickerMessage> _updateValidator;

    static TickerMessageValidation()
    {
        _insertValidator = new Validator<TickerMessage>();
        _insertValidator.AddRules(x => x.Snowflake, ZeroValidator<long>.CreateRules("snowflake"));
        _insertValidator.AddRules(x => x.TickerUser, MailAddressValidator.CreateRules("ticket user"));
        _insertValidator.AddRules(x => x.Text, TickerMessageTextValidator.CreateRules("text"));
        _insertValidator.AddRules(x => x.Priority, TickerMessagePriorityValidator.CreateRules("priority"));

        _updateValidator = new Validator<TickerMessage>();
        _updateValidator.AddRules(x => x.Snowflake, SnowflakeValidator.CreateRules());
        _updateValidator.AddRules(x => x.TickerUser, MailAddressValidator.CreateRules("ticket user"));
        _updateValidator.AddRules(x => x.Text, TickerMessageTextValidator.CreateRules("text"));
        _updateValidator.AddRules(x => x.Priority, TickerMessagePriorityValidator.CreateRules("priority"));
    }

    internal static void EnsureInsertable(TickerMessage tickerMessage)
        => _insertValidator.Ensure(tickerMessage);

    public static void EnsureUpdatable(TickerMessage tickerMessage)
        => _updateValidator.Ensure(tickerMessage);

    internal static void EnsureSnowflake(long snowflake)
        => SnowflakeValidator.Ensure(snowflake);
}