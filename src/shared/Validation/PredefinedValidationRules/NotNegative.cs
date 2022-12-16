using System.Numerics;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;

public sealed class NotNegative<T> : IValidationRule<T>
    where T : INumber<T>
{
    private readonly string _validationMessage;

    public NotNegative(string field)
    {
        _validationMessage = $"Field '{field}' cannot be negative!";
    }

    public string ValidationMessage => _validationMessage;

    public bool Check(T value)
        => !T.IsNegative(value);
}
