﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation;

public sealed class Validator<T>
{
    public readonly IList<Func<T, ValidationResult?>> _validations = [];

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

    public void AddRules<TProperty>(Func<T, IEnumerable<TProperty>> property, Action<Validator<TProperty>> setup)
    {
        var validator = new Validator<TProperty>();

        setup.Invoke(validator);

        var validation = CreateValidation(property, validator);

        _validations.Add(validation);
    }

    public Func<T, ValidationResult?> CreateValidation<TProperty>(Func<T, IEnumerable<TProperty>> property, Validator<TProperty> validator)
        => (@object) =>
        {
            var enumeration = property.Invoke(@object);

            foreach (var value in enumeration)
            {
                var validationResult = validator.Validate(value);

                if (validationResult != null &&
                    validationResult != ValidationRuleResult.Success)
                {
                    return validationResult;
                }
            }

            return ValidationRuleResult.Success;
        };

    public Func<T, ValidationResult?> CreateValidation<TProperty>(Func<T, TProperty> property, IValidationRule<TProperty> rule)
        => (value) =>
        {
            var valid = rule.Check(property.Invoke(value));

            return valid
                ? ValidationRuleResult.Success
                : new ValidationRuleResult(rule, rule.ValidationMessage);
        };

    public void Ensure(T value)
    {
        var validationResult = Validate(value);

        if (validationResult != null &&
            validationResult != ValidationRuleResult.Success)
        {
            ValidationException.Throw(validationResult, value);
        }
    }

    public ValidationResult? Validate(T value)
    {
        foreach (var validation in _validations)
        {
            var validationResult = validation.Invoke(value);

            if (validationResult != null &&
                validationResult != ValidationRuleResult.Success)
            {
                return validationResult;
            }
        }

        return ValidationRuleResult.Success;
    }
}