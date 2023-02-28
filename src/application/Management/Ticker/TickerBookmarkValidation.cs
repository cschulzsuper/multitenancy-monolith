using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

internal static class TickerBookmarkValidation
{
    private readonly static Validator<TickerBookmark> _insertValidator;
    private readonly static Validator<TickerBookmark> _updateValidator;

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
    }

    internal static void EnsureInsertable(TickerBookmark @object)
        => _insertValidator.Ensure(@object);

    public static void EnsureUpdatable(TickerBookmark @object)
        => _updateValidator.Ensure(@object);

    internal static void EnsureTickerBookmark(long tickerBookmark)
        => SnowflakeValidator.Ensure(tickerBookmark);
}