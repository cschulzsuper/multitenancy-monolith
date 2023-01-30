using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Administration.ConcreteValidators;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Administration.ConcreteAnnotations;

public sealed class ObjectTypeAttribute : ValidationAttribute
{
    public ValidationResult? _validationResult;

    public override bool IsValid(object? value)
    {
        if (value is not string objectType) { return false; }

        _validationResult = ObjectTypeValidator.Validate(objectType);

        return _validationResult == ValidationResult.Success;
    }

    public override string FormatErrorMessage(string name)
        => _validationResult?.ErrorMessage ?? base.FormatErrorMessage(name);
}
