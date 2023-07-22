using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteAnnotations;

public sealed class TickerMessageTextAttribute : ValidationAttribute
{
    private readonly static Validator<string> _validator;

    static TickerMessageTextAttribute()
    {
        _validator = new Validator<string>();
        _validator.AddRules(x => x, TickerMessageTextValidator.CreateRules("{0}"));
    }

    public ValidationResult? _validationResult;

    public string _field = "ticket message text";

    public override bool IsValid(object? value)
    {
        if (value is not string tickerMessageText)
        {
            return false;
        }

        _validationResult = _validator.Validate(tickerMessageText);

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