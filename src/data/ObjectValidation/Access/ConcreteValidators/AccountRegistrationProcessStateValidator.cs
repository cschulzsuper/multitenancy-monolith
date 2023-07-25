using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Access.ConcreteValidators;

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
            ValidationRules.NotNull(field),
            ValidationRules.NotEmpty(field),
            ValidationRules.AllowedValues(field, AccountRegistrationProcessStates.All),
        };

        return rules;
    }

    public static void Ensure(string accountRegistrationProcessState)
        => _validator.Ensure(accountRegistrationProcessState);

    public static ValidationResult? Validate(string accountRegistrationProcessState)
        => _validator.Validate(accountRegistrationProcessState);
}