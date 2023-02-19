using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;

public sealed class TokenAttribute : ValidationAttribute
{
    private readonly static Validator<string> _validator;

    static TokenAttribute()
    {
        _validator = new Validator<string>();
        _validator.AddRules(x => x, SecretValidator.CreateRules("{0}"));
    }

    public ValidationResult? _validationResult;

    public string _field = "token";

    public override bool IsValid(object? value)
    {
        if (value is not string token)
        {
            return false;
        }

        _validationResult = _validator.Validate(token);

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
