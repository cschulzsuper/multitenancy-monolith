using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;

public sealed class LowerCase : IValidationRule<string>
{
    private readonly string _validationMessage;

    public LowerCase(string field)
    {
        _validationMessage = string.Format(ValidationErrors.ValueIsEmpty, field);
    }

    public string ValidationMessage => _validationMessage;

    public bool Check(string value)
        => !value.Any(x => char.IsLetter(x) && char.IsUpper(x));
}