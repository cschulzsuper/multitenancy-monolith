using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
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

        _updateValidator = new Validator<TickerMessage>();
        _updateValidator.AddRules(x => x.Snowflake, SnowflakeValidator.CreateRules());
        _updateValidator.AddRules(x => x.TickerUser, MailAddressValidator.CreateRules("ticket user"));
    }

    internal static void EnsureInsertable(TickerMessage tickerMessage)
        => _insertValidator.Ensure(tickerMessage);

    public static void EnsureUpdatable(TickerMessage tickerMessage)
        => _updateValidator.Ensure(tickerMessage);

    internal static void EnsureSnowflake(long snowflake)
        => SnowflakeValidator.Ensure(snowflake);
}