using System.Text.Json;

namespace ChristianSchulz.MultitenancyMonolith.Shared;

public static class Utf8JsonWriterExtensions
{
    public static void WriteStringIfNotNull(this Utf8JsonWriter writer, JsonEncodedText propertyName, string? value)
    {
        if (value != null)
        {
            writer.WriteString(propertyName, value);
        }
    }
}