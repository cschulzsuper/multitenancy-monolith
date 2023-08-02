using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

public sealed class RequiredValidator
{
    private static readonly Validator<string> _validator;

    static RequiredValidator()
    {
        _validator = new Validator<string>();
        _validator.AddRules(x => x, CreateRules());
    }

    public static IValidationRule<string>[] CreateRules(string field = "string")
    {
        var rules = new IValidationRule<string>[]
        {
            ValidationRules.NotNull(field),
            ValidationRules.NotEmpty(field),
        };

        return rules;
    }

    public static void Ensure(string value)
        => _validator.Ensure(value);

    public static ValidationResult? Validate(string value)
        => _validator.Validate(value);
}