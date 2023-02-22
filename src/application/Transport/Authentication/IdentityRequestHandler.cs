﻿using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Responses;
using ChristianSchulz.MultitenancyMonolith.Objects.Authentication;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

internal sealed class IdentityRequestHandler : IIdentityRequestHandler
{
    private readonly IIdentityManager _identityManager;

    public IdentityRequestHandler(IIdentityManager identityManager)
    {
        _identityManager = identityManager;
    }

    public async Task<IdentityResponse> GetAsync(string uniqueName)
    {
        var identity = await _identityManager.GetAsync(uniqueName);

        var response = new IdentityResponse
        {
            UniqueName = identity.UniqueName,
            MailAddress = identity.MailAddress
        };

        return response;
    }

    public IQueryable<IdentityResponse> GetAll()
    {
        var identities = _identityManager.GetAll();

        var response = identities.Select(member =>
            new IdentityResponse
            {
                UniqueName = member.UniqueName,
                MailAddress = member.MailAddress
            });

        return response;
    }

    public async Task<IdentityResponse> InsertAsync(IdentityRequest request)
    {
        var member = new Identity
        {
            UniqueName = request.UniqueName,
            MailAddress = request.MailAddress,
            Secret = request.Secret
        };

        await _identityManager.InsertAsync(member);

        var response = new IdentityResponse
        {
            UniqueName = member.UniqueName,
            MailAddress = member.MailAddress
        };

        return response;
    }

    public async Task UpdateAsync(string uniqueName, IdentityRequest request)
    => await _identityManager.UpdateAsync(uniqueName,
        member =>
        {
            member.UniqueName = request.UniqueName;
            member.Secret = request.Secret;
            member.MailAddress = request.MailAddress;
        });

    public async Task DeleteAsync(string uniqueName)
        => await _identityManager.DeleteAsync(uniqueName);
}