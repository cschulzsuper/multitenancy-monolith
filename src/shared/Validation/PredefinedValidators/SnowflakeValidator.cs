namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

public sealed class SnowflakeValidator
{
    private readonly static Validator<long> _validator;

    static SnowflakeValidator()
    {
        _validator = new Validator<long>();
        _validator.AddRules(x => x, CreateRules());
    }

    public static IValidationRule<long>[] CreateRules(string field = "snowflake")
    {
        var rules = new IValidationRule<long>[]
        {
            ValidationRules.NotZero<long>(field),
            ValidationRules.NotNegative<long>(field)
        };

        return rules;
    }

    public static void Ensure(long value)
        => _validator.Ensure(value);
}