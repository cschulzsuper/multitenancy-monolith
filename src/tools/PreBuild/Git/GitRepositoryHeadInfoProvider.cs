using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Git;

public sealed class GitRepositoryHeadInfoProvider
{
    private readonly GitClient _client;

    public GitRepositoryHeadInfoProvider(GitClient client)
    {
        _client = client;
    }

    public async Task<string> GetBranchNameAsync()
    {
        var output = await _client.ExecuteRevParseAsync("--abbrev-ref", "HEAD");

        var branchName = output.Trim();

        return branchName;
    }

    public async Task<string> GetCommitHashAsync()
    {
        var output = await _client.ExecuteRevParseAsync(string.Empty, "HEAD");

        var branchName = output.Trim();

        return branchName;
    }

    public async Task<string> GetShortCommitHashAsync()
    {
        var output = await _client.ExecuteRevParseAsync("--short", "HEAD");

        var branchName = output.Trim();

        return branchName;
    }
}
