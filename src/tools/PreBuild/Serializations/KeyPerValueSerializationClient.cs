using ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Contract;
using Microsoft.Extensions.Options;
using System.Text;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Serializations;

public sealed class KeyPerValueSerializationClient : IPreBuildSerializationClient
{
    private readonly IPreBuildOutput _output;

    public KeyPerValueSerializationClient(IPreBuildOutput output)
    {
        _output = output;
    }

    public async Task WriteAsync<T>(T @object)
    {
        var type = typeof(T);
        var typeName = typeof(T).Name;

        var properties = type.GetProperties();

        foreach (var property in properties)
        {
            var propertyName = property.Name;

            var propertyValue = property.GetValue(@object);
            if (propertyValue == null) continue;

            var propertyStringValue = propertyValue.ToString();
            if (propertyStringValue == null) continue;

            var sinkName = $"{typeName}__{propertyName}";
            var sinkValue = Encoding.UTF8.GetBytes(propertyStringValue.Trim());

            await _output.WriteAsync(sinkName, sinkValue);
        }
    }
}