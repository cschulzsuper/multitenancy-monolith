using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChristianSchulz.MultitenancyMonolith.Server.Json;

public class ObjectJsonConverter : JsonConverter<object?>
{
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType switch
        {
            JsonTokenType.Null => null,
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.Number when reader.TryGetInt64(out long l) => l,
            JsonTokenType.Number => reader.GetDecimal(),
            JsonTokenType.String => reader.GetString()!,
            _ => JsonDocument.ParseValue(ref reader).RootElement.Clone()
        };

    public override void Write(Utf8JsonWriter writer, object? objectToWrite, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, objectToWrite, objectToWrite?.GetType() ?? typeof(object), options);
}