﻿using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Responses;
using System.Collections.Generic;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

internal sealed class IdentityRequestHandler : IIdentityRequestHandler
{
    private readonly IIdentityManager _identityManager;

    public IdentityRequestHandler(IIdentityManager identityManager)
    {
        _identityManager = identityManager;
    }

    public IdentityResponse Get(string uniqueName)
    {
        var member = _identityManager.Get(uniqueName);

        var response = new IdentityResponse
        {
            UniqueName = member.UniqueName
        };

        return response;
    }

    public IEnumerable<IdentityResponse> GetAll()
    {
        var identities = _identityManager.GetAll();

        var response = identities.Select(member =>
            new IdentityResponse
            {
                UniqueName = member.UniqueName
            });

        return response;
    }
}