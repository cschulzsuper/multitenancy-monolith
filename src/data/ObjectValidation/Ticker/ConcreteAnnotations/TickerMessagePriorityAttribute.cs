using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteAnnotations;

public sealed class TickerMessagePriorityAttribute : ValidationAttribute
{
    private readonly static Validator<string> _validator;

    static TickerMessagePriorityAttribute()
    {
        _validator = new Validator<string>();
        _validator.AddRules(x => x, TickerMessagePriorityValidator.CreateRules("{0}"));
    }

    public ValidationResult? _validationResult;

    public string _field = "ticket message priority";

    public override bool IsValid(object? value)
    {
        if (value is not string tickerMessagePriority)
        {
            return false;
        }

        _validationResult = _validator.Validate(tickerMessagePriority);

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