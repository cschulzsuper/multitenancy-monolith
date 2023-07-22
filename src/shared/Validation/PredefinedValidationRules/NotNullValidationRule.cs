namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;

public sealed class NotNullValidationRule : IValidationRule<string>
{
    private readonly string _validationMessage;

    internal NotNullValidationRule(string field)
    {
        _validationMessage = string.Format(ValidationErrors.ValueIsNull, field);
    }

    public string ValidationMessage => _validationMessage;

    public bool Check(string value)
        => value != null;
}