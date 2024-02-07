using ChristianSchulz.MultitenancyMonolith.Application.Diagnostic.Responses;
using ChristianSchulz.MultitenancyMonolith.Configuration;

namespace ChristianSchulz.MultitenancyMonolith.Application.Diagnostic;

internal sealed class BuildInfoRequestHandler : IBuildInfoRequestHandler
{
    private readonly IConfigurationProxyProvider _configurationProxyProvider;

    public BuildInfoRequestHandler(IConfigurationProxyProvider configurationProxyProvider)
    {
        _configurationProxyProvider = configurationProxyProvider;
    }

    public BuildInfoResponse Get()
    {
        var buildInfo = _configurationProxyProvider.GetBuildInfo();

        var response = new BuildInfoResponse
        {
            BranchName = buildInfo.BranchName,
            BuildNumber = buildInfo.BuildNumber,
            CommitHash = buildInfo.CommitHash,
            ShortCommitHash = buildInfo.ShortCommitHash

        };

        return response;
    }
}