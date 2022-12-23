using System.Text.RegularExpressions;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;

public sealed partial class MailAddress : IValidationRule<string>
{
    private readonly string _validationMessage;

    public MailAddress(string field)
    {
        _validationMessage = $"Field '{field}' must be a valid mail address!";
    }

    public string ValidationMessage => _validationMessage;

    public bool Check(string value)
        => System.Net.Mail.MailAddress.TryCreate(value, out _);
}
