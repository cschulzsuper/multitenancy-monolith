namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;

public sealed class NotEmpty : IValidationRule<string>
{
    private readonly string _validationMessage;

    public NotEmpty(string field)
    {
        _validationMessage = $"Field '{field}' cannot be empty!";
    }

    public string ValidationMessage => _validationMessage;

    public bool Check(string value)
        => value != string.Empty;
}
