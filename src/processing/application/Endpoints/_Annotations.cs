using System;

namespace ChristianSchulz.MultitenancyMonolith.Application;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ErrorMessageAttribute : Attribute
{
    public string ErrorMessage { get; }

    public ErrorMessageAttribute(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class AuthenticationAttribute : Attribute
{

}