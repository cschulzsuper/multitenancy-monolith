using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

public sealed class UniqueConstrainValidator<T>
    where T : class
{
    private static readonly Validator<IEnumerable<T>> _validator;

    static UniqueConstrainValidator()
    {
        _validator = new Validator<IEnumerable<T>>();
        _validator.AddRules(x => x, CreateRules(x => x.GetHashCode()));
    }

    public static IValidationRule<IEnumerable<T>>[] CreateRules(Func<T, IComparable> select, string field = "value")
    {
        var rules = new IValidationRule<IEnumerable<T>>[]
        {
            ValidationRules.Unique(field, select)
        };

        return rules;
    }

    public static void Ensure(IEnumerable<T> enumerable)
        => _validator.Ensure(enumerable);

    public static ValidationResult? Validate(IEnumerable<T> enumerable)
        => _validator.Validate(enumerable);
}