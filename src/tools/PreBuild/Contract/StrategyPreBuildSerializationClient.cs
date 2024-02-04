using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Contract;

public class StrategyPreBuildSerializationClient : IPreBuildSerializationClient
{
    private readonly PreBuildSerializationClientOptions _options;
    private readonly PreBuildSerializationClientResolver _resolver;

    public StrategyPreBuildSerializationClient(
        IOptions<PreBuildSerializationClientOptions> options,
        PreBuildSerializationClientResolver resolver)
    {
        _options = options.Value;
        _resolver = resolver;
    }

    public async Task WriteAsync<T>(T @object)
    {
        await _resolver.Resolve(_options.SerializationClientType).WriteAsync(@object);
    }
}