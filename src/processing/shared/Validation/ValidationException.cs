using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;
using Humanizer;
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation;

public sealed class ValidationException : Exception
{
    private ValidationException(string message) : base(message) { }

    [DoesNotReturn]
    public static void Throw<TValue>(ValidationResult validationResult, TValue value)
    {
        if (value is IEnumerable || value is ValueType)
        {
            ThrowValueInvalid(validationResult, value);
        }
        else
        {
            ThrowObjectInvalid<TValue>(validationResult);
        }
    }

    [DoesNotReturn]
    public static void ThrowValueInvalid<TValue>(ValidationResult validationResult, TValue value)
    {
        ValidationException exception;

        if (validationResult.ErrorMessage != null)
        {
            exception = new ValidationException(validationResult.ErrorMessage);
        }
        else
        {
            var formattedValue = FormatValue(value);
            exception = new ValidationException($"Value '{formattedValue}' is not valid.");
        }

        exception.Data["error-code"] = "value-invalid";

        throw exception;
    }

    [DoesNotReturn]
    public static void ThrowObjectInvalid<TEntity>(ValidationResult validationResult)
    {
        var objectType = typeof(TEntity).Name.Humanize();

        ValidationException exception;

        if (validationResult.ErrorMessage != null)
        {
            exception = new ValidationException(validationResult.ErrorMessage);
        }
        else
        {
            exception = new ValidationException($"Object '{objectType}' is not valid.");
        }

        exception.Data["error-code"] = 
            validationResult is ValidationRuleResult validationRuleResult &&
            validationRuleResult.Rule.GetType().IsGenericType &&
            validationRuleResult.Rule.GetType().GetGenericTypeDefinition() == typeof(UniquePropertyValueValidationRule<>)
            ? "object-conflict"
            : "object-invalid";

        exception.Data["object-type"] = objectType;

        throw exception;
    }

    private static string FormatValue<TValue>(TValue value)
    {
        if (value == null)
        {
            return "unknown";
        }

        if (value is string valueString)
        {
            return valueString.Length > 40
                ? FormatValueString(valueString)
                : valueString;
        }

        var valueType = typeof(TValue);

        if (valueType.IsPrimitive)
        {
            return value.ToString() ?? "unknown";
        }

        return "unknown";
    }

    private static string FormatValueString(string value)
    {
        var begin = value[..10].TrimEnd('.');
        var end = value.Substring(value.Length - 11, 10).TrimEnd('.');

        return $"{begin}...{end}";
    }
}