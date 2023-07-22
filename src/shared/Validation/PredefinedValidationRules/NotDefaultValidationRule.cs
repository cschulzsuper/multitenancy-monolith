using System;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;

public sealed class NotDefaultValidationRule<T> : IValidationRule<T>
    where T : struct, IEquatable<T>
{
    private readonly string _validationMessage;

    internal NotDefaultValidationRule(string field)
    {
        _validationMessage = string.Format(ValidationErrors.ValueIsDefault, field);
    }

    public string ValidationMessage => _validationMessage;

    public bool Check(T value)
        => !value.Equals(default);
}