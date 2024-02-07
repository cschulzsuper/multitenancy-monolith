using ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Contract;
using ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Git;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Tools.PreBuild;

public sealed class PreBuildService
{
    private readonly PreBuildSettings _settings;
    private readonly IPreBuildSerializationClient _serializationClient;
    private readonly GitRepositoryHeadInfoProvider _gitRepositoryHeadInfoProvider;

    public PreBuildService(
        PreBuildSettings settings,
        IPreBuildSerializationClient serializationClient,
        GitRepositoryHeadInfoProvider gitRepositoryHeadInfoProvider)
    {
        _settings = settings;
        _serializationClient = serializationClient;
        _gitRepositoryHeadInfoProvider = gitRepositoryHeadInfoProvider;
    }

    internal async Task RunAsync()
    {
        var buildInfo = new BuildInfo
        {
            BuildNumber = await GetBuildNumberAsync(),
            BranchName = await GetBranchNameAsync(),
            CommitHash = await GetCommitHashAsync(),
            ShortCommitHash = await GetShortCommitHashAsync(),
        };

        await _serializationClient.WriteAsync(buildInfo);
    }

    private async Task<string?> GetShortCommitHashAsync()
    {
        if (_settings.ShortCommitHash != null)
            return _settings.ShortCommitHash;

        return await _gitRepositoryHeadInfoProvider.GetShortCommitHashAsync();
    }

    private async Task<string?> GetCommitHashAsync()
    {
        if (_settings.CommitHash != null)
            return _settings.CommitHash;

        return await _gitRepositoryHeadInfoProvider.GetCommitHashAsync();
    }

    private async Task<string?> GetBranchNameAsync()
    {
        if (_settings.BranchName != null)
            return _settings.BranchName;

        return await _gitRepositoryHeadInfoProvider.GetBranchNameAsync();
    }

    private async Task<string?> GetBuildNumberAsync()
    {
        await Task.CompletedTask;

        if (_settings.BuildNumber != null)
            return _settings.BuildNumber;

        return null;
    }
}