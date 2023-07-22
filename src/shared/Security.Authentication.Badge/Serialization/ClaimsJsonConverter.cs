using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge.Serialization;

internal sealed class ClaimsJsonConverter : JsonConverter<Claim[]>
{
    private static readonly JsonEncodedText _type = JsonEncodedText.Encode("type");
    private static readonly JsonEncodedText _client = JsonEncodedText.Encode("client");
    private static readonly JsonEncodedText _identity = JsonEncodedText.Encode("identity");
    private static readonly JsonEncodedText _group = JsonEncodedText.Encode("group");
    private static readonly JsonEncodedText _member = JsonEncodedText.Encode("member");
    private static readonly JsonEncodedText _mail = JsonEncodedText.Encode("mail");
    private static readonly JsonEncodedText _verification = JsonEncodedText.Encode("verification");

    public override Claim[] Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        var request = new List<Claim>();

        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException("Unexpected end when reading JSON.");
        }

        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Unexpected end when reading JSON.");
            }

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                ReadValue(ref reader, request, options);
            }

            if (reader.TokenType != JsonTokenType.EndObject)
            {
                throw new JsonException("Unexpected end when reading JSON.");
            }
        }

        if (reader.TokenType != JsonTokenType.EndArray)
        {
            throw new JsonException("Unexpected end when reading JSON.");
        }

        return request.ToArray();
    }

    internal static void ReadValue(ref Utf8JsonReader reader, ICollection<Claim> value,
        JsonSerializerOptions _)
    {

        if (reader.TryReadStringProperty(_type, out var stringValue))
        {
            value.Add(new Claim("type", stringValue, ClaimValueTypes.Base64Binary));
            return;
        }

        if (reader.TryReadStringProperty(_client, out stringValue))
        {
            value.Add(new Claim("client", stringValue));
            return;
        }

        if (reader.TryReadStringProperty(_identity, out stringValue))
        {
            value.Add(new Claim("identity", stringValue));
            return;
        }

        if (reader.TryReadStringProperty(_group, out stringValue))
        {
            value.Add(new Claim("group", stringValue));
            return;
        }

        if (reader.TryReadStringProperty(_member, out stringValue))
        {
            value.Add(new Claim("member", stringValue));
            return;
        }

        if (reader.TryReadStringProperty(_mail, out stringValue))
        {
            value.Add(new Claim("mail", stringValue, ClaimValueTypes.Base64Binary));
            return;
        }

        if (reader.TryReadStringProperty(_verification, out stringValue))
        {
            value.Add(new Claim("verification", stringValue, ClaimValueTypes.Base64Binary));
            return;
        }
    }

    public override void Write(Utf8JsonWriter writer, Claim[] value,
        JsonSerializerOptions _)
    {
        writer.WriteStartArray();

        foreach (var claim in value)
        {
            var propertyName = GetPropertyNameOrDefault(claim.Type);
            var propertyValue = claim.Value;

            if (propertyName != null)
            {
                writer.WriteStartObject();
                writer.WriteStringIfNotNull(propertyName.Value, propertyValue);
                writer.WriteEndObject();
            }
        }

        writer.WriteEndArray();
    }

    private static JsonEncodedText? GetPropertyNameOrDefault(string claimType)
        => claimType switch
        {
            "type" => _type,
            "client" => _client,
            "identity" => _identity,
            "group" => _group,
            "member" => _member,
            "mail" => _mail,
            "verification" => _verification,
            _ => null
        };
}