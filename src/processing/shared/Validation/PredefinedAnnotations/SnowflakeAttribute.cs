using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;

public sealed class SnowflakeAttribute : ValidationAttribute
{
    private static readonly Validator<long> _validator;

    static SnowflakeAttribute()
    {
        _validator = new Validator<long>();
        _validator.AddRules(x => x, SnowflakeValidator.CreateRules("{0}"));
    }

    public ValidationResult? _validationResult;

    public string _field = "snowflake";

    public override bool IsValid(object? value)
    {
        if (value is not long snowflake)
        {
            return false;
        }

        _validationResult = _validator.Validate(snowflake);

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