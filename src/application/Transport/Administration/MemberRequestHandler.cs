using ChristianSchulz.MultitenancyMonolith.Application.Administration.Responses;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

internal sealed class MemberRequestHandler : IMemberRequestHandler
{
    private readonly IMemberManager _memberManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MemberRequestHandler(
        IMemberManager memberManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _memberManager = memberManager;
        _httpContextAccessor = httpContextAccessor;
    }

    private string Group => _httpContextAccessor.HttpContext.User.FindFirstValue("Group")
        ?? throw new TransportException("Could not find 'Group' claim");

    public MemberResponse Get(string uniqueName)
    {
        var member = _memberManager.Get(Group, uniqueName);

        var response = new MemberResponse
        {
            UniqueName = member.UniqueName,
            Group = member.Group,
            Identity = member.Identity
        };

        return response;
    }

    public IEnumerable<MemberResponse> GetAll()
    {
        var members = _memberManager.GetAll(Group);

        var response = members.Select(member =>
            new MemberResponse
            {
                UniqueName = member.UniqueName,
                Group = member.Group,
                Identity = member.Identity
            });

        return response;
    }
}
