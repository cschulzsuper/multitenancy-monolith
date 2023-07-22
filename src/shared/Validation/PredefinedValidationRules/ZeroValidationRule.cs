using System.Numerics;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;

public sealed class ZeroValidationRule<T> : IValidationRule<T>
    where T : INumber<T>
{
    private readonly string _validationMessage;

    internal ZeroValidationRule(string field)
    {
        _validationMessage = string.Format(ValidationErrors.ValueNotZero, field);
    }

    public string ValidationMessage => _validationMessage;

    public bool Check(T value)
        => T.IsZero(value);
}