using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Administration.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Administration.ConcreteAnnotations;

public sealed class CustomPropertyTypeAttribute : ValidationAttribute
{
    private readonly static Validator<string> _validator;

    static CustomPropertyTypeAttribute()
    {
        _validator = new Validator<string>();
        _validator.AddRules(x => x, CustomPropertyNameValidator.CreateRules("{0}"));
    }

    public ValidationResult? _validationResult;

    public string _field = "custome property type";

    public override bool IsValid(object? value)
    {
        if (value is not string customPropertyType)
        {
            return false;
        }

        _validationResult = _validator.Validate(customPropertyType);

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
