using ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Contract;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Serializations;

public sealed class JsonSerializationClient : IPreBuildSerializationClient
{
    private readonly IPreBuildOutput _output;

    private static readonly JsonSerializerOptions _jsonSerializerOptions
        = new()
        {
            WriteIndented = true
        };

    public JsonSerializationClient(IPreBuildOutput output)
    {
        _output = output;
    }

    public async Task WriteAsync<T>(T @object)
    {
        var typeName = typeof(T).Name;

        var wrapper = new Dictionary<string, T>
        {
            { typeName, @object }
        };

        var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(wrapper, _jsonSerializerOptions);

        await _output.WriteAsync(typeName, jsonBytes);
    }
}