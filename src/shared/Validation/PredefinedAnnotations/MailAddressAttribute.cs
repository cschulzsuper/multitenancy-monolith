using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;

public sealed class MailAddressAttribute : ValidationAttribute
{
    public ValidationResult? _validationResult;

    public override bool IsValid(object? value)
    {
        if (value is not string uniqueName) { return false; }

        _validationResult = MailAddressValidator.Validate(uniqueName);

        return _validationResult == ValidationResult.Success;
    }

    public override string FormatErrorMessage(string name)
        => _validationResult?.ErrorMessage ?? base.FormatErrorMessage(name);
}