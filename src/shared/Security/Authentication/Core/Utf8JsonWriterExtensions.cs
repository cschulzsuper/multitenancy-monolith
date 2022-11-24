using System.Text.Json;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Core;

internal static class Utf8JsonWriterExtensions
{
    public static void WriteStringIfNotNull(this Utf8JsonWriter writer, JsonEncodedText propertyName, string? value)
    {
        if (value != null)
        {
            writer.WriteString(propertyName, value);
        }
    }
}