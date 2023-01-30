using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Administration.ConcreteValidators;

public sealed class CustomPropertyTypeValidator
{
    private readonly static Validator<string> _validator;

    static CustomPropertyTypeValidator()
    {
        _validator = new Validator<string>();
        _validator.AddRules(x => x, CreateRules());
    }

    public static IValidationRule<string>[] CreateRules(string field = "custom property type")
    {
        var rules = new IValidationRule<string>[]
        {
            new NotNull(field),
            new NotEmpty(field),
            new Allowed(field, "string"),
        };

        return rules;
    }

    public static void Ensure(string customPropertyType)
        => _validator.Ensure(customPropertyType);

    public static ValidationResult? Validate(string customPropertyType)
        => _validator.Validate(customPropertyType);
}