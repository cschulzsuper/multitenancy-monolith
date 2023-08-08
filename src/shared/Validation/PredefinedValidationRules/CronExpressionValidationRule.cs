using Cronos;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;

public sealed class CronExpressionValidationRule : IValidationRule<string>
{
    private readonly string _validationMessage;

    internal CronExpressionValidationRule(string field)
    {
        _validationMessage = string.Format(ValidationErrors.ValueNotCronExpression, field);
    }

    public string ValidationMessage => _validationMessage;

    public bool Check(string value)
    {
        try
        {
            _ = CronExpression.Parse(value);
            return true;
        } catch
        {
            return false;
        }
    }
}
