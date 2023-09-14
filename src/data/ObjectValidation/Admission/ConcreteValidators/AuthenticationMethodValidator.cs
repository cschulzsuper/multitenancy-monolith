using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Admission.ConcreteValidators;

public sealed class AuthenticationMethodValidator
{
    private static readonly Validator<string> _validator;

    static AuthenticationMethodValidator()
    {
        _validator = new Validator<string>();
        _validator.AddRules(x => x, CreateRules());
    }

    public static IValidationRule<string>[] CreateRules(string field = "authentication method")
    {
        var rules = new IValidationRule<string>[]
        {
            ValidationRules.NotNull(field),
            ValidationRules.NotEmpty(field),
            ValidationRules.AllowedValues(field, AuthenticationMethods.All)
        };

        return rules;
    }

    public static void Ensure(string authenticationMethod)
        => _validator.Ensure(authenticationMethod);

    public static ValidationResult? Validate(string authenticationMethod)
        => _validator.Validate(authenticationMethod);
}