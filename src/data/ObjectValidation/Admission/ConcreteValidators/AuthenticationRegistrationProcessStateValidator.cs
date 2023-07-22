using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Admission.ConcreteValidators;

public sealed class AuthenticationRegistrationProcessStateValidator
{
    private readonly static Validator<string> _validator;

    static AuthenticationRegistrationProcessStateValidator()
    {
        _validator = new Validator<string>();
        _validator.AddRules(x => x, CreateRules());
    }

    public static IValidationRule<string>[] CreateRules(string field = "authentication registration process state")
    {
        var rules = new IValidationRule<string>[]
        {
        new NotNull(field),
        new NotEmpty(field),
        new Allowed(field, AuthenticationRegistrationProcessStates.All),
        };

        return rules;
    }

    public static void Ensure(string authenticationRegistrationProcessState)
        => _validator.Ensure(authenticationRegistrationProcessState);

    public static ValidationResult? Validate(string authenticationRegistrationProcessState)
        => _validator.Validate(authenticationRegistrationProcessState);
}