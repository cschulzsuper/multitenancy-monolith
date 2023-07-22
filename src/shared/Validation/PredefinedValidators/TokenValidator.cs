using System;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

public sealed class TokenValidator
{
    private readonly static Validator<Guid> _validator;

    static TokenValidator()
    {
        _validator = new Validator<Guid>();
        _validator.AddRules(x => x, CreateRules());
    }

    public static IValidationRule<Guid>[] CreateRules(string field = "token")
    {
        var rules = new IValidationRule<Guid>[]
        {
            ValidationRules.NotDefault<Guid>(field),
        };

        return rules;
    }

    public static void Ensure(Guid value)
        => _validator.Ensure(value);
}