using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;

public sealed class SecretAttribute : ValidationAttribute
{
    private readonly static Validator<string> _validator;

    static SecretAttribute()
    {
        _validator = new Validator<string>();
        _validator.AddRules(x => x, SecretValidator.CreateRules("{0}"));
    }

    public ValidationResult? _validationResult;

    public string _field = "secret";

    public override bool IsValid(object? value)
    {
        if (value is not string secret)
        {
            return false;
        }

        _validationResult = _validator.Validate(secret);

        return _validationResult == ValidationResult.Success;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (validationContext.DisplayName == validationContext.MemberName)
        {
            validationContext.DisplayName = _field;
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