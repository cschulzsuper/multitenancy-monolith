namespace ChristianSchulz.MultitenancyMonolith.Shared.Validation;

public static class ValidationErrors
{
    public const string ValueNotAllowed = "Value '{0}' is not allowed.";

    public const string ValueNotCamelCased = "Value '{0}' must be camelCased.";

    public const string ValueNotKebabCased = "Value '{0}' must be kebab-cased.";

    public const string ValueNotMailAddress = "Value '{0}' must be a valid mail address.";

    public const string ValueIsDefault = "Value '{0}' cannot be default.";

    public const string ValueIsEmpty = "Value '{0}' cannot be empty.";

    public const string ValueIsNegative = "Value '{0}' cannot be negative.";

    public const string ValueIsNull = "Value '{0}' cannot be null.";

    public const string ValueIsZero = "Value '{0}' cannot be zero.";

    public const string ValueTooLong = "Value '{0}' must not exceed {1} chracters.";

    public const string ValueNotZero = "Value '{0}' must be zero.";
}