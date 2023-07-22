using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Schedule.ConcreteValidators;

public sealed class JobTypeValidator
{
    private readonly static Validator<string> _validator;

    static JobTypeValidator()
    {
        _validator = new Validator<string>();
        _validator.AddRules(x => x, CreateRules());
    }

    public static IValidationRule<string>[] CreateRules(string field = "job type")
    {
        var rules = new IValidationRule<string>[]
        {
            ValidationRules.NotNull(field),
            ValidationRules.NotEmpty(field),
            ValidationRules.AllowedValues(field, "cron-expression"),
        };

        return rules;
    }

    public static void Ensure(string jobType)
        => _validator.Ensure(jobType);

    public static ValidationResult? Validate(string jobType)
        => _validator.Validate(jobType);
}