using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal sealed class MembershipManager : IMembershipManager
{
    private static IDictionary<string, Membership[]>? _membership;

    private const string MembershipConfigurationKey = "SeedData:Administration:Memberships";

    public MembershipManager(IConfiguration configuration)
    {
        var membershipSection = configuration.GetRequiredSection(MembershipConfigurationKey);

        _membership ??= membershipSection.Get<Membership[]>()?
            .GroupBy(x => x.Group)
            .ToDictionary(
                membership => membership.Key,
                membership => membership.ToArray())

             ?? throw new ManagementException($"Could not get `{MembershipConfigurationKey}` configuration"); ;
    }

    public IEnumerable<Membership> GetAll(string group)
    {
        var found = _membership?.TryGetValue(group, out var memberships)
            ?? throw new UnreachableException($"The `{nameof(_membership)}` field should never be null");

        if (!found)
        {
            throw new ManagementException($"Group `{group}` does not exist");
        }

        return memberships?.AsReadOnly()
            ?? throw new UnreachableException($"The local variable `{nameof(memberships)}` should never be null"); ;
    }
}