namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;

public sealed class NotEmptyValidationRule : IValidationRule<string>
{
    private readonly string _validationMessage;

    internal NotEmptyValidationRule(string field)
    {
        _validationMessage = string.Format(ValidationErrors.ValueIsEmpty, field);
    }

    public string ValidationMessage => _validationMessage;

    public bool Check(string value)
        => value != string.Empty && value == value.Trim();
}