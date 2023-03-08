using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;

public sealed class AccountRegistrationProcessStateValidator
{
    private readonly static Validator<string> _validator;

    static AccountRegistrationProcessStateValidator()
    {
        _validator = new Validator<string>();
        _validator.AddRules(x => x, CreateRules());
    }

    public static IValidationRule<string>[] CreateRules(string field = "account registration process state")
    {
        var rules = new IValidationRule<string>[]
        {
            new NotNull(field),
            new NotEmpty(field),
            new Allowed(field, AccountRegistrationProcessStates.All),
        };

        return rules;
    }

    public static void Ensure(string accountRegistrationProcessState)
        => _validator.Ensure(accountRegistrationProcessState);

    public static ValidationResult? Validate(string accountRegistrationProcessState)
        => _validator.Validate(accountRegistrationProcessState);
}