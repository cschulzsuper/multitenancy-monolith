using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using ChristianSchulz.MultitenancyMonolith.Frontend.Portal.Services.Diagnostic.Models;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.Portal.Services.Diagnostic;

public sealed class BuildInfoService
{
    private readonly BuildInfo _buildInfo;

    public BuildInfoService(IConfigurationProxyProvider configurationProxyProvider)
    {
        _buildInfo = configurationProxyProvider.GetBuildInfo();
    }

    public BuildInfoModel Get()
    {
        var model = new BuildInfoModel
        {
            BranchName = _buildInfo.BranchName,
            BuildNumber = _buildInfo.BuildNumber,
            CommitHash = _buildInfo.CommitHash,
            ShortCommitHash = _buildInfo.ShortCommitHash
        };

        return model;
    }
}
