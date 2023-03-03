using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

internal static class TickerBookmarkValidation
{
    private readonly static Validator<TickerBookmark> _insertValidator;
    private readonly static Validator<TickerBookmark> _updateValidator;

    private readonly static Validator<long> _tickerBookmarkValidator;
    private readonly static Validator<long> _tickerMessageValidator;
    private readonly static Validator<string> _tickerUserValidator;

    static TickerBookmarkValidation()
    {
        _insertValidator = new Validator<TickerBookmark>();
        _insertValidator.AddRules(x => x.Snowflake, ZeroValidator<long>.CreateRules("snowflake"));
        _insertValidator.AddRules(x => x.TickerUser, MailAddressValidator.CreateRules("ticker user"));
        _insertValidator.AddRules(x => x.TickerMessage, SnowflakeValidator.CreateRules("ticker message"));

        _updateValidator = new Validator<TickerBookmark>();
        _updateValidator.AddRules(x => x.Snowflake, SnowflakeValidator.CreateRules());
        _updateValidator.AddRules(x => x.TickerUser, MailAddressValidator.CreateRules("ticker user"));
        _updateValidator.AddRules(x => x.TickerMessage, SnowflakeValidator.CreateRules("ticker message"));

        _tickerBookmarkValidator = new Validator<long>();
        _tickerBookmarkValidator.AddRules(x => x, SnowflakeValidator.CreateRules("ticker bookmark"));

        _tickerMessageValidator = new Validator<long>();
        _tickerMessageValidator.AddRules(x => x, SnowflakeValidator.CreateRules("ticker message"));

        _tickerUserValidator = new Validator<string>();
        _tickerUserValidator.AddRules(x => x, MailAddressValidator.CreateRules("ticker user"));
    }

    internal static void EnsureInsertable(TickerBookmark @object)
        => _insertValidator.Ensure(@object);

    public static void EnsureUpdatable(TickerBookmark @object)
        => _updateValidator.Ensure(@object);

    internal static void EnsureTickerBookmark(long tickerBookmark)
        => _tickerBookmarkValidator.Ensure(tickerBookmark);

    internal static void EnsureTickerUser(string tickerUser)
        => _tickerUserValidator.Ensure(tickerUser);

    internal static void EnsureTickerMessage(long tickerMessage)
        => _tickerMessageValidator.Ensure(tickerMessage);
}