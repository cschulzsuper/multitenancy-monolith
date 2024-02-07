using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Git;

public sealed class GitClient
{
    private readonly string _repositoryPath;

    public GitClient(IOptions<GitClientOptions> options)
    {
        _repositoryPath = options.Value.RepositoryPath;
    }

    public async Task<string> ExecuteRevParseAsync(string options, string args)
    {
        using Process process = new();

        process.StartInfo.WorkingDirectory = _repositoryPath;

        process.StartInfo.FileName = "git";
        process.StartInfo.Arguments = $"rev-parse {options} {args}";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        process.Start();

        string output = await process.StandardOutput.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new Exception("Error");
        }

        return output;
    }
}
