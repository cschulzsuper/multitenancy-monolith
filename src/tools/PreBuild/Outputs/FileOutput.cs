using ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Contract;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Outputs;

public sealed class FileOutput : IPreBuildOutput
{
    private readonly FileOutputOptions _options;

    public FileOutput(IOptions<FileOutputOptions> options)
    {
        _options = options.Value;
    }

    public async Task WriteAsync(string target, byte[] bytes)
    {
        var outputDirectory = _options.OutputDirectoryPath;

        var outputFilenameFormat = _options.OutputFilenameFormat ?? "{0}";
        var outputFilename = string.Format(outputFilenameFormat, target);

        var outputPath = Path.Combine(outputDirectory, outputFilename);

        await File.WriteAllBytesAsync(outputPath, bytes);
    }
}