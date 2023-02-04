namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;

public sealed class NotNull : IValidationRule<string>
{
    private readonly string _validationMessage;

    public NotNull(string field)
    {
        _validationMessage = string.Format(ValidationErrors.ValueIsNull, field);
    }

    public string ValidationMessage => _validationMessage;

    public bool Check(string value)
        => value != null;
}