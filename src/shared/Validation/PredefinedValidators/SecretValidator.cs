using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;

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
        new NotNull(field),
        new NotEmpty(field),
        };

        return rules;
    }

    public static void Ensure(string value)
        => _validator.Ensure(value);
}