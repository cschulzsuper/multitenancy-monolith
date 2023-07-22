using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Extension.ConcreteValidators;

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
            ValidationRules.NotNull(field),
            ValidationRules.NotEmpty(field),
            ValidationRules.AllowedValues(field, "business-object"),
        };

        return rules;
    }

    public static void Ensure(string objectType)
        => _validator.Ensure(objectType);

    public static ValidationResult? Validate(string objectType)
        => _validator.Validate(objectType);
}