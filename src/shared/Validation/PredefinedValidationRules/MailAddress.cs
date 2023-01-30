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
    {
        var valid = System.Net.Mail.MailAddress.TryCreate(value, out _);
        if (!valid) 
        {
            return false;
        }

        if (value.Length > 254)
        {
            return false;
        }

        var valueParts = value.Split('@');
        if (valueParts.Length != 2)
        {
            return false;
        }

        if (valueParts[0].Length > 64)
        {
            return false;
        }

        return true;
    }
}