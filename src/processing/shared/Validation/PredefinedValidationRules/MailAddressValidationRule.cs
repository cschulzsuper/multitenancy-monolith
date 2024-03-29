﻿namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;

public sealed partial class MailAddressValidationRule : IValidationRule<string>
{
    private readonly string _validationMessage;

    internal MailAddressValidationRule(string field)
    {
        _validationMessage = string.Format(ValidationErrors.ValueNotMailAddress, field);
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