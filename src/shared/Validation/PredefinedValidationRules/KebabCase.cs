using System.Text.RegularExpressions;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;

public sealed partial class KebabCase : IValidationRule<string>
{
    private readonly string _validationMessage;

    public KebabCase(string field)
    {
        _validationMessage = string.Format(ValidationErrors.ValueNotKebabCased, field);
    }

    public string ValidationMessage => _validationMessage;

    public bool Check(string value)
        => KebabCaseRegex().IsMatch(value);

    [GeneratedRegex("^[a-z0-9-]*$")]
    private static partial Regex KebabCaseRegex();
}