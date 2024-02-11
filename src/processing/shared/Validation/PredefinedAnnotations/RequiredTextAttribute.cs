using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;

public sealed class RequiredTextAttribute : ValidationAttribute
{
    public ValidationResult? _validationResult;

    public override bool IsValid(object? value)
    {
        if (value is not string text) { return false; }

        _validationResult = RequiredTextValidator.Validate(text);

        return _validationResult == ValidationResult.Success;
    }

    public override string FormatErrorMessage(string name)
        => _validationResult?.ErrorMessage ?? base.FormatErrorMessage(name);
}