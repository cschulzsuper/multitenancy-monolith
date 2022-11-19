using System.Text.Json;

namespace ChristianSchulz.MultitenancyMonolith.Shared;

public static class ClaimsJsonSerializerOptions
{
    public static readonly JsonSerializerOptions Options;

    static ClaimsJsonSerializerOptions()
    {
        Options = new JsonSerializerOptions(JsonSerializerOptions.Default);
        Options.Converters.Add(new ClaimsJsonConverter());
    }
}
