using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Extension.ConcreteValidators;

public sealed class CustomPropertyTypeValidator
{
    private static readonly Validator<string> _validator;

    static CustomPropertyTypeValidator()
    {
        _validator = new Validator<string>();
        _validator.AddRules(x => x, CreateRules());
    }

    public static IValidationRule<string>[] CreateRules(string field = "custom property type")
    {
        var rules = new IValidationRule<string>[]
        {
            ValidationRules.NotNull(field),
            ValidationRules.NotEmpty(field),
            ValidationRules.AllowedValues(field, "string"),
        };

        return rules;
    }

    public static void Ensure(string customPropertyType)
        => _validator.Ensure(customPropertyType);

    public static ValidationResult? Validate(string customPropertyType)
        => _validator.Validate(customPropertyType);
}