using System;
using System.Collections.Generic;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;

public sealed class UniquePropertyValueValidationRule<T> : IValidationRule<IEnumerable<T>>
{
    private readonly string _validationMessage;
    private readonly Func<T, IComparable> _select;

    internal UniquePropertyValueValidationRule(string field, Func<T, IComparable> select)
    {
        _validationMessage = string.Format(ValidationErrors.ValueNotUnique, field);
        _select = select;
    }

    public string ValidationMessage => _validationMessage;

    public bool Check(IEnumerable<T> value)
        => value.Select(_select).Count() == value.Select(_select).Distinct().Count();
}