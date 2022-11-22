using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal sealed class MemberManager : IMemberManager
{
    private static IDictionary<string, Member[]>? _members;

    private const string MemberConfigurationKey = "Template:Administration:Members";

    public MemberManager(IConfiguration configuration)
    {
        var groupSections = configuration.GetRequiredSection(MemberConfigurationKey).GetChildren();

        _members ??= groupSections
            .ToDictionary(
                group => group.Key,
                group => group.Get<Member[]>() ?? throw new ManagementException($"Could not get `{MemberConfigurationKey}` configuration for group `{group.Key}`"));
    }

    public Member Get(string group, string uniqueName)
    {
        var member = GetAll(group)
            .SingleOrDefault(x => x.UniqueName == uniqueName);

        if (member == null)
        {
            throw new ManagementException($"Memeber `{uniqueName}` does not exist in group `{group}`");
        }

        return member;
    }

    public IEnumerable<Member> GetAll(string group)
    {
        var found = _members?.TryGetValue(group, out var members)
            ?? throw new UnreachableException($"The `{nameof(_members)}` field should never be null");

        if (!found)
        {
            throw new ManagementException($"Group `{group}` does not exist");
        }

        return members?.AsReadOnly()
            ?? throw new UnreachableException($"The local variable `{nameof(members)}` should never be null"); ;
    }
}