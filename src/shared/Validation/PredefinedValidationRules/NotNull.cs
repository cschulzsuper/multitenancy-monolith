namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;

public sealed class NotNull : IValidationRule<string>
{
    private readonly string _validationMessage;

    public NotNull(string field)
    {
        _validationMessage = $"Field '{field}' cannot be null!";
    }

    public string ValidationMessage => _validationMessage;

    public bool Check(string value)
        => value != null;
}
