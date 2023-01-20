using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;
using System.Numerics;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

public sealed class ZeroValueValidator<T>
    where T : INumber<T>
{
    private readonly static Validator<T> _validator;

    static ZeroValueValidator()
    {
        _validator = new Validator<T>();
        _validator.AddRules(x => x, CreateRules());
    }

    public static IValidationRule<T>[] CreateRules(string field = "value")
    {
        var rules = new IValidationRule<T>[]
        {
        new Zero<T>(field),
        };

        return rules;
    }

    public static void Ensure(T value)
        => _validator.Ensure(value);
}