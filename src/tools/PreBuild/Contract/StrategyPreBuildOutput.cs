using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Contract;

public class StrategyPreBuildOutput : IPreBuildOutput
{
    private readonly PreBuildOutputOptions _options;
    private readonly PreBuildOutputResolver _resolver;

    public StrategyPreBuildOutput(
        IOptions<PreBuildOutputOptions> options,
        PreBuildOutputResolver resolver)
    {
        _options = options.Value;
        _resolver = resolver;
    }

    public async Task WriteAsync(string target, byte[] bytes)
    {
        await _resolver.Resolve(_options.OutputType).WriteAsync(target, bytes);
    }
}