using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application;

public sealed class ConfigurationException : Exception
{
    private ConfigurationException(string message) : base(message) { }

    [DoesNotReturn]
    public static void ThrowNotConfigured(string configuration)
    {
        var exception = new ConfigurationException($"Configuration '{configuration}' is not configured.");

        throw exception;
    }
}