using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Authentication.Core;

internal static class Utf8JsonReaderExtensions
{
    public static bool TryReadStringProperty(this ref Utf8JsonReader reader, JsonEncodedText propertyName, [NotNullWhen(true)] out string? value)
    {
        if (!reader.ValueTextEquals(propertyName.EncodedUtf8Bytes))
        {
            value = default;
            return false;
        }

        reader.Read();
        value = reader.GetString()!;
        return true;
    }
}