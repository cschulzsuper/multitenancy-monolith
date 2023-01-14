﻿using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Requests;
using ChristianSchulz.MultitenancyMonolith.Application.Authentication.Responses;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public interface IIdentityRequestHandler
{
    ValueTask<IdentityResponse> GetAsync(string uniqueName);

    IQueryable<IdentityResponse> GetAll();

    ValueTask<IdentityResponse> InsertAsync(IdentityRequest request);

    ValueTask UpdateAsync(string uniqueName, IdentityRequest request);

    ValueTask DeleteAsync(string uniqueName);
}