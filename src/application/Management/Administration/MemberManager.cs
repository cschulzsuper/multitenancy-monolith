using ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal sealed class MemberManager : IMemberManager
{
    private readonly ClaimsPrincipal _user;

    private static IDictionary<string, Member[]>? _members;

    private const string MemberConfigurationKey = "SeedData:Administration:Members";

    public MemberManager(
        IConfiguration configuration, 
        ClaimsPrincipal user)
    {
        var groupSections = configuration.GetRequiredSection(MemberConfigurationKey).GetChildren();

        _members ??= groupSections
            .ToDictionary(
                group => group.Key,
                group => group.Get<Member[]>() ?? throw new ManagementException($"Could not get `{MemberConfigurationKey}` configuration for group `{group.Key}`"));
        
        _user = user;
    }

    public Member Get(string uniqueName)
    {
        var member = GetAll()
            .SingleOrDefault(x => x.UniqueName == uniqueName);

        if (member == null)
        {
            throw new ManagementException($"Memeber `{uniqueName}` does not exist");
        }

        return member;
    }

    public IEnumerable<Member> GetAll()
    {
        var group = _user.GetClaim("Group");

        var found = _members?.TryGetValue(group, out var members)
            ?? throw new UnreachableException($"The `{nameof(_members)}` field should never be null");

        if (!found)
        {
            throw new ManagementException($"Could not query members");
        }

        return members?.AsReadOnly()
            ?? throw new UnreachableException($"The local variable `{nameof(members)}` should never be null"); ;
    }
}