using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Extension.ConcreteValidators;

public sealed class CustomPropertyNameValidator
{
    private readonly static Validator<string> _validator;

    static CustomPropertyNameValidator()
    {
        _validator = new Validator<string>();
        _validator.AddRules(x => x, CreateRules());
    }

    public static IValidationRule<string>[] CreateRules(string field = "custom property name")
    {
        var rules = new IValidationRule<string>[]
        {
            new NotNull(field),
            new NotEmpty(field),
            new CamelCase(field),
            new StringLength(field, 140),
        };

        return rules;
    }

    public static void Ensure(string customPropertyName)
        => _validator.Ensure(customPropertyName);

    public static ValidationResult? Validate(string customPropertyName)
        => _validator.Validate(customPropertyName);
}