using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;

public sealed class TickerUserSecretStatesValidator
{
    private readonly static Validator<string> _validator;

    static TickerUserSecretStatesValidator()
    {
        _validator = new Validator<string>();
        _validator.AddRules(x => x, CreateRules());
    }

    public static IValidationRule<string>[] CreateRules(string field = "ticker user secret state")
    {
        var rules = new IValidationRule<string>[]
        {
            new NotNull(field),
            new NotEmpty(field),
            new Allowed(field, TickerUserSecretStates.All),
        };

        return rules;
    }

    public static void Ensure(string tickerMessagePriority)
        => _validator.Ensure(tickerMessagePriority);

    public static ValidationResult? Validate(string tickerMessagePriority)
        => _validator.Validate(tickerMessagePriority);
}