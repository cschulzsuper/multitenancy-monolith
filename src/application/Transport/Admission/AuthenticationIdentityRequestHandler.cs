using ChristianSchulz.MultitenancyMonolith.Application.Admission.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Admission.Responses;
using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using ChristianSchulz.MultitenancyMonolith.Shared.QuerySearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

internal sealed class AuthenticationIdentityRequestHandler : IAuthenticationIdentityRequestHandler
{
    private const string SearchTermKeyMailAddress = "mail-address";
    private const string SearchTermKeyUniqueName = "unique-name";


    private readonly IAuthenticationIdentityManager _authenticationIdentityManager;

    public AuthenticationIdentityRequestHandler(IAuthenticationIdentityManager authenticationIdentityManager)
    {
        _authenticationIdentityManager = authenticationIdentityManager;
    }


    public async Task HeadAsync(string authenticationIdentity)
    {
        var exists = await _authenticationIdentityManager.ExistsAsync(authenticationIdentity);

        if (!exists)
        {
            TransportException.ThrowNotFound<AuthenticationIdentity>(authenticationIdentity);
        }
    }

    public async Task<AuthenticationIdentityResponse> GetAsync(string authenticationIdentity)
    {
        var @object = await _authenticationIdentityManager.GetAsync(authenticationIdentity);

        var response = new AuthenticationIdentityResponse
        {
            UniqueName = @object.UniqueName,
            MailAddress = @object.MailAddress
        };

        return response;
    }

    public async IAsyncEnumerable<AuthenticationIdentityResponse> GetAll(string? query, int? skip, int? take)
    {
        var objects = _authenticationIdentityManager
                .GetAsyncEnumerable(queryable =>
                {
                    queryable = WhereSearchQuery(queryable, query)
                        .OrderBy(x => x.UniqueName);

                    if (skip != null) queryable = queryable.Skip(skip.Value);
                    if (take != null) queryable = queryable.Take(take.Value);

                    return queryable;
                });

        await foreach (var @object in objects)
        {
            var response = new AuthenticationIdentityResponse
            {
                UniqueName = @object.UniqueName,
                MailAddress = @object.MailAddress
            };

            yield return response;
        }
    }

    public async Task<AuthenticationIdentityResponse> InsertAsync(AuthenticationIdentityRequest request)
    {
        var @object = new AuthenticationIdentity
        {
            UniqueName = request.UniqueName,
            MailAddress = request.MailAddress,
            Secret = $"{Guid.NewGuid()}"
        };

        await _authenticationIdentityManager.InsertAsync(@object);

        var response = new AuthenticationIdentityResponse
        {
            UniqueName = @object.UniqueName,
            MailAddress = @object.MailAddress
        };

        return response;
    }

    public async Task UpdateAsync(string authenticationIdentity, AuthenticationIdentityRequest request)
    => await _authenticationIdentityManager.UpdateAsync(authenticationIdentity,
        @object =>
        {
            @object.UniqueName = request.UniqueName;
            @object.MailAddress = request.MailAddress;
        });

    public async Task DeleteAsync(string authenticationIdentity)
        => await _authenticationIdentityManager.DeleteAsync(authenticationIdentity);

    private static IQueryable<AuthenticationIdentity> WhereSearchQuery(IQueryable<AuthenticationIdentity> query, string? searchQuery)
    {
        if (searchQuery == null)
        {
            return query;
        }

        var searchTerms = SearchQueryParser.Parse(searchQuery);
        var uniqueNames = searchTerms.GetValidSearchTermValues<string>(SearchTermKeyUniqueName);
        var mailAddresses = searchTerms.GetValidSearchTermValues<string>(SearchTermKeyMailAddress);

        query = query.Where(x => 
            (!uniqueNames.Any() && !mailAddresses.Any()) ||
            (uniqueNames.Any() && uniqueNames.Contains(x.UniqueName)) ||
            (mailAddresses.Any() && mailAddresses.Contains(x.MailAddress)));

        return query;
    }
}