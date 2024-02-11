using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Admission.ConcreteValidators;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Admission.ConcreteAnnotations;

public sealed class AuthenticationMethodAttribute : ValidationAttribute
{
    public ValidationResult? _validationResult;

    public override bool IsValid(object? value)
    {
        if (value is not string authenticationMethod) { return false; }

        _validationResult = AuthenticationMethodValidator.Validate(authenticationMethod);

        return _validationResult == ValidationResult.Success;
    }

    public override string FormatErrorMessage(string name)
        => _validationResult?.ErrorMessage ?? base.FormatErrorMessage(name);
}