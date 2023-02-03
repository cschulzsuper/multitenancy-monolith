using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Administration.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Administration.ConcreteAnnotations;

public sealed class CustomPropertyNameAttribute : ValidationAttribute
{
    private readonly static Validator<string> _validator;

    static CustomPropertyNameAttribute()
    {
        _validator = new Validator<string>();
        _validator.AddRules(x => x, CustomPropertyNameValidator.CreateRules("{0}"));
    }

    public ValidationResult? _validationResult;

    public string _field = "custom property name";

    public override bool IsValid(object? value)
    {
        if (value is not string customPropertyName)
        {
            return false;
        }

        _validationResult = _validator.Validate(customPropertyName);

        return _validationResult == ValidationResult.Success;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (validationContext.DisplayName != validationContext.MemberName)
        {
            _field = validationContext.DisplayName;
        }

        return base.IsValid(value, validationContext);
    }

    public override string FormatErrorMessage(string name)
    {
        var errorMessage = _validationResult?.ErrorMessage;

        return errorMessage != null
            ? string.Format(errorMessage, name)
            : base.FormatErrorMessage(name);
    }
}
