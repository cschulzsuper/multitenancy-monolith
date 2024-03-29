﻿namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;

public sealed class StringLengthValidationRule : IValidationRule<string>
{
    private readonly int _length;

    private readonly string _validationMessage;

    internal StringLengthValidationRule(string field, int length)
    {
        _length = length;

        _validationMessage = string.Format(ValidationErrors.ValueTooLong, field, length);
    }

    public string ValidationMessage => _validationMessage;

    public bool Check(string value)
        => value.Length <= _length;
}