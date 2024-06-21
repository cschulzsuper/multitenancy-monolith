using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation
{
    internal class ValidationRuleResult : ValidationResult
    {
        public ValidationRuleResult(IValidationRule rule, string? errorMessage)
            : base(errorMessage)
        {
            Rule = rule;
        }

        public ValidationRuleResult(IValidationRule rule, string? errorMessage, IEnumerable<string>? memberNames) 
            : base(errorMessage, memberNames)
        {
            Rule = rule;
        }

        protected ValidationRuleResult(IValidationRule rule, ValidationResult validationResult) 
            : base(validationResult)
        {
            Rule = rule;
        }

        public IValidationRule Rule { get; }
    }
}
