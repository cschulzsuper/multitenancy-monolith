using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge.Serialization;

internal sealed class ClaimsJsonConverter : JsonConverter<Claim[]>
{
    private static readonly JsonEncodedText Badge = JsonEncodedText.Encode("badge");
    private static readonly JsonEncodedText Client = JsonEncodedText.Encode("client");
    private static readonly JsonEncodedText Identity = JsonEncodedText.Encode("identity");
    private static readonly JsonEncodedText Group = JsonEncodedText.Encode("group");
    private static readonly JsonEncodedText Member = JsonEncodedText.Encode("member");
    private static readonly JsonEncodedText Mail = JsonEncodedText.Encode("mail");
    private static readonly JsonEncodedText Verification = JsonEncodedText.Encode("verification");

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

        if (reader.TryReadStringProperty(Badge, out var stringValue))
        {
            value.Add(new Claim("badge", stringValue, ClaimValueTypes.Base64Binary));
            return;
        }

        if (reader.TryReadStringProperty(Client, out stringValue))
        {
            value.Add(new Claim("client", stringValue));
            return;
        }

        if (reader.TryReadStringProperty(Identity, out stringValue))
        {
            value.Add(new Claim("identity", stringValue));
            return;
        }

        if (reader.TryReadStringProperty(Group, out stringValue))
        {
            value.Add(new Claim("group", stringValue));
            return;
        }

        if (reader.TryReadStringProperty(Member, out stringValue))
        {
            value.Add(new Claim("member", stringValue));
            return;
        }

        if (reader.TryReadStringProperty(Mail, out stringValue))
        {
            value.Add(new Claim("mail", stringValue, ClaimValueTypes.Base64Binary));
            return;
        }

        if (reader.TryReadStringProperty(Verification, out stringValue))
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
            "badge" => Badge,
            "client" => Client,
            "identity" => Identity,
            "group" => Group,
            "member" => Member,
            "mail" => Mail,
            "verification" => Verification,
            _ => null
        };
}