﻿using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

internal static class TickerUserValidation
{
    private readonly static Validator<TickerUser> _insertValidator;
    private readonly static Validator<TickerUser> _updateValidator;

    private readonly static Validator<long> _tickerUserSnowflakeValidator;
    private readonly static Validator<string> _tickerUserMailAddressValidator;

    static TickerUserValidation()
    {
        _insertValidator = new Validator<TickerUser>();
        _insertValidator.AddRules(x => x.Snowflake, ZeroValidator<long>.CreateRules("snowflake"));
        _insertValidator.AddRules(x => x.MailAddress, MailAddressValidator.CreateRules());
        _insertValidator.AddRules(x => x.Secret, SecretValidator.CreateRules());
        _insertValidator.AddRules(x => x.SecretState, TickerUserSecretStatesValidator.CreateRules("secret state"));
        _insertValidator.AddRules(x => x.SecretToken, TokenValidator.CreateRules("secret token"));
        _insertValidator.AddRules(x => x.DisplayName, DisplayNameValidator.CreateRules());

        _updateValidator = new Validator<TickerUser>();
        _updateValidator.AddRules(x => x.Snowflake, SnowflakeValidator.CreateRules());
        _updateValidator.AddRules(x => x.MailAddress, MailAddressValidator.CreateRules());
        _updateValidator.AddRules(x => x.Secret, SecretValidator.CreateRules());
        _updateValidator.AddRules(x => x.SecretState, TickerUserSecretStatesValidator.CreateRules("secret state"));
        _updateValidator.AddRules(x => x.SecretToken, TokenValidator.CreateRules("secret token"));
        _updateValidator.AddRules(x => x.DisplayName, DisplayNameValidator.CreateRules());

        _tickerUserSnowflakeValidator = new Validator<long>();
        _tickerUserSnowflakeValidator.AddRules(x => x, SnowflakeValidator.CreateRules("ticker user"));

        _tickerUserMailAddressValidator = new Validator<string>();
        _tickerUserMailAddressValidator.AddRules(x => x, MailAddressValidator.CreateRules("ticker user"));
    }

    internal static void EnsureInsertable(TickerUser ticketUser)
        => _insertValidator.Ensure(ticketUser);

    public static void EnsureUpdatable(TickerUser ticketUser)
        => _updateValidator.Ensure(ticketUser);

    internal static void EnsureTickerUser(long ticketUser)
        => _tickerUserSnowflakeValidator.Ensure(ticketUser);

    internal static void EnsureTickerUser(string ticketUser)
        => _tickerUserMailAddressValidator.Ensure(ticketUser);

}