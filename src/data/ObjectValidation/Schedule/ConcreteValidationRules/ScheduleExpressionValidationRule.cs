using ChristianSchulz.MultitenancyMonolith.Shared.Validation;
using System;
namespace ChristianSchulz.MultitenancyMonolith.ObjectValidation.Schedule.ConcreteValidationRules
{
    public sealed class ScheduleExpressionValidationRule<T> : IValidationRule<T>
    {
        private readonly Func<T, string> _expressionTypeSelector;
        private readonly Func<T, string> _expressionSelector;

        private readonly IValidationRule<string> _cronExpression;

        public ScheduleExpressionValidationRule(string field, Func<T, string> expressionTypeSelector, Func<T, string> expressionSelector)
        {
            ValidationMessage = string.Format(ValidationErrors.ValueNotValidatable, field);

            _expressionTypeSelector = expressionTypeSelector;
            _expressionSelector = expressionSelector;

            _cronExpression = ValidationRules.CronExpression(field);
        }

        public string ValidationMessage { get; private set; }

        public bool Check(T value)
        {
            var expressionType = _expressionTypeSelector(value);
            var expression = _expressionSelector(value);

            var result = expressionType switch
            {
                "cron-expression" => _cronExpression.Check(expression),

                _ => false
            };

            return result;
        }

        public bool CheckCronExpression(string expression)
        {
            var result = _cronExpression.Check(expression);

            if (result == false)
            {
                ValidationMessage = _cronExpression.ValidationMessage;
            }

            return result;
        }
    }
}
