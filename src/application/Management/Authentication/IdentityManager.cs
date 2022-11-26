using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

internal sealed class IdentityManager : IIdentityManager
{
    private static Identity[]? _identities;

    private const string IdentitiesConfigurationKey = "SeedData:Authentication:Identities";

    public IdentityManager(IConfiguration configuration)
    {
        _identities ??= configuration.GetRequiredSection(IdentitiesConfigurationKey).Get<Identity[]>()
            ?? throw new ManagementException($"Could not get `{IdentitiesConfigurationKey}` configuration");
    }

    public Identity Get(string uniqueName)
        => _identities?.Single(x => x.UniqueName == uniqueName)
            ?? throw new UnreachableException($"The `{nameof(_identities)}` field should never be null");

    public IEnumerable<Identity> GetAll()
        => _identities?.AsReadOnly()
            ?? throw new UnreachableException($"The `{nameof(_identities)}` field should never be null");
}