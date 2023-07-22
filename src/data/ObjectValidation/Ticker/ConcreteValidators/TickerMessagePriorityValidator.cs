using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;

public sealed class TickerMessagePriorityValidator
{
    private readonly static Validator<string> _validator;

    static TickerMessagePriorityValidator()
    {
        _validator = new Validator<string>();
        _validator.AddRules(x => x, CreateRules());
    }

    public static IValidationRule<string>[] CreateRules(string field = "ticket message priority")
    {
        var rules = new IValidationRule<string>[]
        {
        new NotNull(field),
        new NotEmpty(field),
        new Allowed(field, TickerMessagePriorities.All),
        };

        return rules;
    }

    public static void Ensure(string tickerMessagePriority)
        => _validator.Ensure(tickerMessagePriority);

    public static ValidationResult? Validate(string tickerMessagePriority)
        => _validator.Validate(tickerMessagePriority);
}