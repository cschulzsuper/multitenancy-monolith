using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;

public sealed class DisplayNameAttribute : ValidationAttribute
{
    public ValidationResult? _validationResult;

    public override bool IsValid(object? value)
    {
        if (value is not string displayName) { return false; }

        _validationResult = DisplayNameValidator.Validate(displayName);

        return _validationResult == ValidationResult.Success;
    }

    public override string FormatErrorMessage(string name)
        => _validationResult?.ErrorMessage ?? base.FormatErrorMessage(name);
}