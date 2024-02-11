using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;

public sealed class LowerCaseValidationRule : IValidationRule<string>
{
    private readonly string _validationMessage;

    internal LowerCaseValidationRule(string field)
    {
        _validationMessage = string.Format(ValidationErrors.ValueIsEmpty, field);
    }

    public string ValidationMessage => _validationMessage;

    public bool Check(string value)
        => !value.Any(x => char.IsLetter(x) && char.IsUpper(x));
}