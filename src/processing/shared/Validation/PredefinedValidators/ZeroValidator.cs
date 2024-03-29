﻿using System.Numerics;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

public sealed class ZeroValidator<T>
    where T : INumber<T>
{
    private static readonly Validator<T> _validator;

    static ZeroValidator()
    {
        _validator = new Validator<T>();
        _validator.AddRules(x => x, CreateRules());
    }

    public static IValidationRule<T>[] CreateRules(string field = "value")
    {
        var rules = new IValidationRule<T>[]
        {
            ValidationRules.Zero<T>(field),
        };

        return rules;
    }

    public static void Ensure(T value)
        => _validator.Ensure(value);
}