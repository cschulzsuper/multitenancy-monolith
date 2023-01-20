using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation;

public sealed class Validator<T>
{
    public readonly IList<Func<T, ValidationResult?>> _validations = new List<Func<T, ValidationResult?>>();

    public void AddRule<TProperty>(Func<T, TProperty> property, IValidationRule<TProperty> rule)
    {
        var validation = CreateValidation(property, rule);

        _validations.Add(validation);
    }

    public void AddRules<TProperty>(Func<T, TProperty> property, params IValidationRule<TProperty>[] rules)
    {
        foreach (var rule in rules)
        {
            AddRule(property, rule);
        }
    }

    public Func<T, ValidationResult?> CreateValidation<TProperty>(Func<T, TProperty> property, IValidationRule<TProperty> rule)
        => (value) =>
        {
            var valid = rule.Check(property.Invoke(value));

            return valid
                ? ValidationResult.Success
                : new ValidationResult(rule.ValidationMessage);
        };

    public void Ensure(T value)
    {
        foreach (var validation in _validations)
        {
            var validationResult = validation.Invoke(value);

            if (validationResult != null &&
                validationResult != ValidationResult.Success)
            {
                throw new ValidationException(validationResult.ErrorMessage
                    ?? $"The '{typeof(T).Name}' value is not valid.");
            }
        }
    }
}