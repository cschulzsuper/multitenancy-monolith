using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

public sealed class UniqueNameValidator
{
    private readonly static Validator<string> _validator;

    static UniqueNameValidator()
    {
        _validator = new Validator<string>();
        _validator.AddRules(x => x, CreateRules());
    }

    public static IValidationRule<string>[] CreateRules(string field = "unique name")
    {
        var rules = new IValidationRule<string>[]
        {
            new NotNull(field),
            new NotEmpty(field),
            new KebabCase(field),
            new StringLength(field, 140),
        };

        return rules;
    }

    public static void Ensure(string uniqueName)
        => _validator.Ensure(uniqueName);

    public static ValidationResult? Validate(string uniqueName)
        => _validator.Validate(uniqueName);
}