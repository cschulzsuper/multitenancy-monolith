using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

public sealed class SecretValidator
{
    private readonly static Validator<string> _validator;

    static SecretValidator()
    {
        _validator = new Validator<string>();
        _validator.AddRules(x => x, CreateRules());
    }

    public static IValidationRule<string>[] CreateRules(string field = "secret")
    {
        var rules = new IValidationRule<string>[]
        {
            ValidationRules.NotNull(field),
            ValidationRules.NotEmpty(field),
            ValidationRules.StringLength(field, 140)
        };

        return rules;
    }

    public static void Ensure(string secret)
        => _validator.Ensure(secret);

    public static ValidationResult? Validate(string secret)
        => _validator.Validate(secret);
}