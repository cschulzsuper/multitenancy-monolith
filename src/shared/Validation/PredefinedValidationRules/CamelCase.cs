using System.Text.RegularExpressions;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;

public sealed partial class CamelCase : IValidationRule<string>
{
    private readonly string _validationMessage;

    public CamelCase(string field)
    {
        _validationMessage = $"Field '{field}' must be camelCased!";
    }

    public string ValidationMessage => _validationMessage;

    public bool Check(string value)
        => CamelCaseRegex().IsMatch(value);

    [GeneratedRegex("^[a-z]+([A-Z0-9][a-z0-9]+)*[A-Za-z0-9]*$")]
    private static partial Regex CamelCaseRegex();
}