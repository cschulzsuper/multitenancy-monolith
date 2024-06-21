using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedValidationRules;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation;

public static class ValidationRules
{
    public static IValidationRule<string> AllowedValues(string field, params string[] allowedValues)
        => new AllowedValuesValidationRule(field, allowedValues);

    public static IValidationRule<string> CamelCase(string field)
        => new CamelCaseValidationRule(field);

    public static IValidationRule<string> CronExpression(string field)
        => new CronExpressionValidationRule(field);

    public static IValidationRule<string> KebabCase(string field)
        => new KebabCaseValidationRule(field);

    public static IValidationRule<string> LowerCase(string field)
        => new LowerCaseValidationRule(field);

    public static IValidationRule<string> MailAddress(string field)
        => new MailAddressValidationRule(field);

    public static IValidationRule<T> NotDefault<T>(string field)
        where T : struct, IEquatable<T>
        => new NotDefaultValidationRule<T>(field);

    public static IValidationRule<string> NotEmpty(string field)
        => new NotEmptyValidationRule(field);

    public static IValidationRule<T> NotNegative<T>(string field)
        where T : INumber<T>
        => new NotNegativeValidationRule<T>(field);

    public static IValidationRule<string> NotNull(string field)
        => new NotNullValidationRule(field);

    public static IValidationRule<T> NotZero<T>(string field)
        where T : INumber<T>
        => new NotZeroValidationRule<T>(field);

    public static IValidationRule<string> StringLength(string field, int length)
        => new StringLengthValidationRule(field, length);

    public static IValidationRule<IEnumerable<T>> Unique<T>(string field, Func<T, IComparable> select)
        => new UniqueValidationRule<T>(field, select);

    public static IValidationRule<T> Zero<T>(string field) 
        where T : INumber<T>
        => new ZeroValidationRule<T>(field);
}
