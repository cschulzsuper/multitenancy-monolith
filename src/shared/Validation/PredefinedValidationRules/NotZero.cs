using System.Numerics;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;

public sealed class NotZero<T> : IValidationRule<T>
    where T : INumber<T>
{
    private readonly string _validationMessage;

    public NotZero(string field)
    {
        _validationMessage = $"Field '{field}' cannot be zero!";
    }

    public string ValidationMessage => _validationMessage;

    public bool Check(T value)
        => !T.IsZero(value);
}
