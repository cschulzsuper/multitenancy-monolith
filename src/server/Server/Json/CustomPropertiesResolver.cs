using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ChristianSchulz.MultitenancyMonolith.Server.Json;

public sealed class CustomPropertiesResolver : DefaultJsonTypeInfoResolver
{
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var typeInfo = base.GetTypeInfo(type, options);
        if (typeInfo.Kind == JsonTypeInfoKind.Object)
        {
            typeInfo.OnDeserialized = ConvertDictionaries;
        }

        return typeInfo;
    }

    private void ConvertDictionaries(object @object)
    {
        var dictionaries = @object.GetType()
            .GetProperties()
            .Where(property => property.PropertyType == typeof(IDictionary<string, object>))
            .Select(property => (IDictionary<string, object>)property.GetValue(@object)!)
            .ToArray();

        foreach (var dictionary in dictionaries)
        {
            var keys = dictionary.Keys.ToArray();

            foreach (var key in keys)
            {
                if (dictionary[key] is not JsonElement jsonElement)
                {
                    dictionary.Remove(key);
                    continue;
                }

                object? value = jsonElement.ValueKind switch
                {
                    JsonValueKind.Null => null,
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Number when jsonElement.TryGetInt64(out long l) => l,
                    JsonValueKind.Number => jsonElement.GetDecimal(),
                    JsonValueKind.String => jsonElement.GetString()!,
                    _ => null
                };

                if (value == null)
                {
                    dictionary.Remove(key);
                    continue;
                }

                dictionary[key] = value;
            }
        }
    }
}

