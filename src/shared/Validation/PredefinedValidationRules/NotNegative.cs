using System.Numerics;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;

public sealed class NotNegative<T> : IValidationRule<T>
    where T : INumber<T>
{
    private readonly string _validationMessage;

    public NotNegative(string field)
    {
        _validationMessage = string.Format(ValidationErrors.ValueIsNegative, field);
    }

    public string ValidationMessage => _validationMessage;

    public bool Check(T value)
        => !T.IsNegative(value);
}