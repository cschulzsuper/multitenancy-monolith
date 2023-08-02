﻿using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;

public sealed class TickerUserSecretStateValidator
{
    private static readonly Validator<string> _validator;

    static TickerUserSecretStateValidator()
    {
        _validator = new Validator<string>();
        _validator.AddRules(x => x, CreateRules());
    }

    public static IValidationRule<string>[] CreateRules(string field = "ticker user secret state")
    {
        var rules = new IValidationRule<string>[]
        {
        ValidationRules.NotNull(field),
        ValidationRules.NotEmpty(field),
        ValidationRules.AllowedValues(field, TickerUserSecretStates.All),
        };

        return rules;
    }

    public static void Ensure(string tickerUserSecretState)
        => _validator.Ensure(tickerUserSecretState);

    public static ValidationResult? Validate(string tickerUserSecretState)
        => _validator.Validate(tickerUserSecretState);
}