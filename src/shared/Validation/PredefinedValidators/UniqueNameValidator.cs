using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;

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
        };

        return rules;
    }

    public static void Ensure(string uniqueName)
        => _validator.Ensure(uniqueName);
}