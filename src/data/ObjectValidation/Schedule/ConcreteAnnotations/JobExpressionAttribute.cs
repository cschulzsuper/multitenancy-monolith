using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Schedule.ConcreteValidators;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Schedule.ConcreteAnnotations;

public sealed class JobExpressionAttribute : ValidationAttribute
{
    public ValidationResult? _validationResult;

    public override bool IsValid(object? value)
    {
        if (value is not string jobExpression) { return false; }

        _validationResult = JobTypeValidator.Validate(jobExpression);

        return _validationResult == ValidationResult.Success;
    }

    public override string FormatErrorMessage(string name)
        => _validationResult?.ErrorMessage ?? base.FormatErrorMessage(name);
}