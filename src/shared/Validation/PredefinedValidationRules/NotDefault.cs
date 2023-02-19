using System;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;

public sealed class NotDefault<T> : IValidationRule<T>
    where T : struct, IEquatable<T>
{
    private readonly string _validationMessage;

    public NotDefault(string field)
    {
        _validationMessage = string.Format(ValidationErrors.ValueIsDefault, field);
    }

    public string ValidationMessage => _validationMessage;

    public bool Check(T value)
        => !value.Equals(default);
}