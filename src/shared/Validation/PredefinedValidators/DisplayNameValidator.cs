using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

public sealed class DisplayNameValidator
{
    private readonly static Validator<string> _validator;

    static DisplayNameValidator()
    {
        _validator = new Validator<string>();
        _validator.AddRules(x => x, CreateRules());
    }

    public static IValidationRule<string>[] CreateRules(string field = "display name")
    {
        var rules = new IValidationRule<string>[]
        {
            new NotNull(field),
            new NotEmpty(field),
            new StringLength(field, 140),
        };

        return rules;
    }

    public static void Ensure(string displayName)
        => _validator.Ensure(displayName);

    public static ValidationResult? Validate(string displayName)
        => _validator.Validate(displayName);
}