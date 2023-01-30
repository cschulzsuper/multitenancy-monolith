using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Administration.ConcreteValidators;

public sealed class ObjectTypeValidator
{
    private readonly static Validator<string> _validator;

    static ObjectTypeValidator()
    {
        _validator = new Validator<string>();
        _validator.AddRules(x => x, CreateRules());
    }

    public static IValidationRule<string>[] CreateRules(string field = "object type")
    {
        var rules = new IValidationRule<string>[]
        {
            new NotNull(field),
            new NotEmpty(field),
            new Allowed(field, "business-object"),
        };

        return rules;
    }

    public static void Ensure(string objectType)
        => _validator.Ensure(objectType);

    public static ValidationResult? Validate(string objectType)
        => _validator.Validate(objectType);
}