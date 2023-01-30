using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;

public sealed class Allowed : IValidationRule<string>
{
    private readonly object[] _allowedValues;

    private readonly string _validationMessage;
    
    public Allowed(string field, params object[] allowedValues)
    {
        _allowedValues = allowedValues;

        _validationMessage = $"The value for '{field}' is not allowed!";
    }

    public string ValidationMessage => _validationMessage;

    public bool Check(string value)
        => _allowedValues.Contains(value);
}