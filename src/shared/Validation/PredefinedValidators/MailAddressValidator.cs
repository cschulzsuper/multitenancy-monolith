using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidators;

public sealed class MailAddressValidator
{
    private static readonly Validator<string> _validator;

    static MailAddressValidator()
    {
        _validator = new Validator<string>();
        _validator.AddRules(x => x, CreateRules());
    }

    public static IValidationRule<string>[] CreateRules(string field = "mail address")
    {
        var rules = new IValidationRule<string>[]
        {
            ValidationRules.NotNull(field),
            ValidationRules.NotEmpty(field),
            ValidationRules.LowerCase(field),
            ValidationRules.MailAddress(field)
        };

        return rules;
    }

    public static void Ensure(string mailAddress)
        => _validator.Ensure(mailAddress);

    public static ValidationResult? Validate(string mailAddress)
        => _validator.Validate(mailAddress);
}