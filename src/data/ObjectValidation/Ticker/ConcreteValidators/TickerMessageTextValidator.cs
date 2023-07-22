using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;

public sealed class TickerMessageTextValidator
{
    private readonly static Validator<string> _validator;

    static TickerMessageTextValidator()
    {
        _validator = new Validator<string>();
        _validator.AddRules(x => x, CreateRules());
    }

    public static IValidationRule<string>[] CreateRules(string field = "custom property name")
    {
        var rules = new IValidationRule<string>[]
        {
        ValidationRules.NotNull(field),
        ValidationRules.NotEmpty(field),
        ValidationRules.StringLength(field, 4000),
        };

        return rules;
    }

    public static void Ensure(string tickerMessageText)
        => _validator.Ensure(tickerMessageText);

    public static ValidationResult? Validate(string tickerMessageText)
        => _validator.Validate(tickerMessageText);
}