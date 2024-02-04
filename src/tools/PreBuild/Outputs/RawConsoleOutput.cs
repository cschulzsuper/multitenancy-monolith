using ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Contract;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Outputs;

public sealed class RawConsoleOutput : IPreBuildOutput
{
    public Task WriteAsync(string target, byte[] bytes)
    {
        var decodedValue = Encoding.UTF8.GetString(bytes);

        Console.WriteLine(decodedValue);

        return Task.CompletedTask;
    }
}